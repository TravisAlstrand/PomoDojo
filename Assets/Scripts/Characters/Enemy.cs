using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Moving")]
    [SerializeField] private Transform patrolPointA;
    [SerializeField] private Transform patrolPointB;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float idleTime = 3f;
    [Header("Player Detection")]
    [SerializeField] private GameObject exclamationIcon;
    [SerializeField] private float waitForFirstAttack = .5f;
    [SerializeField] private Transform detectionOrigin;
    [SerializeField] private Vector2 detectionBoxSize = new Vector2(20f, 2f);
    [SerializeField] private LayerMask detectionLayer;
    [Header("Attack")]
    [SerializeField] private GameObject kunaiPrefab;
    [SerializeField] private Transform kunaiSpawnPoint;
    [SerializeField] private float kunaiSpeed = 20f;
    [SerializeField] private float attackAgainTime = 1f;
    [Header("RagDoll")]
    [SerializeField] private Rigidbody2D[] boneRigidBodies;
    [SerializeField] private Collider2D[] boneColliders;

    private Vector3 currentPatrolTarget;
    private float idleTimer, attackTimer;
    private bool canAttack = true;
    private enum EnemyState { Patrolling, Idling, Detecting, Attacking, Dead }
    private EnemyState currentState;
    private bool detectsPlayer;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private Vector3 playerPosition;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentPatrolTarget = patrolPointA.position;
        DisableRagDoll();
    }

    private void Update()
    {
        if (currentState != EnemyState.Dead)
        {
            DetectPlayer();
            if (detectsPlayer)
            {
                exclamationIcon.SetActive(true);
            }
            else
            {
                exclamationIcon.SetActive(false);
            }

            switch (currentState)
            {
                case EnemyState.Patrolling:
                    PatrolBehavior();
                    break;
                case EnemyState.Idling:
                    IdleBehavior();
                    break;
                case EnemyState.Detecting:
                    // Detecting behavior is handled by the coroutine
                    break;
                case EnemyState.Attacking:
                    AttackBehavior();
                    break;
            }

            FlipSprite();
        }
        else
        {
            exclamationIcon.SetActive(false);
        }
    }

    private void DetectPlayer()
    {
        playerPosition = FindFirstObjectByType<PlayerController>().transform.position;
        Collider2D hit = Physics2D.OverlapBox(detectionOrigin.position, detectionBoxSize, 0f, detectionLayer);

        if (hit != null && hit.CompareTag("Player"))
        {
            if (!detectsPlayer)
            {
                detectsPlayer = true;
                StartCoroutine(HandleDetection());
            }
        }
        else
        {
            detectsPlayer = false;
            if (currentState == EnemyState.Detecting)
            {
                StopCoroutine(HandleDetection());
                currentState = EnemyState.Patrolling; // Return to patrolling if the player escapes
            }
        }
    }

    private IEnumerator HandleDetection()
    {
        currentState = EnemyState.Detecting;

        // Delay before attacking
        yield return new WaitForSeconds(waitForFirstAttack);

        if (detectsPlayer)
        {
            currentState = EnemyState.Attacking;
        }
    }

    private void PatrolBehavior()
    {
        Vector2 direction = (currentPatrolTarget - transform.position).normalized;
        rigidBody.linearVelocity = direction * moveSpeed;

        animator.Play("Running");
        animator.Play("ArmRunning");

        if (Vector2.Distance(transform.position, currentPatrolTarget) < 0.1f)
        {
            rigidBody.linearVelocity = Vector2.zero;
            currentState = EnemyState.Idling;
            StartIdling();
        }
    }

    private void IdleBehavior()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            EndIdling();
        }
    }

    private void AttackBehavior()
    {
        if (!detectsPlayer)
        {
            currentState = EnemyState.Idling;
            StartIdling();
        }

        rigidBody.linearVelocity = Vector2.zero;
        animator.Play("Idle");
        if (canAttack)
        {
            animator.Play("Attack");
            ThrowKunai();
            attackTimer = attackAgainTime;
            canAttack = false;
        }
        else
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                canAttack = true;
            }
        }
    }

    private void ThrowKunai()
    {
        // Calculate the direction to the player
        Vector2 direction = (new Vector2(playerPosition.x, playerPosition.y) - (Vector2)transform.position).normalized;
        // Ensure direction.x is either 1 or -1
        float horizontalDirection = Mathf.Sign(direction.x);
        GameObject kunaiObject = Instantiate(kunaiPrefab, kunaiSpawnPoint.position, Quaternion.identity);
        Kunai kunai = kunaiObject.GetComponent<Kunai>();
        kunai.Launch("Enemy", horizontalDirection, kunaiSpeed);
    }

    private void StartIdling()
    {
        animator.Play("Idle");
        animator.Play("ArmIdle");
        idleTimer = idleTime;
    }

    private void EndIdling()
    {
        currentState = EnemyState.Patrolling;
        currentPatrolTarget = currentPatrolTarget == patrolPointA.position ? patrolPointB.position : patrolPointA.position;
    }

    private void FlipSprite()
    {
        Vector3 localScale = transform.localScale;
        if (rigidBody.linearVelocity.x < 0)
        {
            localScale.x = -1f;
        }
        else if (rigidBody.linearVelocity.x > 0)
        {
            localScale.x = 1f;
        }
        transform.localScale = localScale;
    }

    public void EnableRagDoll()
    {
        currentState = EnemyState.Dead;
        animator.enabled = false;

        foreach (var rb in boneRigidBodies)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        foreach (var collider in boneColliders)
        {
            collider.enabled = true;
        }

        GetComponent<Collider2D>().enabled = false;
        rigidBody.linearVelocity = Vector2.zero;

        Destroy(gameObject, 5f);
    }

    private void DisableRagDoll()
    {
        animator.enabled = true;

        foreach (var rb in boneRigidBodies)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        foreach (var collider in boneColliders)
        {
            collider.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(detectionOrigin.position, detectionBoxSize);

        if (patrolPointA != null && patrolPointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(patrolPointA.position, patrolPointB.position);
        }
    }
}
