using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Estadísticas de Salud")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Referencias de UI")]
    public GameObject healthBarPrefab;
    private EnemyHealthBar healthBar;
    private static Transform canvasTransform;

    [Header("Botín (Loot)")]
    public GameObject coinPrefab;
    public GameObject healthPackPrefab;
    [Range(0, 100)] public int dropChance = 50; 
    [Range(0, 100)] public int healthPackChance = 20;

    [Header("Estadísticas de IA")]
    public float moveSpeed = 3f; 
    public float attackRange = 1.5f;
    public int attackDamage = 10; 
    public float attackCooldown = 2f; 

    private Transform playerTarget; 
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float nextAttackTime = 0f;
    private bool isDead = false; 

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip deathSound;    
    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();

        rb.freezeRotation = true; 
        
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        else
        {
            Debug.LogError("¡No se encontró al jugador! Asegúrate de que Aiden tenga el Tag 'Player'.");
        }

        if (canvasTransform == null)
        {
            GameObject canvasObject = GameObject.Find("Canvas");
            if (canvasObject != null)
            {
                canvasTransform = canvasObject.transform;
            }
            else
            {
                Debug.LogError("No se encontró el objeto 'Canvas' en la escena.");
                return;
            }
        }
        if (healthBarPrefab != null)
        {
            GameObject healthBarObject = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            healthBarObject.transform.SetParent(canvasTransform);
            healthBar = healthBarObject.GetComponent<EnemyHealthBar>();
            healthBar.target = this.transform;
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }

   void FixedUpdate()
    {
        if (isDead || playerTarget == null)
        {
            rb.linearVelocity = Vector2.zero; 
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
        float directionToPlayer = playerTarget.position.x - transform.position.x;

        if (distanceToPlayer > attackRange)
        {

            rb.linearVelocity = new Vector2(Mathf.Sign(directionToPlayer) * moveSpeed, rb.linearVelocity.y);
            
            FlipSprite(directionToPlayer);
        }
        else
        {
            rb.linearVelocity = Vector2.zero; 
            FlipSprite(directionToPlayer); 

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void FlipSprite(float direction)
    {
        if (direction > 0)
        {
            spriteRenderer.flipX = true; 
        }
        else if (direction < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void Attack()
    {
        Debug.Log("¡Enemigo ataca!");
        GameManager.Instance.TakePlayerDamage(attackDamage);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.SetCurrentHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true; 

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        GetComponent<Collider2D>().enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero; 

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
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        float fadeDuration = 0.5f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            float progress = timer / fadeDuration;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - progress);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}