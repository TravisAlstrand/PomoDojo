using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private GameObject[] redBricks;
    [SerializeField] private Vector2 brickForceMin;
    [SerializeField] private Vector2 brickForceMax;
    [SerializeField] private float timeBeforeDestroying = 2.25f;
    private bool hasBeenHit = false;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Kunai") && !hasBeenHit) {
            hasBeenHit = true;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.magenta;
            foreach (GameObject brick in redBricks)
            {
                Rigidbody2D brickRB = brick.GetComponent<Rigidbody2D>();
                Vector2 brickForce = new(Random.Range(brickForceMin.x, brickForceMax.x), Random.Range(brickForceMin.y, brickForceMax.y));
                brickRB.bodyType = RigidbodyType2D.Dynamic;
                brickRB.AddForce(brickForce);
            }
            StartCoroutine(WaitToDestroyBricks());
        }
    }

    private IEnumerator WaitToDestroyBricks() {
        yield return new WaitForSeconds(timeBeforeDestroying);
        foreach (GameObject brick in redBricks) {
            Destroy(brick);
        }
    }
}
