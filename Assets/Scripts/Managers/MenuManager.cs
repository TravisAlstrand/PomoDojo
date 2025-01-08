using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private Image[] pointers;
    [SerializeField] private TextMeshProUGUI[] bestTexts;

    private float[] bestTimes = new float[3];

    private PlayerInput playerInput;

    private void Awake() {
        playerInput = FindFirstObjectByType<PlayerInput>();
    }

    private void Start() {
        InitializeMenu();
    }

    private void Update() {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        for (int i = 0; i < buttons.Length; i++)
        {
            bool isSelected = currentSelected == buttons[i].gameObject;
            // Activate or deactivate the corresponding pointer
            pointers[i].gameObject.SetActive(isSelected);
        }
    }

    public void InitializeMenu() {
        // Deactivate all pointers initially
        foreach (Image pointer in pointers)
        {
            pointer.gameObject.SetActive(false);
        }

        playerInput.SwitchToUIMap();
        buttons[0].Select();
        GetBestTimes();
        SetBestTexts();
    }

    private void GetBestTimes() {
        for (int i = 0; i < 3; i++) {
            bestTimes[i] = DataPersister.Instance.GetTrainingBest(i+1);
        }
    }

    private void SetBestTexts() {
        for (int i = 0; i < bestTexts.Length; i++) {
            if (bestTimes[i] == 0f) {
                bestTexts[i].text = "Best Time: N/A";
                buttons[i + 1].interactable = false;
            } else {
                bestTexts[i].text = $"Best Time: {bestTimes[i]}";
            }
        }
    }
}
