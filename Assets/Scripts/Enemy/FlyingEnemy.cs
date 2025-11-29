using UnityEngine;
using System.Collections;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Estadísticas de Salud")]
    public int maxHealth = 40; 
    private int currentHealth;

    [Header("Referencias de UI")]
    public GameObject healthBarPrefab;
    private EnemyHealthBar healthBar;
    private static Transform canvasTransform;

    [Header("Botín (Loot)")]
    public GameObject coinPrefab;
    public GameObject healthPackPrefab;
    [Range(0, 100)] public int dropChance = 80; 
    [Range(0, 100)] public int healthPackChance = 30;

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip deathSound;   

    [Header("IA de Vuelo")]
    public float flySpeed = 3.5f;
    public float attackRange = 7f; 
    public float stopDistance = 4f; 
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

        audioSource = GetComponent<AudioSource>();

        rb.gravityScale = 0f;   
        rb.freezeRotation = true;

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

        FlipSprite(directionVector.x);

        if (distance > stopDistance)
        {
            rb.linearVelocity = directionVector * flySpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 2f);
        }

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
        rb.gravityScale = 2f; 
        rb.linearVelocity = Vector2.zero; 
        DropLoot(); 

        StartCoroutine(FadeOutAndDestroy());
    }


    void DropLoot()
    {
        int randomValue = Random.Range(0, 100);

        if (randomValue <= dropChance)
        {
            int lootType = Random.Range(0, 100);

            if (lootType <= healthPackChance)
            {
                if (healthPackPrefab != null)
                    Instantiate(healthPackPrefab, transform.position, Quaternion.identity);
            }
            else
            {
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