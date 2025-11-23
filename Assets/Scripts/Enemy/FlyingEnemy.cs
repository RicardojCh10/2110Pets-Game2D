using UnityEngine;
using System.Collections;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Estadísticas de Salud")]
    public int maxHealth = 40; // Más débil que el terrestre
    private int currentHealth;

    [Header("Referencias de UI")]
    public GameObject healthBarPrefab;
    private EnemyHealthBar healthBar;
    private static Transform canvasTransform;

    [Header("Botín (Loot)")]
    public GameObject coinPrefab;
    public GameObject healthPackPrefab;
    [Range(0, 100)] public int dropChance = 80; // 50% de probabilidad de soltar algo
    [Range(0, 100)] public int healthPackChance = 30;

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip deathSound;   

    [Header("IA de Vuelo")]
    public float flySpeed = 3.5f;
    public float attackRange = 7f; // Distancia para empezar a disparar
    public float stopDistance = 4f; // Distancia mínima (para no chocar con Aiden)
    public float fireRate = 2f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Transform playerTarget;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float nextFireTime = 0f;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Obtener AudioSource
        audioSource = GetComponent<AudioSource>();

        // --- CONFIGURACIÓN CLAVE PARA VOLAR ---
        rb.gravityScale = 0f;   // ¡CERO GRAVEDAD PARA QUE VUELE!
        rb.freezeRotation = true;
        // Recomendado: Aumentar el "Linear Drag" en el Inspector a 1 o 2 para que frene suave

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) playerTarget = playerObject.transform;

        SetupHealthBar();
    }

    void FixedUpdate()
    {
        if (isDead || playerTarget == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTarget.position);
        Vector2 directionVector = (playerTarget.position - transform.position).normalized;

        // 1. Voltear Sprite (solo basado en X)
        FlipSprite(directionVector.x);

        // 2. IA de Vuelo
        if (distance > stopDistance)
        {
            // Se mueve hacia el jugador en línea recta (X e Y)
            rb.linearVelocity = directionVector * flySpeed;
        }
        else
        {
            // Se detiene/frena suavemente
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 2f);
        }

        // 3. IA de Disparo
        if (distance <= attackRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (firePoint != null && bulletPrefab != null)
        {
            // Apunta la bala directo a Aiden
            Vector2 direction = (playerTarget.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            Instantiate(bulletPrefab, firePoint.position, rotation);
        }
    }

    void FlipSprite(float dirX)
    {
        if (dirX > 0) spriteRenderer.flipX = true;
        else if (dirX < 0) spriteRenderer.flipX = false;
    }

    // --- Funciones Compartidas (Daño, Muerte, UI) ---
    // (Mismas funciones que el robot terrestre)
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (healthBar != null) healthBar.SetCurrentHealth(currentHealth);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        GetComponent<Collider2D>().enabled = false;
        // Al morir, activamos la gravedad para que caiga al suelo dramáticamente
        rb.gravityScale = 2f; 
        rb.linearVelocity = Vector2.zero; // Frenar impulso lateral
        DropLoot(); 

        StartCoroutine(FadeOutAndDestroy());
    }


    // --- Lógica de Botín (Loot) ---
    void DropLoot()
    {
        // 1. ¿Tengo suerte? (Tira un dado de 0 a 100)
        int randomValue = Random.Range(0, 100);

        if (randomValue <= dropChance) // Si sacaste menos de 50 (ganaste)
        {
            // 2. ¿Qué premio me toca?
            int lootType = Random.Range(0, 100);

            if (lootType <= healthPackChance)
            {
                // Premio Mayor: Botiquín
                if (healthPackPrefab != null)
                    Instantiate(healthPackPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                // Premio Normal: Moneda
                if (coinPrefab != null)
                    Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 0.5f;
        float timer = 0f;
        Color originalColor = spriteRenderer.color;
        // Esperamos un poco antes de desvanecer para que se vea caer
        yield return new WaitForSeconds(0.5f); 
        
        while (timer < fadeDuration)
        {
            float progress = timer / fadeDuration;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - progress);
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    void SetupHealthBar()
    {
        // ... (Mismo código de SetupHealthBar del Robot Terrestre) ...
         if (canvasTransform == null)
        {
            GameObject canvasObject = GameObject.Find("Canvas");
            if (canvasObject != null) canvasTransform = canvasObject.transform;
        }
        if (healthBarPrefab != null && canvasTransform != null)
        {
            GameObject bar = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            bar.transform.SetParent(canvasTransform);
            healthBar = bar.GetComponent<EnemyHealthBar>();
            healthBar.target = this.transform;
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }
}