using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value = 10; // Cuántas monedas te da

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(value);
            // Aquí podrías poner un sonido: SoundManager.Instance.PlayCoinSound();
            Destroy(gameObject);
        }
    }
}