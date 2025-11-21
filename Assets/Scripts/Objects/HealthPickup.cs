using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20; // Cuánta vida recupera

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Necesitamos una función en GameManager para curar al jugador
            GameManager.Instance.HealPlayer(healAmount);
            Destroy(gameObject);
        }
    }
}