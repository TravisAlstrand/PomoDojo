using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private int currentSceneIndex;

    private void Start() {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene(0);
    }

    public void LoadLevel(int index) {
        SceneManager.LoadScene(index);
    }

    public void ReloadCurrentScene() {
        SceneManager.LoadScene(currentSceneIndex);
    }

    public string GetCurrentScene() {
        return SceneManager.GetActiveScene().name;
    }
}
