using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int damageFromEnemy = 35;

    [Header("Efectos de Daño Visual")]
    public float flashDuration = 0.1f;    
    public Color flashColor = Color.red;  
    public float shakeMagnitude = 0.1f;   
    public float shakeDuration = 0.2f;    

    [Header("Invulnerabilidad y Parpadeo")]
    public float invulnerabilityTime = 1.5f; 
    public float blinkInterval = 0.1f;       
    private bool isInvulnerable = false;    

    public AudioSource audioSource; 
    public AudioClip damageSound;  
    
    
    private SpriteRenderer spriteRenderer; 
    private Color originalColor;          
    
    private Coroutine blinkCoroutine; 
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

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvulnerable) return; 

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("¡El jugador tocó un enemigo!");

            PlaySound(damageSound);

            InitiateDamageSequence();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvulnerable) return; 
        
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log("¡Aiden fue golpeado por una bala!");
            ReceiveDamage(damageFromEnemy); 
            PlaySound(damageSound);
            
            InitiateDamageSequence();
            Destroy(other.gameObject); 
        }
    }

    private void InitiateDamageSequence()
    {
        if (damageSequenceCoroutine != null)
        {
            StopCoroutine(damageSequenceCoroutine);
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            if (spriteRenderer != null) spriteRenderer.color = originalColor;
            isInvulnerable = false;
        }

        damageSequenceCoroutine = StartCoroutine(DamageSequence());
    }



    private IEnumerator DamageSequence()
    {
        isInvulnerable = true;

        yield return StartCoroutine(HandleDamageEffect()); 

        blinkCoroutine = StartCoroutine(HandleInvulnerabilityBlink());
        
        float remainingInvulnTime = invulnerabilityTime; 
        remainingInvulnTime -= shakeDuration;
        remainingInvulnTime -= flashDuration;
        
        if (remainingInvulnTime < 0) remainingInvulnTime = 0;

        if (remainingInvulnTime > 0)
        {
             yield return new WaitForSeconds(remainingInvulnTime);
        }

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        isInvulnerable = false;
        damageSequenceCoroutine = null; 
    }

    public IEnumerator HandleDamageEffect()
    {
        Vector3 currentPosition = transform.position; 
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
        }

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
        
        yield return new WaitForSeconds(flashDuration);
    }

    private IEnumerator HandleInvulnerabilityBlink()
    {
        while(true)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white; 
            }
            yield return new WaitForSeconds(blinkInterval);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f); 
            }
            yield return new WaitForSeconds(blinkInterval);
        }
    }

public void ReceiveDamage(int damageAmount)
{
    if (isInvulnerable) return; 
    
    GameManager.Instance.TakePlayerDamage(damageAmount);

    PlaySound(damageSound);

    InitiateDamageSequence();
}

}

