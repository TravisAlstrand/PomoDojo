using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint1;
    [SerializeField] private Transform respawnPoint2;

    private PlayerController playerController;
    private GameObject player;

    private void Awake() {
        playerController = FindFirstObjectByType<PlayerController>();
        player = playerController.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == player) {
            if (!playerController.hasTaco) {
                player.transform.position = respawnPoint1.position;
            }
            else {
                player.transform.position = respawnPoint2.position;
            }
        }
    }
}
