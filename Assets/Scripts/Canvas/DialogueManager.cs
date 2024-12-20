using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nextLineHintText;
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private Image senseiArrow;
    [SerializeField] private Image playerArrow;
    [SerializeField] private float timeToWait = 2f;
    [SerializeField] private bool loadNextSceneAfter = true;
    [SerializeField] private bool loadMainMenuAfter = false;
    [SerializeField] private bool startTimerAfter = false;

    private int currentLineIndex = 0;

    private PlayerInput playerInput;
    private FadeController fadeController;
    private Timer timer;

    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        fadeController = FindFirstObjectByType<FadeController>();
        timer = FindFirstObjectByType<Timer>();
    }

    private void Start()
    {
        playerInput.SwitchToUIMap();
        StartCoroutine(WaitToStartDialogue());
    }

    private void Update()
    {
        if (dialogueBox.activeInHierarchy)
        {
            if (playerInput.FrameInput.SubmitRelease)
            {
                currentLineIndex++;
            }

            if (currentLineIndex >= dialogueLines.Length)
            {
                dialogueBox.SetActive(false);
                dialogueText.gameObject.SetActive(false);
                nextLineHintText.gameObject.SetActive(false);
                senseiArrow.gameObject.SetActive(false);
                playerArrow.gameObject.SetActive(false);
                if (loadNextSceneAfter)
                {
                    StartCoroutine(WaitToStartNextScene());
                }
                else if (loadMainMenuAfter) {
                    StartCoroutine(WaitToLoadMenu());
                }
                else if (startTimerAfter)
                {
                    playerInput.SwitchToGameplayMap();
                    timer.StartTimer();
                }
                else
                {
                    playerInput.SwitchToGameplayMap();
                }
            }
            else
            {
                CheckIfName();
                dialogueText.text = dialogueLines[currentLineIndex];
            }
        }
    }

    private void ActivateDialogueBox()
    {
        dialogueText.text = "";
        dialogueBox.SetActive(true);
        CheckIfName();
        dialogueText.text = dialogueLines[currentLineIndex];
        dialogueText.gameObject.SetActive(true);
        nextLineHintText.gameObject.SetActive(true);
    }

    private void CheckIfName()
    {
        string line = dialogueLines[currentLineIndex];
        if (line.StartsWith("n-"))
        {
            // If line is a "n-[name]" line activate correct arrow
            if (line == "n-Sensei")
            {
                senseiArrow.gameObject.SetActive(true);
                playerArrow.gameObject.SetActive(false);
            }
            else if (line == "n-Player")
            {
                senseiArrow.gameObject.SetActive(false);
                playerArrow.gameObject.SetActive(true);
            }
            else
            {
                senseiArrow.gameObject.SetActive(false);
                playerArrow.gameObject.SetActive(false);
            }
            currentLineIndex++;
        }
    }

    private IEnumerator WaitToStartDialogue()
    {
        yield return new WaitForSeconds(timeToWait);
        ActivateDialogueBox();
    }

    private IEnumerator WaitToStartNextScene()
    {
        yield return new WaitForSeconds(timeToWait);
        playerInput.SwitchToGameplayMap();
        fadeController.FadeImageIn("next");
    }

    private IEnumerator WaitToLoadMenu() {
        yield return new WaitForSeconds(timeToWait);
        fadeController.FadeImageIn("menu");
    }
}
