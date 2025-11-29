using UnityEngine;

public class KiroCompanion : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;
    public AidenMovementRecorder aidenRecorder; 

    [Header("Configuración de Seguimiento")]
    public float stopDistance = 1.0f;
    public float sprintDistance = 6f;
    public float teleportDistance = 15f;
    public float followSpeed = 6f; 
    public float sprintMultiplier = 1.8f;
    public float followSmoothness = 0.5f; 

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

        if (distance > teleportDistance)
        {
            transform.position = target.position; 
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (distance > stopDistance)
        {
            float actualSpeed = followSpeed;
            if (distance > sprintDistance) actualSpeed *= sprintMultiplier;

            Vector3 direction = (delayedPosition - currentPosition).normalized;
            
            Vector2 targetVelocity = new Vector2(direction.x * actualSpeed, direction.y * actualSpeed);
            
            Vector2 newVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, followSmoothness);
            
            rb.linearVelocity = new Vector2(newVelocity.x, newVelocity.y);

            FlipSprite(direction.x);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void FlipSprite(float directionX)
    {
        if (directionX > 0) spriteRenderer.flipX = true;
        else if (directionX < 0) spriteRenderer.flipX = false;
    }
}