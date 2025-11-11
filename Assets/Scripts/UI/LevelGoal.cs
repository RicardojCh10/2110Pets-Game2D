// --- LevelGoal.cs ---
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    private bool levelCompleted = false; // Para evitar que se active varias veces

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Comprueba si el objeto que entró es el jugador
        //    (¡Asegúrate de que Aiden tenga el Tag "Player"!)
        if (other.CompareTag("Player") && !levelCompleted)
        {
            levelCompleted = true; // Marca como completado
            Debug.Log("¡META ALCANZADA!");

            // 2. Llama al GameManager para que se encargue de todo
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel();
            }
        }
    }
}