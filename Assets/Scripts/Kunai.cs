using UnityEngine;

public class Kunai : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float timeUntilDestroy = 2f;

    private float destroyTimer;

    private Rigidbody2D rigidBody;
    private PlayerController player;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Start()
    {
        Vector3 localScale = transform.localScale;
        float direction;
        if (!player.isGrounded && player.isOnWall)
        {
            direction = -player.transform.localScale.x;
        }
        else
        {
            direction = player.transform.localScale.x;
        }
        localScale.x = direction;
        transform.localScale = localScale;
        rigidBody.linearVelocityX = moveSpeed * direction;
        destroyTimer = timeUntilDestroy;
    }

    private void Update()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        rigidBody.linearVelocityX = 0f;
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.EnableRagDoll();
            }
        }
    }

}
