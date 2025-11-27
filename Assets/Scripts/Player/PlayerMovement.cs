using UnityEngine;
using UnityEngine.InputSystem; // ¡Asegúrate de que esta línea esté!

public class PlayerMovement : MonoBehaviour
{
    // --- Referencias de Movimiento ---
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 10f;

    // --- Configuración de Gravedad ---
    [Header("Gravity Settings")]
    public float baseGravityScale = 3f;
    public float fallGravityScale = 6f;
    public float shortJumpCutoff = 0.5f;

    // --- Referencias de Agacharse ---
    [Header("Crouch Settings")]
    public Vector2 crouchingColliderSize = new Vector2(2, 2);
    public Vector2 crouchingColliderOffset = new Vector2(0, -1);

    // --- Referencias del Suelo ---
    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Corner Snagging Fix")]
    public LayerMask cornerLayer;     
    public float raycastHorizontalOffset = 0.4f; 

    public float verticalRaycastHeight = 0.5f;    
    public float pushAmount = 0.2f;    
    // --- Componentes ---
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    // --- Variables de Estado ---
    private float moveInputX = 0f;
    private bool isGrounded;
    private bool isCrouching = false;
    private Vector2 standingColliderSize;
    private Vector2 standingColliderOffset;

    //Variable de animacion
    private Animator animator;
    
    // (Hemos eliminado las variables de PlayerControls, Awake, OnEnable y OnDisable
    // porque el componente PlayerInput se encarga de eso ahora)

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        // Guardamos las dimensiones originales del collider
        standingColliderSize = capsuleCollider.size;
        standingColliderOffset = capsuleCollider.offset;

        // Establecer la gravedad inicial
        rb.gravityScale = baseGravityScale;

        // Obtener el componente Animator
        animator = GetComponent<Animator>();
    }

    // --- FUNCIONES PÚBLICAS DE EVENTOS ---
    // Estas son las funciones que conectarás en el Inspector

    public void OnMove(InputAction.CallbackContext context)
    {
        // Leemos el valor Vector2 de la acción "Move"
        // y solo nos quedamos con el componente horizontal (X)
        moveInputX = context.ReadValue<Vector2>().x;

        // Actualizar la animación de correr
        if (animator != null)
        {
            bool isWalking = Mathf.Abs(moveInputX) > 0.1f;
            animator.SetBool("Walking", isWalking);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Si se PRESIONA la tecla
        if (context.performed && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (animator != null)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("Walking", false);
            }
        }

        // Si se SUELTA la tecla (para el salto corto)
        if (context.canceled && rb.linearVelocity.y > 0) 
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * shortJumpCutoff);
        }
    }

public void OnCrouch(InputAction.CallbackContext context)
    {
        // Si se PRESIONA la tecla
        if (context.performed)
        {
            isCrouching = true;
            capsuleCollider.size = crouchingColliderSize;
            capsuleCollider.offset = crouchingColliderOffset;

            // ¡NUEVO! Activa la animación de agacharse
            if (animator != null)
            {
                animator.SetBool("IsCrouching", true);
                // También es buena práctica asegurar que Walking se detenga
                animator.SetBool("Walking", false); 
            }
        }

        // Si se SUELTA la tecla
        if (context.canceled)
        {
            // Antes de levantarse, puedes añadir una lógica de comprobación
            // para ver si no hay un techo sobre el jugador, pero por ahora solo se levanta:
            
            isCrouching = false;
            capsuleCollider.size = standingColliderSize;
            capsuleCollider.offset = standingColliderOffset;

            // ¡NUEVO! Desactiva la animación de agacharse
            if (animator != null)
            {
                animator.SetBool("IsCrouching", false);
            }
        }
    }

    // --- Actualizaciones de Física y Frames ---

    private void Update()
    {
        // Comprobamos si está en el suelo en cada frame
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        
        HandleSpriteFlip();
if (animator != null)
        {
            if (isGrounded)
            {
                // Si está en el suelo, desactiva cualquier animación aérea.
                animator.SetBool("IsJumping", false);
                
                // Vuelve a comprobar si camina si está en el suelo (ya lo tienes en OnMove, pero es más seguro aquí)
                bool isWalking = Mathf.Abs(moveInputX) > 0.1f && !isCrouching;
                animator.SetBool("Walking", isWalking);
            }
            else // Si está en el aire
            {
                // Si la velocidad vertical es positiva, está subiendo.
                if (rb.linearVelocity.y > 0.01f)
                {
                    animator.SetBool("IsJumping", true);
                }
                // Si la velocidad vertical es negativa, está cayendo.
                else if (rb.linearVelocity.y < -0.01f)
                {
                    animator.SetBool("IsJumping", false);
                }
            }
        }
    }
 // PlayerMovement.cs

    private void FixedUpdate()
    {
        // 1. Control de Gravedad
        if (rb.linearVelocity.y < 0) // Si estamos cayendo
        {
            rb.gravityScale = fallGravityScale;
        }
        else // Si estamos subiendo o en el suelo
        {
            rb.gravityScale = baseGravityScale;
        }

        // 2. Control de Movimiento
 if (!isCrouching || !isGrounded) // Si NO está agachado O NO está en el suelo
    {
        // Si no está agachado, o si está en el aire (y puede moverse)
        rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);
    }
    else // Si SÍ está agachado Y SÍ está en el suelo
    {
        // Detener el movimiento horizontal al agacharse en el suelo
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
        
// 3. Lógica Anti-Enganche en Esquinas (Basada en la imagen)
    
    // Raycast Izquierda
    RaycastHit2D raycastHitIzquierda = Physics2D.Raycast(
        (Vector2)transform.position + new Vector2(-raycastHorizontalOffset, verticalRaycastHeight), 
        Vector2.up, // Dirección: Arriba
        0.05f, // Longitud: Un valor muy pequeño para solo chequear la esquina
        cornerLayer
    );

    // Raycast Derecha
    RaycastHit2D raycastHitDerecha = Physics2D.Raycast(
        (Vector2)transform.position + new Vector2(raycastHorizontalOffset, verticalRaycastHeight), 
        Vector2.up, // Dirección: Arriba
        0.05f, // Longitud
        cornerLayer
    );

    // Si está tocando el raycast izquierdo pero no el derecho (moviéndose a la izquierda o detenido)
    if (raycastHitIzquierda && !raycastHitDerecha)
    {
        // Empujar ligeramente a la derecha
        transform.position += new Vector3(pushAmount, 0f, 0f);
    }
    
    // Si está tocando el raycast derecho pero no el izquierdo (moviéndose a la derecha o detenido)
    else if (raycastHitDerecha && !raycastHitIzquierda)
    {
        // Empujar ligeramente a la izquierda
        transform.position -= new Vector3(pushAmount, 0f, 0f);
    }
}
    private void HandleSpriteFlip()
    {
        // Voltear el sprite
        if (moveInputX > 0) { transform.localScale = new Vector3(1, 1, 1); }
        else if (moveInputX < 0) { transform.localScale = new Vector3(-1, 1, 1); }
    }
}