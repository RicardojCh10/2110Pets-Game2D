using UnityEngine;
using System.Collections;

public class RobotEnemy : MonoBehaviour
{
    [Header("Estadísticas de Salud")]
    public int maxHealth = 150;
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



    [Header("IA de Combate")]
    public float moveSpeed = 2f; 
    public float shootingRange = 6f; 
    public float fireRate = 1.5f;
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

        rb.gravityScale = 3f;  
        rb.freezeRotation = true;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) playerTarget = playerObject.transform;

        SetupHealthBar();
    }

    void FixedUpdate()
    {
        if (isDead || playerTarget == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
        float directionX = playerTarget.position.x - transform.position.x;

        FlipSprite(directionX);

        if (distanceToPlayer > shootingRange)
        {
        
            rb.linearVelocity = new Vector2(Mathf.Sign(directionX) * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
         
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
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

    void FlipSprite(float direction)
    {
        if (direction > 0) spriteRenderer.flipX = true;  
        else if (direction < 0) spriteRenderer.flipX = false; 
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