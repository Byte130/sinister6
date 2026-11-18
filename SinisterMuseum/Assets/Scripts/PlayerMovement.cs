using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference move;
    public InputActionReference jump;
    public InputActionReference sprint;

    [SerializeField] private Camera Camera;
    [SerializeField] private Rigidbody rb;

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
    public float currentStamina=50f;
    public bool canSprint = true;
    public float staminaThreshold = 10f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public float maxGravity = -25f;
    public float gravityIncreaseRate = 1.2f;
    private float currentGravity;
    private Vector3 lastMoveDirection = Vector3.forward;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentGravity = gravity; 
        rb.useGravity = false;
    }

    private void Update()
    {
        bool grounded = IsGrounded();
    }

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        sprint.action.Enable();
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        sprint.action.Disable();
    }

    private void FixedUpdate()
    {
        bool isgrounded = IsGrounded();
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
        IsSprinting();
        if (sprint.action.IsPressed()&&canSprint==true)
        {
            moveSpeed = sprintSpeed; // Set sprint speed
            currentStamina -= Time.fixedDeltaTime*5f; // Decrease stamina over time
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Clamp stamina to max value
        }
        else
        {
            moveSpeed = basespeed; // Set normal speed
            currentStamina += Time.fixedDeltaTime; // Regenerate stamina over time
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Clamp stamina to max value
        }

        //movement logic
        Vector2 moveInput = move.action.ReadValue<Vector2>();
        Vector3 horizontalVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.y * moveSpeed);
        rb.linearVelocity = horizontalVelocity;



        //turn logic
        if (moveInput.magnitude > 0.1f)
        {
            lastMoveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
            rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }


        //jump logic
        if (jump.action.triggered && isgrounded)
        {
                rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * gravity), ForceMode.VelocityChange);
        }
    }
    public void IsSprinting()
    {
        if (sprint.action.IsPressed() && currentStamina <= 0.1)
        {
            canSprint = false;
        }
        else if (sprint.action.IsPressed() && currentStamina >staminaThreshold)
        {
            canSprint = true;
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.2f);
    }

    public bool IsMoving()
    {
        return move.action.ReadValue<Vector2>().magnitude > 0.1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.useGravity = true;
        }
    }
}
