using UnityEngine;

public class KiroCompanion : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // El Transform de Aiden
    public AidenMovementRecorder aidenRecorder; // Script grabador de Aiden

    [Header("Configuración de Seguimiento")]
    public float stopDistance = 1.0f;
    public float sprintDistance = 6f;
    public float teleportDistance = 15f;
    public float followSpeed = 6f; // Velocidad base de seguimiento
    public float sprintMultiplier = 1.8f;
    public float followSmoothness = 0.5f; // Suavidad del seguimiento (0 a 1)

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
        
        if (target != null)
        {
            aidenRecorder = target.GetComponent<AidenMovementRecorder>();
        }
        
        if (aidenRecorder == null)
        {
             Debug.LogError("AidenMovementRecorder no se encontró en el objetivo. ¡El seguimiento fallará!");
        }
    }

    void FixedUpdate()
    {
        if (target == null || aidenRecorder == null) return;

        Vector3 delayedPosition = aidenRecorder.GetDelayedPosition();
        Vector3 currentPosition = transform.position;
        
        float distance = Vector2.Distance(currentPosition, delayedPosition);

        // --- 1. Teletransporte de Emergencia ---
        if (distance > teleportDistance)
        {
            transform.position = target.position; // Teletransporta a la posición actual de Aiden
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // --- 2. Lógica de Movimiento y Seguimiento ---
        if (distance > stopDistance)
        {
            float actualSpeed = followSpeed;
            if (distance > sprintDistance) actualSpeed *= sprintMultiplier;

            // Calcular la dirección hacia el punto de retraso
            Vector3 direction = (delayedPosition - currentPosition).normalized;
            
            // Usamos la posición Y del punto retrasado para que Kiro suba y baje con Aiden
            Vector2 targetVelocity = new Vector2(direction.x * actualSpeed, direction.y * actualSpeed);
            
            // Suavizamos el seguimiento (Lerp) para un movimiento más natural
            Vector2 newVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, followSmoothness);
            
            rb.linearVelocity = new Vector2(newVelocity.x, newVelocity.y);

            // 3. Voltear Sprite
            FlipSprite(direction.x);
        }
        else
        {
            // Detener el movimiento si está dentro de la distancia de parada
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void FlipSprite(float directionX)
    {
        if (directionX > 0) spriteRenderer.flipX = true;
        else if (directionX < 0) spriteRenderer.flipX = false;
    }
}