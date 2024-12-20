using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    private float timeToWait = 1.2f;

    private Animator animator;
    private SceneController sceneController;

    private void Awake()
    {
        animator = fadeImage.GetComponent<Animator>();
        sceneController = FindFirstObjectByType<SceneController>();
    }

    private void Start()
    {
        FadeImageOut();
    }

    public void FadeImageOut()
    {
        animator.Play("FadeIn");
    }

    public void FadeImageIn(string sceneToLoad)
    {
        animator.Play("FadeOut");
        StartCoroutine(WaitToLoadNextScene(sceneToLoad));
    }

    private IEnumerator WaitToLoadNextScene(string sceneToLoad)
    {
        yield return new WaitForSeconds(timeToWait);
        if (sceneToLoad.ToLower() == "next") {
            sceneController.LoadNextScene();
        } else if (sceneToLoad.ToLower() == "menu") {
            sceneController.LoadMainMenu();
        } else if (sceneToLoad.ToLower() == "reload") {
            sceneController.ReloadCurrentScene();
        }
    }
}
