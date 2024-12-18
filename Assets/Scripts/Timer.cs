using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private float startTime = 60f;
    [SerializeField] private TextMeshProUGUI timerText;

    private bool timerActive;
    private float timer;

    private void Update()
    {
        if (!timerActive) { return; }

        timer -= Time.deltaTime;
        UpdateTimer();

        if (timer <= 0f)
        {
            timer = 0f;
            timerActive = false;
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

    private void UpdateTimer()
    {
        timerText.text = timer.ToString("F2");
    }

    public void RemoveTimer()
    {
        timerActive = false;
        timerText.gameObject.SetActive(false);
    }
}
