using UnityEngine;

public class AcidPit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Aiden cayó en el ácido!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakePlayerDamage(9999);
            }
        }
    }
}