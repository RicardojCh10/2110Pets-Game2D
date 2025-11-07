using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Puedes ajustar cuánto daño te hace un enemigo desde el Inspector
    public int damageFromEnemy = 35;

    // Esta función se llama automáticamente cuando este collider choca con otro
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobamos si el objeto con el que chocamos tiene el tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Imprimimos un mensaje en la consola para saber que funcionó
            Debug.Log("¡El jugador tocó un enemigo!");

            // Llamamos a nuestro GameManager (que es un Singleton)
            // y le pedimos que nos reste vida.
            GameManager.Instance.TakePlayerDamage(damageFromEnemy);
        }
    }
}