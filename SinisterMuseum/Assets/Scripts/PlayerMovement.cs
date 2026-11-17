using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionReference move;
    public InputActionReference jump;

    [SerializeField] private Camera Camera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5f;
    private bool grounded = true;


    [Header("Jumping")]
    public float jumpHeight = 3f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public float maxGravity = -25f;
    public float gravityIncreaseRate = 1.2f;
    private float currentGravity;

    private bool wasGrounded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentGravity = gravity; 
        wasGrounded = IsGrounded();

    }

    private void Update()
    {
        bool grounded = IsGrounded();
    }

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
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




        Vector2 moveInput = move.action.ReadValue<Vector2>();
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        rb.linearVelocity = movement * moveSpeed;

        if (jump.action.triggered && isgrounded)
        {
                rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * gravity), ForceMode.VelocityChange);
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
