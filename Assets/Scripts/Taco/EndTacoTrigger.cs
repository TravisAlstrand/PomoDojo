using System.Collections;
using UnityEngine;

public class EndTacoTrigger : MonoBehaviour
{
    [SerializeField] private GameObject finalDialogue;

    private PlayerController player;
    private Timer timer;
    private Taco taco;
    private FadeController fadeController;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        timer = FindFirstObjectByType<Timer>();
        taco = FindFirstObjectByType<Taco>();
        fadeController = FindFirstObjectByType<FadeController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && player.hasTaco)
        {
            timer.PauseTimer();
            timer.TimerTextGreen();
            taco.TacoFinisher();
            player.DisableMovement();
        }
    }

    public void StartTrainingEnd() {
        finalDialogue.SetActive(true);
        StartCoroutine(WaitToEndLevel());
    }

    private IEnumerator WaitToEndLevel() {
        yield return new WaitForSeconds(7f);
        fadeController.FadeImageIn();
    }
}
