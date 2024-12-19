using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint1;
    [SerializeField] private Transform respawnPoint2;

    private PlayerController playerController;
    private GameObject player;
    private Timer timer;

    private void Awake() {
        playerController = FindFirstObjectByType<PlayerController>();
        player = playerController.gameObject;
        timer = FindFirstObjectByType<Timer>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == player) {
            timer.PauseTimer();
            timer.TimerTextRed();
            playerController.JustRespawned();
            if (!playerController.hasTaco) {
                player.transform.position = respawnPoint1.position;
            }
            else {
                player.transform.position = respawnPoint2.position;
            }

        }
    }
}
