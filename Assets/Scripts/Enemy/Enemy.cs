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
    [Range(0, 100)] public int dropChance = 50; // 50% de probabilidad de soltar algo
    [Range(0, 100)] public int healthPackChance = 20;

    // Estadísticas de IA
    [Header("Estadísticas de IA")]
    public float moveSpeed = 3f; // Qué tan rápido se mueve
    public float attackRange = 1.5f; // Qué tan cerca debe estar para atacar
    public int attackDamage = 10; // Cuánto daño hace
    public float attackCooldown = 2f; // Tiempo (en seg) entre ataques

    //  Referencias de IA ---
    private Transform playerTarget; // El transform de Aiden
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float nextAttackTime = 0f; // Para el cooldown del ataque
    private bool isDead = false; // Para evitar que ataque después de muerto

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        // Obtener componentes de IA ---
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // rb.gravityScale = 0;
        rb.freezeRotation = true; 
        
        // Encontrar al jugador (Aiden)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        else
        {
            Debug.LogError("¡No se encontró al jugador! Asegúrate de que Aiden tenga el Tag 'Player'.");
        }

        // --- Lógica del Canvas  ---
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

    // Update para la IA 
   void FixedUpdate()
    {
        if (isDead || playerTarget == null)
        {
            rb.linearVelocity = Vector2.zero; // Detente si estás muerto
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
        float directionToPlayer = playerTarget.position.x - transform.position.x;

        if (distanceToPlayer > attackRange)
        {
            // --- 2. Perseguir al Jugador ---
            // Moverse usando la velocidad del Rigidbody
            rb.linearVelocity = new Vector2(Mathf.Sign(directionToPlayer) * moveSpeed, rb.linearVelocity.y);
            
            // Voltear el sprite
            FlipSprite(directionToPlayer);
        }
        else
        {
            // --- 3. Atacar al Jugador ---
            rb.linearVelocity = Vector2.zero; // Detente para atacar
            FlipSprite(directionToPlayer); // Asegúrate de estar mirando al jugador

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    //  Función para voltear el sprite 
    void FlipSprite(float direction)
    {
        if (direction > 0)
        {
            spriteRenderer.flipX = true; // Mirando a la derecha
        }
        else if (direction < 0)
        {
            spriteRenderer.flipX = false; // Mirando a la izquierda
        }
    }

    // Función de Ataque ---
    void Attack()
    {
        Debug.Log("¡Enemigo ataca!");
        // Llama al GameManager para hacerle daño a Aiden
        GameManager.Instance.TakePlayerDamage(attackDamage);
        // Aquí podrías activar una animación de ataque
    }

    // --- Lógica de Daño ---
    public void TakeDamage(int damage)
    {
        if (isDead) return; // No puede recibir daño si ya está muerto

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

    // --- Lógica de Muerte  ---
    void Die()
    {
        isDead = true; //  Marca al enemigo como muerto

        GetComponent<Collider2D>().enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero; 

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

    // --- Lógica de Desvanecimiento ---
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