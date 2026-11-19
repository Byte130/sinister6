using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private Camera Camera;
    [SerializeField] private Rigidbody rb;
    public bool hasGem = false;

    [Header("Movement")]
    private float moveSpeed;
    public float basespeed = 5f;
    public float acceleration = 20f;
    public float deceleration = 30f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Jumping")]
    public float jumpHeight = 3f;

    [Header("Sprint")]
    public float sprintSpeed = 10f;
    public float maxStamina = 100f;
    public float currentStamina = 50f;
    public bool canSprint = true;
    public float staminaThreshold = 10f;
    public float staminaDrainRate = 5f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public float maxGravity = -25f;
    public float gravityIncreaseRate = 1.2f;
    private float currentGravity;
    private Vector3 lastMoveDirection = Vector3.forward;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        currentGravity = gravity;
        rb.useGravity = false;
    }

    private void Update()
    {
        bool grounded = IsGrounded();
    }

    private void FixedUpdate()
    {
        bool isgrounded = IsGrounded();

        // Read input from this player's PlayerInput
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        bool isSprinting = playerInput.actions["Sprint"].IsPressed();
        bool isDashing = playerInput.actions["Dash"].triggered;
        bool isJumping = playerInput.actions["Jump"].triggered;

        //gravity during jump
        if (!IsGrounded())
        {
            currentGravity -= gravityIncreaseRate * Time.fixedDeltaTime;
            currentGravity = Mathf.Max(currentGravity, maxGravity);
            rb.AddForce(Vector3.up * currentGravity, ForceMode.Acceleration);
        }
        else
        {
            currentGravity = gravity;
        }

        // Sprinting logic
        IsSprinting(isSprinting);
        if (isSprinting && canSprint == true)
        {
            moveSpeed = sprintSpeed;
            currentStamina -= Time.fixedDeltaTime * staminaDrainRate;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
        else
        {
            moveSpeed = basespeed;
            currentStamina += Time.fixedDeltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        //movement logic
        Vector3 horizontalVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.y * moveSpeed);
        rb.linearVelocity = horizontalVelocity;

        //dash logic
        if (isDashing)
        {
            if (hasGem)
            {
                specialdash(moveInput);
            }
            else
            {
                Dash(moveInput);
            }
        }

        //turn logic
        if (moveInput.magnitude > 0.1f)
        {
            lastMoveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
            rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        //jump logic
        if (isJumping && isgrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * gravity), ForceMode.VelocityChange);
        }
    }

    public void IsSprinting(bool isSprinting)
    {
        if (isSprinting && currentStamina <= 0.1)
        {
            canSprint = false;
        }
        else if (isSprinting && currentStamina > staminaThreshold)
        {
            canSprint = true;
        }
    }

    //dashes
    public void Dash(Vector2 moveInput)
    {
        if (currentStamina > 10f)
        {
            Vector3 dashDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            rb.AddForce(dashDirection * sprintSpeed * 10f, ForceMode.VelocityChange);
            currentStamina -= 10f;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
        else return;
    }

    public void specialdash(Vector2 moveInput)
    {
        // Implementation here
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.2f);
    }

    public bool IsMoving()
    {
        return playerInput.actions["Move"].ReadValue<Vector2>().magnitude > 0.1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.useGravity = true;
        }
    }
}