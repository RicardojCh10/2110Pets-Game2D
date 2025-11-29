using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    private bool levelCompleted = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !levelCompleted)
        {
            levelCompleted = true;
            Debug.Log("¡META ALCANZADA!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel();
            }
        }
    }
}