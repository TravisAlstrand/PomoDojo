using UnityEngine;

public class EndTacoTrigger : MonoBehaviour
{
    private PlayerController player;
    private Timer timer;
    private Taco taco;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        timer = FindFirstObjectByType<Timer>();
        taco = FindFirstObjectByType<Taco>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && player.hasTaco)
        {
            timer.PauseTimer();
            timer.TimerTextGreen();
            taco.DisableTacoCanvasImage();
            Debug.Log("Taco Received!");
        }
    }
}
