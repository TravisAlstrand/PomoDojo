using System.Collections;
using UnityEngine;

public class EndTacoTrigger : MonoBehaviour
{
    [SerializeField] private GameObject finalDialogue;
    private float timeTaken;
    private float currentBestTime;

    private PlayerController player;
    private Timer timer;
    private Taco taco;
    private FadeController fadeController;
    private SceneController sceneController;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        timer = FindFirstObjectByType<Timer>();
        taco = FindFirstObjectByType<Taco>();
        fadeController = FindFirstObjectByType<FadeController>();
        sceneController = FindFirstObjectByType<SceneController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && player.hasTaco)
        {
            timer.PauseTimer();
            timer.TimerTextGreen();
            timeTaken = Mathf.Round(timer.GetTimeTaken() * 100f) / 100f;
            string currentSceneName = sceneController.GetCurrentScene();
            int currentTrainingNumber;
            if (currentSceneName.Contains("1")) {
                currentTrainingNumber = 1;
            } else if (currentSceneName.Contains("2")) {
                currentTrainingNumber = 2;
            } else {
                currentTrainingNumber = 3;
            }
            currentBestTime = DataPersister.Instance.GetTrainingBest(currentTrainingNumber);
            if (currentBestTime == 0 || timeTaken < currentBestTime) {
                DataPersister.Instance.SetTrainingBest(currentTrainingNumber, timeTaken);
            }
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
        fadeController.FadeImageIn("next");
    }
}
