using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 5f;

    private Transform currentTarget;

    private Rigidbody2D platformRB;

    private void Awake()
    {
        platformRB = platform.GetComponent<Rigidbody2D>();
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
        Vector2 newPosition = Vector2.MoveTowards(platformRB.position, currentTarget.position, moveSpeed * Time.fixedDeltaTime);
        platformRB.MovePosition(newPosition);

        if (Vector2.Distance(platformRB.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }
}
