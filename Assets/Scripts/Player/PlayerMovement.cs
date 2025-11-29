using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 10f;

    [Header("Gravity Settings")]
    public float baseGravityScale = 3f;
    public float fallGravityScale = 6f;
    public float shortJumpCutoff = 0.5f;

    [Header("Crouch Settings")]
    public Vector2 crouchingColliderSize = new Vector2(2, 2);
    public Vector2 crouchingColliderOffset = new Vector2(0, -1);

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    private float moveInputX = 0f;
    private bool isGrounded;
    private bool isCrouching = false;
    private Vector2 standingColliderSize;
    private Vector2 standingColliderOffset;

    private Animator animator;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        standingColliderSize = capsuleCollider.size;
        standingColliderOffset = capsuleCollider.offset;

        rb.gravityScale = baseGravityScale;

        animator = GetComponent<Animator>();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputX = context.ReadValue<Vector2>().x;

        if (animator != null)
        {
            bool isWalking = Mathf.Abs(moveInputX) > 0.1f;
            animator.SetBool("Walking", isWalking);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (animator != null)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("Walking", false);
            }
        }

        if (context.canceled && rb.linearVelocity.y > 0) 
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * shortJumpCutoff);
        }
    }

public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
            capsuleCollider.size = crouchingColliderSize;
            capsuleCollider.offset = crouchingColliderOffset;

            if (animator != null)
            {
                animator.SetBool("IsCrouching", true);
                animator.SetBool("Walking", false); 
            }
        }

        if (context.canceled)
        {
            
            isCrouching = false;
            capsuleCollider.size = standingColliderSize;
            capsuleCollider.offset = standingColliderOffset;

            if (animator != null)
            {
                animator.SetBool("IsCrouching", false);
            }
        }
    }


    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        
        HandleSpriteFlip();
if (animator != null)
        {
            if (isGrounded)
            {
                animator.SetBool("IsJumping", false);
                
                bool isWalking = Mathf.Abs(moveInputX) > 0.1f && !isCrouching;
                animator.SetBool("Walking", isWalking);
            }
            else
            {
                if (rb.linearVelocity.y > 0.01f)
                {
                    animator.SetBool("IsJumping", true);
                }
                else if (rb.linearVelocity.y < -0.01f)
                {
                    animator.SetBool("IsJumping", false);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0) 
        {
            rb.gravityScale = fallGravityScale;
        }
        else 
        {
            rb.gravityScale = baseGravityScale;
        }

        if (!isCrouching) 
        {
            rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);
        }
        else 
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void HandleSpriteFlip()
    {
        if (moveInputX > 0) { transform.localScale = new Vector3(1, 1, 1); }
        else if (moveInputX < 0) { transform.localScale = new Vector3(-1, 1, 1); }
    }
}