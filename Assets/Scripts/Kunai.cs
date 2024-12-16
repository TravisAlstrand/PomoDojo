using UnityEngine;

public class Kunai : MonoBehaviour
{
    private string kunaiThrower;
    private readonly float timeUntilDestroy = 2f;
    private float destroyTimer;

    private Rigidbody2D rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
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

    public void Launch(string thrower, float direction, float speed)
    {
        kunaiThrower = thrower;
        Vector3 localScale = transform.localScale;
        localScale.x = direction;
        transform.localScale = localScale;
        rigidBody.linearVelocityX = speed * direction;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        rigidBody.linearVelocityX = 0f;
        if (other.gameObject.CompareTag("Enemy") && kunaiThrower == "Player")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.EnableRagDoll();
            }
        }
        rigidBody.gravityScale = 1f;
    }

}
