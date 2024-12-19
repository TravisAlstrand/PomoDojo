using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Taco : MonoBehaviour
{
    [SerializeField] private Image tacoImage;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private ParticleSystem tacoParticles;

    private Animator animator;
    private EndTacoTrigger endTacoTrigger;

    private void Awake() {
        animator = GetComponent<Animator>();
        endTacoTrigger = FindFirstObjectByType<EndTacoTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().hasTaco = true;
            tacoImage.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void TacoFinisher() {
        tacoImage.gameObject.SetActive(false);
        gameObject.SetActive(true);
        transform.position = respawnPoint.position;
        animator.Play("FinSpin");
        StartCoroutine(WaitToDestroy());
    }

    private IEnumerator WaitToDestroy() {
        yield return new WaitForSeconds(3f);
        Instantiate(tacoParticles, respawnPoint);
        endTacoTrigger.StartTrainingEnd();
        Destroy(gameObject);
    }
}
