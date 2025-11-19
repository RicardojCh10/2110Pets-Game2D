using UnityEngine;
using UnityEngine.SceneManagement;

public class KiroCompanion : MonoBehaviour
{
    [Header("Configuración Básica")]
    public Transform target;
    public float stopDistance = 1.0f;
    public float sprintDistance = 6f;
    public float teleportDistance = 15f;
    public float sprintMultiplier = 1.8f;

    [Header("Configuración de Salto y Física")]
    public float jumpForce = 8f; // Fuerza de salto (ajústala para que no sea descomunal)
    public float jumpCheckOffset = 1.2f; // Diferencia de altura para decidir saltar
    public float gravityScale = 3f; // Gravedad normal
    public float fallGravityScale = 5f; // Gravedad al caer (para que baje rápido)

    [Header("Detección de Suelo")]
    public Transform groundCheck; // Asigna un objeto vacío en las patas de Kiro
    public LayerMask groundLayer; // Capa del suelo
    private bool isGrounded;

    [Header("Velocidad por Nivel")]
    public float speedLevel1 = 5f;
    public float speedLevel2 = 3.5f;
    public float speedLevel3 = 2f;

    private float currentSpeed;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true;
        rb.gravityScale = gravityScale;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        SetSpeedBasedOnLevel();
    }

    void SetSpeedBasedOnLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Nivel_1") currentSpeed = speedLevel1;
        else if (sceneName == "Nivel_2") currentSpeed = speedLevel2;
        else if (sceneName == "Nivel_3") currentSpeed = speedLevel3;
        else currentSpeed = speedLevel1;
    }

    void Update()
    {
        // Checar si está en el suelo (si asignaste el groundCheck)
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        }
        else
        {
            // Si no asignaste groundCheck, usamos la velocidad vertical como aproximación
            isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.05f;
        }
    }

    void FixedUpdate()
    {
        // --- Control de Gravedad Dinámica (Igual que Aiden) ---
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallGravityScale; // Cae más rápido
        }
        else
        {
            rb.gravityScale = gravityScale; // Sube normal
        }

        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        float distanceY = target.position.y - transform.position.y;
        float directionX = target.position.x - transform.position.x;

        // --- Teletransporte de Emergencia ---
        if (distance > teleportDistance)
        {
            transform.position = target.position;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // --- Movimiento Horizontal ---
        float actualSpeed = currentSpeed;
        if (distance > sprintDistance) actualSpeed *= sprintMultiplier;

        if (distance > stopDistance)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(directionX) * actualSpeed, rb.linearVelocity.y);
            if(animator != null) animator.SetBool("IsWalking", true);
            if (Mathf.Abs(directionX) > 0.1f) FlipSprite(directionX);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if(animator != null) animator.SetBool("IsWalking", false);
        }

        // --- Salto Inteligente ---
        // Salta solo si: Aiden está alto, Kiro está en el suelo, y no estamos cayendo
        if (distanceY > jumpCheckOffset && isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if(animator != null) animator.SetTrigger("Jump");
        }
    }

    void FlipSprite(float direction)
    {
        if (direction > 0) spriteRenderer.flipX = true;
        else if (direction < 0) spriteRenderer.flipX = false;
    }
}