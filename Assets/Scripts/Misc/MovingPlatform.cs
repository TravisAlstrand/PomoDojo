using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // [SerializeField] private GameObject platform;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 5f;

    private Transform currentTarget;

    // private Rigidbody2D platformRB;
    private Rigidbody2D rigidBody;

    private void Awake()
    {
        // platformRB = platform.GetComponent<Rigidbody2D>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentTarget = pointA;
    }

    private void FixedUpdate()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        Vector2 newPosition = Vector2.MoveTowards(rigidBody.position, currentTarget.position, moveSpeed * Time.fixedDeltaTime);
        rigidBody.MovePosition(newPosition);

        if (Vector2.Distance(rigidBody.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.parent = null;
        }
    }
}
