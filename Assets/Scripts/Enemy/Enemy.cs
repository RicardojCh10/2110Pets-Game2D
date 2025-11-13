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

    // --- NUEVO: Estadísticas de IA ---
    [Header("Estadísticas de IA")]
    public float moveSpeed = 3f; // Qué tan rápido se mueve
    public float attackRange = 1.5f; // Qué tan cerca debe estar para atacar
    public int attackDamage = 10; // Cuánto daño hace
    public float attackCooldown = 2f; // Tiempo (en seg) entre ataques

    // --- NUEVO: Referencias de IA ---
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

        // --- Lógica del Canvas (sin cambios) ---
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

    

    // --- NUEVO: Función de Ataque ---
    void Attack()
    {
        Debug.Log("¡Enemigo ataca!");
        // Llama al GameManager para hacerle daño a Aiden
        GameManager.Instance.TakePlayerDamage(attackDamage);
        // Aquí podrías activar una animación de ataque
    }

    // --- Lógica de Daño (sin cambios) ---
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

    // --- Lógica de Muerte (con un pequeño ajuste) ---
    void Die()
    {
        isDead = true; // --- NUEVO: Marca al enemigo como muerto

        GetComponent<Collider2D>().enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        StartCoroutine(FadeOutAndDestroy());
    }

    // --- Lógica de Desvanecimiento (sin cambios) ---
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