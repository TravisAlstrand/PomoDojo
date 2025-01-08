using UnityEngine;

public class DataPersister : MonoBehaviour
{
    private float bestTraining1, bestTraining2, bestTraining3;

    public static DataPersister Instance { get; private set; }
    private MenuManager menuManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData(); // Load saved data when this instance is created
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadData()
    {
        bestTraining1 = PlayerPrefs.GetFloat("Training1Best", 0f);
        bestTraining2 = PlayerPrefs.GetFloat("Training2Best", 0f);
        bestTraining3 = PlayerPrefs.GetFloat("Training3Best", 0f);
    }

    public void SetTrainingBest(int trainingNumber, float time)
    {
        switch (trainingNumber)
        {
            case 1:
                if (bestTraining1 == 0 || time < bestTraining1)
                {
                    bestTraining1 = time;
                    PlayerPrefs.SetFloat("Training1Best", time);
                }
                break;
            case 2:
                if (bestTraining2 == 0 || time < bestTraining2)
                {
                    bestTraining2 = time;
                    PlayerPrefs.SetFloat("Training2Best", time);
                }
                break;
            case 3:
                if (bestTraining3 == 0 || time < bestTraining3)
                {
                    bestTraining3 = time;
                    PlayerPrefs.SetFloat("Training3Best", time);
                }
                break;
        }
        PlayerPrefs.Save(); // Ensure changes are written to disk
    }

    public float GetTrainingBest(int trainingNumber)
    {
        switch (trainingNumber)
        {
            case 1:
                return bestTraining1;
            case 2:
                return bestTraining2;
            case 3:
                return bestTraining3;
            default:
                return 0f;
        }
    }

    public void ClearSaveData() {
        PlayerPrefs.SetFloat("Training1Best", 0f);
        PlayerPrefs.SetFloat("Training2Best", 0f);
        PlayerPrefs.SetFloat("Training3Best", 0f);
        PlayerPrefs.Save();
        // Reload in-memory data to match PlayerPrefs
        LoadData();
        menuManager = FindFirstObjectByType<MenuManager>();
        menuManager.InitializeMenu();
    }
}
