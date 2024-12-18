using UnityEngine;
using UnityEngine.UI;

public class Taco : MonoBehaviour
{
    [SerializeField] private Image tacoImage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().hasTaco = true;
            tacoImage.gameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}
