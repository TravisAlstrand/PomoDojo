using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float extraGravity = 700f;
    [SerializeField] private float gravityDelay = .2f;
    [SerializeField] private float coyoteTime = .1f;
    [Header("Ground")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    [Header("Walls")]
    [SerializeField] private Transform wallCheckPos;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private LayerMask wallLayer;
    [Header("Attack")]
    [SerializeField] private GameObject kunaiPrefab;
    [SerializeField] private Transform kunaiSpawnPoint;
    [SerializeField] private Transform wallKunaiSpawnPoint;
    [SerializeField] private float timeToAttackAgain = .5f;

    private bool stoppedJumpEarly, isAttacking, canAttack = true;
    private float timeInAir, coyoteTimer, attackTimer;

    public bool isGrounded, isOnWall;

    private Rigidbody2D rigidBody;
    private PlayerInput playerInput;
    private Animator animator;
    private FrameInput frameInput;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GatherInput();
        CheckCurrentState();
        CoyoteTimer();
        HandleJump();
        AttackTimer();
        HandleAttack();
        DidStopJumpEarly();
        GravityDelay();
        FlipSprite();
    }

    private void FixedUpdate()
    {
        Move();
        ApplyExtraGravity();
        // HandleWallSlide();
    }

    private void GatherInput()
    {
        frameInput = playerInput.FrameInput;
    }

    private void Move()
    {
        rigidBody.linearVelocityX = frameInput.Move.x * moveSpeed;
    }

    private void CheckCurrentState()
    {
        Collider2D groundCheck = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);
        Collider2D wallCheck = Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallLayer);
        isGrounded = groundCheck;
        isOnWall = wallCheck;

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
        }
        else if (!isGrounded && !isOnWall)
        {
            animator.Play("Jumping");
            if (!isAttacking)
            {
                animator.Play("ArmIdle");
            }
        }
        else if (!isGrounded && isOnWall)
        {
            animator.Play("WallSliding");
        }
    }

    #region Jump Methods
    private void HandleJump()
    {
        if (!frameInput.Jump) { return; }

        if (coyoteTimer > 0f || isGrounded)
        {
            ApplyJumpForce();
        }
    }

    private void ApplyJumpForce()
    {
        if (!frameInput.Jump) return;

        rigidBody.linearVelocity = Vector2.zero;
        timeInAir = 0f;
        coyoteTimer = 0f;
        stoppedJumpEarly = false;
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
            Instantiate(kunaiPrefab, wallKunaiSpawnPoint.position, Quaternion.identity);
        }
        else
        {
            animator.SetTrigger("isAttacking");
            Instantiate(kunaiPrefab, kunaiSpawnPoint.position, Quaternion.identity);
        }
        canAttack = false;
        isAttacking = true;
        attackTimer = timeToAttackAgain;
    }
    #endregion

    // private void HandleWallSlide()
    // {
    //     if (!isOnWall) { return; };

    //     if (frameInput.Move.x != 0f)
    //     {
    //         rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocityX, Mathf.Clamp(rigidBody.linearVelocityY, -wallSlideSpeed, float.MaxValue));
    //     }
    // }

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
