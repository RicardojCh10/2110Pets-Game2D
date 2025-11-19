using UnityEngine;
using System.Collections;

public class RobotEnemy : MonoBehaviour
{
    [Header("Estadísticas de Salud")]
    public int maxHealth = 150; // Más resistente que el zombie
    private int currentHealth;

    [Header("Referencias de UI")]
    public GameObject healthBarPrefab;
    private EnemyHealthBar healthBar;
    private static Transform canvasTransform;

    [Header("IA de Combate")]
    public float moveSpeed = 2f; // Más lento y pesado
    public float shootingRange = 6f; // Distancia para empezar a disparar
    public float fireRate = 1.5f; // Tiempo entre disparos
    public GameObject bulletPrefab; // ¡Arrastra el prefab de EnemyBullet aquí!
    public Transform firePoint;     // El punto vacío en la punta del cañón

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

        // Configuración física para robot terrestre
        rb.gravityScale = 3f;   // Pesado
        rb.freezeRotation = true;

        // Encontrar a Aiden
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) playerTarget = playerObject.transform;

        // Configuración del Canvas (Igual que tu script anterior)
        SetupHealthBar();
    }

    void FixedUpdate()
    {
        if (isDead || playerTarget == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Mantener gravedad pero frenar X
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);
        float directionX = playerTarget.position.x - transform.position.x;

        // 1. Voltear el Sprite siempre hacia el jugador
        FlipSprite(directionX);

        // 2. IA de Movimiento
        if (distanceToPlayer > shootingRange)
        {
            // ESTADO: PERSEGUIR
            // Se mueve en X hacia el jugador, mantiene su velocidad Y (gravedad)
            rb.linearVelocity = new Vector2(Mathf.Sign(directionX) * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // ESTADO: DISPARAR
            // Se detiene en X para apuntar estable
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
            // Calcular la rotación para apuntar a Aiden
            Vector2 direction = (playerTarget.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            Instantiate(bulletPrefab, firePoint.position, rotation);
        }
    }

    void FlipSprite(float direction)
    {
        // Ajusta esto según hacia dónde mire tu sprite original
        if (direction > 0) spriteRenderer.flipX = true;  // Mira derecha
        else if (direction < 0) spriteRenderer.flipX = false; // Mira izquierda
    }

    // --- Funciones Compartidas (Daño y Muerte) ---
    // (Son idénticas a tu script de Enemy.cs para mantener consistencia)

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
        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(FadeOutAndDestroy());
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