using System.Collections;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class Timer : MonoBehaviour
{
    [SerializeField] private float startTime = 60f;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Transform startRespawnPoint;
    [SerializeField] private GameObject tryAgainText;

    private bool timerActive;
    private float timer;

    private PlayerController player;
    private Taco taco;

    private void Awake() {
        player = FindFirstObjectByType<PlayerController>();
        taco = FindFirstObjectByType<Taco>();
    }

    private void Update()
    {
        if (!timerActive) { return; }

        timer -= Time.deltaTime;
        UpdateTimer();

        if (timer <= 0f)
        {
            timer = 0f;
            TimerTextRed();
            timerActive = false;
            RestartTraining();
        }
    }

    public void StartTimer()
    {
        timer = startTime;
        UpdateTimer();
        timerText.gameObject.SetActive(true);
        timerActive = true;
    }

    public void PauseTimer()
    {
        timerActive = false;
    }

    public void UnPauseTimer()
    {
        timerActive = true;
    }

    public void RemoveTimer()
    {
        timerActive = false;
        timerText.gameObject.SetActive(false);
    }

    public void TimerTextGreen() {
        timerText.color = Color.green;
    }
    
    public void TimerTextRed() {
        timerText.color = Color.red;
    }

    public void TimerTextWhite() {
        timerText.color = Color.white;
    }

    private void UpdateTimer()
    {
        timerText.text = timer.ToString("F2");
    }

    private void RestartTraining() {
        player.DisableMovement();
        player.transform.localScale = Vector2.one;
        player.hasTaco = false;
        if (taco) {
            taco.ResetTaco();
        }
        if (startRespawnPoint) {
            player.gameObject.transform.position = startRespawnPoint.position;
        }
        timer = startTime;
        tryAgainText.SetActive(true);
        StartCoroutine(WaitToRestart());
    }

    private IEnumerator WaitToRestart() {
        yield return new WaitForSeconds(2f);
        player.EnableMovement();
        tryAgainText.SetActive(false);
        TimerTextWhite();
        timerActive = true;
    }

    public float GetTimeTaken() {
        return startTime - timer;
    }
}
