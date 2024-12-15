using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform patrolPointA;
    [SerializeField] private Transform patrolPointB;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float idleTime = 3f;

    private Vector3 currentPatrolTarget;
    private bool isIdling = false;
    private float idleTimer;

    private Rigidbody2D rigidBody;
    private Animator animator;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentPatrolTarget = patrolPointA.position;
    }

    private void Update()
    {
        if (isIdling)
        {
            IdleBehavior();
        }
        else
        {
            PatrolBehavior();
        }

        FlipSprite();
    }

    private void PatrolBehavior()
    {
        // Move towards the current patrol target
        Vector2 direction = (currentPatrolTarget - transform.position).normalized;
        rigidBody.linearVelocity = direction * moveSpeed;

        animator.Play("Running");
        animator.Play("ArmRunning");

        // Check if the enemy has reached the patrol target
        if (Vector2.Distance(transform.position, currentPatrolTarget) < 0.1f)
        {
            rigidBody.linearVelocity = Vector2.zero; // Stop moving
            StartIdling(); // Start idling
        }
    }

    private void IdleBehavior()
    {
        // Count down the idle timer
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            EndIdling(); // Stop idling and switch patrol target
        }
    }

    private void StartIdling()
    {
        animator.Play("Idle");
        animator.Play("ArmIdle");
        isIdling = true;
        idleTimer = idleTime;
    }

    private void EndIdling()
    {
        isIdling = false;
        // Switch to the next patrol point
        if (currentPatrolTarget == patrolPointA.position)
        {
            currentPatrolTarget = patrolPointB.position;
        }
        else
        {
            currentPatrolTarget = patrolPointA.position;
        }
    }

    private void FlipSprite()
    {
        Vector3 localScale = transform.localScale;
        if (rigidBody.linearVelocityX < 0)
        {
            localScale.x = -1f;
        }
        else if (rigidBody.linearVelocityX > 0)
        {
            localScale.x = 1f;
        }
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        // Draw gizmos to visualize patrol points
        if (patrolPointA != null && patrolPointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(patrolPointA.position, patrolPointB.position);
        }
    }
}
