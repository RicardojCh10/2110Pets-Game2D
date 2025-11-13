using UnityEngine;

public class AcidPit : MonoBehaviour
{
    // Esta función se llama automáticamente cuando algo entra en el trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Comprueba si lo que tocó el ácido tiene el Tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Aiden cayó en el ácido!");

            // 2. Busca el GameManager y le dice que mate al jugador
            if (GameManager.Instance != null)
            {
                // Le damos un número de daño altísimo para asegurar la muerte
                GameManager.Instance.TakePlayerDamage(9999);
            }
        }
    }
}