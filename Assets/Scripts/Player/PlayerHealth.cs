using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // Puedes ajustar cuánto daño te hace un enemigo desde el Inspector
    public int damageFromEnemy = 35;

    // --- VARIABLES DE CONFIGURACIÓN ---
    [Header("Efectos de Daño Visual")]
    public float flashDuration = 0.1f;    
    public Color flashColor = Color.red;  
    public float shakeMagnitude = 0.1f;   
    public float shakeDuration = 0.2f;    

    // --- Invulnerabilidad y Parpadeo ---
    [Header("Invulnerabilidad y Parpadeo")]
    public float invulnerabilityTime = 1.5f; 
    public float blinkInterval = 0.1f;       
    private bool isInvulnerable = false;    

    // Variables de Audio para Daño
    public AudioSource audioSource; 
    public AudioClip damageSound;  
    
    
    // --- REFERENCIAS ---
    private SpriteRenderer spriteRenderer; 
    private Color originalColor;          
    
    private Coroutine blinkCoroutine; 
    // NUEVO: Referencia a la Coroutine principal de daño
    private Coroutine damageSequenceCoroutine; 


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; 
        }
        audioSource = GetComponent<AudioSource>();
    }

    // Metodo para reproducir el sonido de daño
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // --- Detección de Colisión Cuerpo a Cuerpo ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si ya estamos invulnerables, ignoramos el golpe
        if (isInvulnerable) return; 

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("¡El jugador tocó un enemigo!");
            GameManager.Instance.TakePlayerDamage(damageFromEnemy);

            // Reproducir el sonido de daño
            PlaySound(damageSound);

            // Iniciar o reiniciar la Secuencia Principal de Daño
            InitiateDamageSequence();
        }
    }

    // --- Detección de Colisión con Balas ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si ya estamos invulnerables, ignoramos el golpe
        if (isInvulnerable) return; 
        
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log("¡Aiden fue golpeado por una bala!");
            GameManager.Instance.TakePlayerDamage(damageFromEnemy);

            // Reproducir el sonido de daño
            PlaySound(damageSound);
            
            // Iniciar o reiniciar la Secuencia Principal de Daño
            InitiateDamageSequence();
            Destroy(other.gameObject); 
        }
    }

    // ----------------------------------------------------
    // NUEVO MÉTODO: Centraliza el inicio de la secuencia de daño
    // ----------------------------------------------------
    private void InitiateDamageSequence()
    {
        // Si ya hay una secuencia de daño en curso, la detenemos y la reiniciamos.
        // Esto asegura que cada golpe inicia una nueva animación y periodo de invulnerabilidad.
        if (damageSequenceCoroutine != null)
        {
            StopCoroutine(damageSequenceCoroutine);
            // También detener el parpadeo y resetear color si se detiene a mitad
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            if (spriteRenderer != null) spriteRenderer.color = originalColor;
            isInvulnerable = false; // Asegurar que el estado se reinicie
        }

        // Iniciar la nueva secuencia de daño y guardar su referencia
        damageSequenceCoroutine = StartCoroutine(DamageSequence());
    }


    // ----------------------------------------------------
    // COROUTINE PRINCIPAL: Secuencia de Daño y Invulnerabilidad
    // ----------------------------------------------------
    private IEnumerator DamageSequence()
    {
        // 1. Marcar como invulnerable
        isInvulnerable = true;

        // 2. Iniciar el efecto de Sacudida y Destello Rojo (efecto instantáneo)
        // Esta coroutine espera a que HandleDamageEffect termine
        yield return StartCoroutine(HandleDamageEffect()); 

        // 3. Iniciar el efecto de Parpadeo y guardamos la referencia de la Coroutine
        blinkCoroutine = StartCoroutine(HandleInvulnerabilityBlink());
        
        // 4. Esperar el tiempo restante de invulnerabilidad
        // Restamos la duración de los efectos iniciales para que el total de invulnerabilidad sea preciso
        float remainingInvulnTime = invulnerabilityTime; // Empezamos con el total
        remainingInvulnTime -= shakeDuration; // Restamos la sacudida
        remainingInvulnTime -= flashDuration; // Restamos el destello rojo
        
        // Asegurarse de que no esperamos un tiempo negativo
        if (remainingInvulnTime < 0) remainingInvulnTime = 0;

        if (remainingInvulnTime > 0)
        {
             yield return new WaitForSeconds(remainingInvulnTime);
        }

        // 5. Finalizar la invulnerabilidad y restablecer el estado visual
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine); // Detener el parpadeo usando la referencia
        }
        
        // Asegurarse de que el color final sea el original
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        // 6. Quitar la invulnerabilidad
        isInvulnerable = false;
        damageSequenceCoroutine = null; // Liberar la referencia de la coroutine principal
    }


    // ----------------------------------------------------
    // COROUTINE A: Maneja la Sacudida y el Destello Rojo
    // ----------------------------------------------------
    public IEnumerator HandleDamageEffect()
    {
        Vector3 currentPosition = transform.position; 
        
        // Efecto de Destello Rojo: Tinte
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
        }

        // Efecto de Vibración (Sacudida)
        float timer = 0f;
        while (timer < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            
            transform.position = currentPosition + new Vector3(offsetX, offsetY, 0);

            timer += Time.deltaTime;
            yield return null; 
        }
        
        transform.position = currentPosition; 
        
        // Espera solo la duración del destello rojo (mientras el sprite ya está rojo)
        yield return new WaitForSeconds(flashDuration);
    }

    // ----------------------------------------------------
    // COROUTINE B: Maneja el Parpadeo Blanco/Transparente
    // ----------------------------------------------------
    private IEnumerator HandleInvulnerabilityBlink()
    {
        while(true)
        {
            // Sprite visible (blanco, para el parpadeo)
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white; 
            }
            yield return new WaitForSeconds(blinkInterval);

            // Sprite invisible (transparente total)
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // Transparente
            }
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}