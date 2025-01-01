using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 17f;
    [SerializeField] private float extraGravity = 700f;
    [SerializeField] private float gravityDelay = .2f;
    [SerializeField] private float coyoteTime = .1f;
    [Header("Ground")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    [Header("Walls")]
    [SerializeField] private float wallJumpTime = .4f;
    [SerializeField] private float wallJumpXForce = 25f;
    [SerializeField] private Transform wallCheckPos;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private PhysicsMaterial2D slippyMaterial;
    [Header("Attack")]
    [SerializeField] private GameObject kunaiPrefab;
    [SerializeField] private Transform kunaiSpawnPoint;
    [SerializeField] private float kunaiSpeed = 75f;
    [SerializeField] private Transform wallKunaiSpawnPoint;
    [SerializeField] private float timeToAttackAgain = .5f;
    [Header("Falling")]
    [SerializeField] private float maxFallVelocity = -30f;

    private bool stoppedJumpEarly, isAttacking, canAttack = true, isWallJumping;
    private float timeInAir, coyoteTimer, attackTimer;
    public bool hasTaco;
    private bool canMove = true;
    private IEnumerator wallJumpRoutine;
    private IEnumerator respawnRoutine;

    [HideInInspector] public bool isGrounded, isOnWall;

    private Rigidbody2D rigidBody;
    private PlayerInput playerInput;
    private Animator animator;
    private FrameInput frameInput;
    private Timer timer;
    // private CinemachinePositionComposer camPosComposer;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        timer = FindFirstObjectByType<Timer>();
        // camPosComposer = FindFirstObjectByType<CinemachinePositionComposer>();
    }

    private void Update()
    {
        if (canMove) {
            GatherInput();
            CheckCurrentState();
            CoyoteTimer();
            HandleJump();
            AttackTimer();
            HandleAttack();
            DidStopJumpEarly();
            GravityDelay();
            if (!isWallJumping)
            {
                FlipSprite();
            }
        }
        else {
            animator.Play("Idle");
            animator.Play("ArmIdle");
        }
    }

    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            Move();
        }
        ApplyExtraGravity();
    }

    private void GatherInput()
    {
        frameInput = playerInput.FrameInput;
    }

    private void Move()
    {
        if (canMove) {
            rigidBody.linearVelocityX = frameInput.Move.x * moveSpeed;
        }
    }

    public void DisableMovement() {
        canMove = false;
        rigidBody.linearVelocityX = 0f;
    }

    public void EnableMovement() {
        canMove = true;
    }

    private void CheckCurrentState()
    {
        Collider2D groundCheck = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);
        Collider2D wallCheck = Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallLayer);
        isGrounded = groundCheck;
        isOnWall = wallCheck;

        // if (!isGrounded && !isOnWall && rigidBody.linearVelocityY < 0f) {
        //     camPosComposer.Lookahead.Time = 0.25f;
        // }
        // else {
        //     camPosComposer.Lookahead.Time = 0.9f;
        // }
        

        if (isGrounded && rigidBody.linearVelocityY <= 0f)
        {
            if (frameInput.Move.x != 0f)
            {
                animator.Play("Running");
                if (!isAttacking)
                {
                    animator.Play("ArmRunning");
                }
            }
            else
            {
                animator.Play("Idle");
                if (!isAttacking)
                {
                    animator.Play("ArmIdle");
                }
            }
            rigidBody.sharedMaterial = slippyMaterial;
        }
        else if (!isGrounded && !isOnWall)
        {
            animator.Play("Jumping");
            if (!isAttacking)
            {
                animator.Play("ArmIdle");
            }
            rigidBody.sharedMaterial = slippyMaterial;
        }
        else if (!isGrounded && isOnWall)
        {
            animator.Play("WallSliding");
            rigidBody.sharedMaterial = null;
        }
    }

    public void JustRespawned() {
        rigidBody.linearVelocity = Vector2.zero;
        canMove = false;
        if (respawnRoutine != null)
        {
            StopCoroutine(respawnRoutine);
        }
        respawnRoutine = JustRespawnedCO();
        StartCoroutine(respawnRoutine);
    }

    private IEnumerator JustRespawnedCO() {
        yield return new WaitForSeconds(2f);
        canMove = true;
        timer.UnPauseTimer();
        timer.TimerTextWhite();
    }

    #region Jump Methods
    private void HandleJump()
    {
        if (!frameInput.Jump) { return; }

        if (coyoteTimer > 0f || isGrounded)
        {
            ApplyJumpForce();
        }
        else if (!isGrounded && isOnWall)
        {
            ApplyWallJumpForce();
        }
    }

    private void ApplyJumpForce()
    {
        rigidBody.linearVelocity = Vector2.zero;
        timeInAir = 0f;
        coyoteTimer = 0f;
        stoppedJumpEarly = false;
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void ApplyWallJumpForce()
    {
        rigidBody.linearVelocity = Vector2.zero;
        timeInAir = 0f;
        coyoteTimer = 0f;
        stoppedJumpEarly = false;
        rigidBody.AddForce(new Vector2(-transform.localScale.x * jumpForce, jumpForce), ForceMode2D.Impulse);
        if (wallJumpRoutine != null)
        {
            StopCoroutine(wallJumpRoutine);
        }
        wallJumpRoutine = WallJumpTimeRoutine();
        StartCoroutine(wallJumpRoutine);
    }

    private IEnumerator WallJumpTimeRoutine()
    {
        isWallJumping = true;
        // flip sprite manually
        if (transform.localScale.x == 1)
        {
            transform.localScale = new(-1, 1, 1);
        }
        else
        {
            transform.localScale = new(1, 1, 1);
        }
        yield return new WaitForSeconds(wallJumpTime);
        wallJumpRoutine = null;
        isWallJumping = false;
    }

    private void CoyoteTimer()
    {
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    private void DidStopJumpEarly()
    {
        if (!frameInput.JumpRelease) { return; }
        stoppedJumpEarly = rigidBody.linearVelocityY > 0.01f;
    }

    private void GravityDelay()
    {
        if (!isGrounded)
        {
            timeInAir += Time.deltaTime;
        }
        else
        {
            timeInAir = 0f;
        }
    }

    private void ApplyExtraGravity()
    {
        if (!isGrounded)
        {
            if (timeInAir > gravityDelay || stoppedJumpEarly)
            {
                rigidBody.AddForce(new Vector2(0f, -extraGravity * Time.deltaTime));
                // max fall speed
                if (rigidBody.linearVelocityY < maxFallVelocity) {
                    rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocityX, maxFallVelocity);
                }
            }
        }
    }
    #endregion

    #region Attack Methods
    private void AttackTimer()
    {
        if (canAttack) { return; }
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            canAttack = true;
            isAttacking = false;
        }
    }

    private void HandleAttack()
    {
        if (!frameInput.Attack || !canAttack) { return; }
        if (isOnWall && !isGrounded)
        {
            animator.SetTrigger("isWallAttacking");
            GameObject kunaiObject = Instantiate(kunaiPrefab, wallKunaiSpawnPoint.position, Quaternion.identity);
            Kunai kunai = kunaiObject.GetComponent<Kunai>();
            kunai.Launch("Player", -transform.localScale.x, kunaiSpeed);
        }
        else
        {
            animator.SetTrigger("isAttacking");
            GameObject kunaiObject = Instantiate(kunaiPrefab, kunaiSpawnPoint.position, Quaternion.identity);
            Kunai kunai = kunaiObject.GetComponent<Kunai>();
            kunai.Launch("Player", transform.localScale.x, kunaiSpeed);
        }
        canAttack = false;
        isAttacking = true;
        attackTimer = timeToAttackAgain;
    }
    #endregion

    private void FlipSprite()
    {
        Vector3 localScale = transform.localScale;
        if (frameInput.Move.x < 0)
        {
            localScale.x = -1f;
        }
        else if (frameInput.Move.x > 0)
        {
            localScale.x = 1f;
        }
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
