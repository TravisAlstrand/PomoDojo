using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    private Animator animator;

    private void Awake()
    {
        animator = fadeImage.GetComponent<Animator>();
    }

    private void Start()
    {
        FadeImageOut();
    }

    public void FadeImageOut()
    {
        animator.Play("FadeIn");
    }

    public void FadeImageIn()
    {
        animator.Play("FadeOut");
        StartCoroutine(WaitToLoadNextScene());
    }

    private IEnumerator WaitToLoadNextScene()
    {
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
