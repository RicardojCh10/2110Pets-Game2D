using UnityEngine;
using System.Collections; // Necesario para usar Coroutines (IEnumerator)

public class PlayerHealth : MonoBehaviour
{
    // Puedes ajustar cuánto daño te hace un enemigo desde el Inspector
    public int damageFromEnemy = 35;

    // --- VARIABLES DE CONFIGURACIÓN ---
    [Header("Efectos de Daño Visual")]
    public float flashDuration = 0.1f;    // Cuánto tiempo dura el destello rojo
    public Color flashColor = Color.red;  // El color del destello
    public float shakeMagnitude = 0.1f;   // Qué tan fuerte vibrará
    public float shakeDuration = 0.2f;    // Cuánto tiempo vibrará

    // --- REFERENCIAS ---
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer de Aiden
    private Color originalColor;          // Para guardar el color original del sprite
    
    // NOTA: Hemos eliminado 'private Vector3 originalPosition;'

    private void Start()
    {
        // Obtener y guardar componentes
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; 
        }
        // NOTA: Ya no guardamos la posición original aquí.
    }

    // Esta función se llama automáticamente cuando este collider choca con otro
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobamos si el objeto con el que chocamos tiene el tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("¡El jugador tocó un enemigo!");
            GameManager.Instance.TakePlayerDamage(damageFromEnemy);
            StartCoroutine(DamageEffect());
        }
    }
    
    // ----------------------------------------------------
    // NUEVA FUNCIÓN: Maneja el destello y la vibración
    // ----------------------------------------------------
    public IEnumerator DamageEffect()
    {
        // Guardamos la posición actual justo antes de empezar la vibración
        Vector3 currentPosition = transform.position; 

        // --- 1. Efecto de Destello Rojo ---
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor; // Cambia el sprite a rojo
        }

        // --- 2. Efecto de Vibración (Sacudida) ---
        float timer = 0f;
        while (timer < shakeDuration)
        {
            // Genera una posición aleatoria (offset) basada en la magnitud
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            
            // USAMOS LA POSICIÓN ACTUAL GUARDADA + EL OFFSET
            transform.position = currentPosition + new Vector3(offsetX, offsetY, 0);

            timer += Time.deltaTime;
            yield return null; // Espera un frame
        }
        
        // Vuelve a la posición exacta donde estaba cuando la coroutine inició
        transform.position = currentPosition; 

        // 3. Espera el tiempo de destello para mantener el color rojo
        yield return new WaitForSeconds(flashDuration);

        // 4. Vuelve al color original
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}