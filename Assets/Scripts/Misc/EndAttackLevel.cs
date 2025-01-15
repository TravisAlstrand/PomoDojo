using UnityEngine;

public class EndAttackLevel : MonoBehaviour
{
    private PlayerController player;
    private FadeController fadeController;

    private void Awake() {
        player = FindFirstObjectByType<PlayerController>();
        fadeController = FindFirstObjectByType<FadeController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            EndLevel();
        }
    }

    private void EndLevel() {
        player.DisableMovement();
        fadeController.FadeImageIn("next");
    }
}
