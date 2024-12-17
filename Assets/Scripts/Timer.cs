using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private float startTime = 60f;
    [SerializeField] private TextMeshProUGUI timerText;

    private bool timerStarted;
    private float timer;

    private void Update()
    {
        if (!timerStarted) { return; }

        timer -= Time.deltaTime;
        UpdateTimer();

        if (timer <= 0f)
        {
            timer = 0f;
            timerStarted = false;
        }
    }

    public void StartTimer()
    {
        timer = startTime;
        UpdateTimer();
        timerText.gameObject.SetActive(true);
        timerStarted = true;
    }

    private void UpdateTimer()
    {
        timerText.text = timer.ToString("F2");
    }
}
