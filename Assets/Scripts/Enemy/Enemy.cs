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

        // --- NUEVO: Obtener componentes de IA ---
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Importante: Asegúrate de que tu Rigidbody2D sea "Kinematic"
        rb.isKinematic = true; 

        // --- NUEVO: Encontrar al jugador (Aiden) ---
        // ¡¡Asegúrate de que tu objeto Aiden tenga el Tag "Player"!!
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

    // --- NUEVO: Update para la IA ---
    void Update()
    {
        // Si el enemigo está muerto o no encuentra al jugador, no hace nada
        if (isDead || playerTarget == null)
        {
            return;
        }

        // 1. Calcular distancia y dirección
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer > attackRange)
        {
            // --- 2. Perseguir al Jugador ---
            // Moverse hacia la posición del jugador
            Vector2 targetPosition = new Vector2(playerTarget.position.x, transform.position.y); // Solo se mueve en X
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Voltear el sprite para que mire al jugador
            FlipSprite(playerTarget.position.x - transform.position.x);
        }
        else
        {
            // --- 3. Atacar al Jugador ---
            // Si estamos en rango y el cooldown ha pasado
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown; // Reinicia el cooldown
            }
        }
    }

    // --- NUEVO: Función para voltear el sprite ---
    void FlipSprite(float direction)
    {
        if (direction > 0)
        {
            spriteRenderer.flipX = false; // Mirando a la derecha
        }
        else if (direction < 0)
        {
            spriteRenderer.flipX = true; // Mirando a la izquierda
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