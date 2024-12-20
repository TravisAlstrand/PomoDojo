using UnityEngine;

public class DataPersister : MonoBehaviour
{
    public void SetTrainingBest(int trainingNumber, float time) {
        if (trainingNumber !< 1 && trainingNumber !> 3) {
            PlayerPrefs.SetFloat($"Training{trainingNumber}Best", time);
        }
    }

    public float GetTrainingBest(int trainingNumber) {
        if (trainingNumber !< 1 && trainingNumber !> 3) {
            return PlayerPrefs.GetFloat($"Training{trainingNumber}Best");
        }
        else {
            return 0f;
        }
    }
} 
