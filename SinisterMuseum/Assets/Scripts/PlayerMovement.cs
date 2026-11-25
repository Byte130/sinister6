using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private Camera Camera;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private bool team;

    [Header("Gem+throw")]
    public bool hasGem = false;
    [SerializeField] private GameObject gemPrefab;

    [Header("Throwing")]
    public float throwForce = 20f;
    public float ThrowArcUp = 1.5f;

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
    public float staminaRegenRate = 5f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public float maxGravity = -25f;
    public float gravityIncreaseRate = 1.2f;
    private float currentGravity;
    private Vector3 lastMoveDirection = Vector3.forward;


    [Header("dash")]
    [SerializeField] private float maxInvinsibleTimer = 1f;
    [SerializeField] private float invinsibleTimer;
    [SerializeField] private bool isStunned = false;
    [SerializeField] private float maxStunnedTimer = 1f;
    [SerializeField] private float stunnedTimer;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashTimer;
    [SerializeField] private bool isDashingActive = false;
    [SerializeField] private Vector3 dashDirection;

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

        if (invinsibleTimer > 0f)
        {
            invinsibleTimer -= Time.deltaTime;
        }
        else if (invinsibleTimer <= 0f && gameObject.tag == "invinsible")
        {
            gameObject.tag = "Player";
        }

        if (stunnedTimer > 0f)
        {
            stunnedTimer -= Time.deltaTime;
        }
        else if (stunnedTimer <= 0f && isStunned)
        {
            isStunned = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    private void FixedUpdate()
    {
        if(isStunned) return;
        bool isgrounded = IsGrounded();

        // Read input from this player's PlayerInput
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector2 aimInput = playerInput.actions["AimGem"].ReadValue<Vector2>();
        bool isSprinting = playerInput.actions["Sprint"].IsPressed();
        bool isDashing = playerInput.actions["Dash"].triggered;
        bool isJumping = playerInput.actions["Jump"].triggered;
        bool isThrowingGem = playerInput.actions["ThrowGem"].triggered;

        // Update dash timer
        if (isDashingActive)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashingActive = false;
            }
        }

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
            currentStamina += Time.fixedDeltaTime * staminaRegenRate;
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

        //throw gem logic
        if (isThrowingGem)
        {
            ThrowGem(aimInput);
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
            Debug.Log("Jumping");
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
        if (isStunned) return;

        if (currentStamina > 10f)
        {
            dashDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            if (dashDirection.magnitude < 0.1f)
            {
                dashDirection = lastMoveDirection;
            }

            rb.AddForce(dashDirection * sprintSpeed * 10f, ForceMode.VelocityChange);
            currentStamina -= 10f;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            isDashingActive = true;
            dashTimer = dashDuration;

            float dashDistance = sprintSpeed * 10f * dashDuration;
            RaycastHit hit;

            //if (Physics.Raycast(transform.position, dashDirection, out hit, dashDistance))
            //{
            //    if (hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject)
            //    {
            //        PlayerMovement otherPlayer = hit.collider.GetComponent<PlayerMovement>();
            //        if (otherPlayer != null && otherPlayer.hasGem)
            //        {
            //            otherPlayer.hasGem = false;
            //            hasGem = true;
            //        }
            //    }
            //}
        }
        else return;
    }

    public void specialdash(Vector2 moveInput)
    {
        if (currentStamina > 10f)
        {
            currentStamina -= 10f;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            gameObject.tag = "invinsible";
            invinsibleTimer = maxInvinsibleTimer;
        }
        else return;
    }

    //throwing
    public void ThrowGem(Vector2 aimInput)
    {
        if (hasGem && gemPrefab != null)
        {
            Vector3 throwDirection;

            // If aiming, use aim direction; otherwise use look direction
            //if (aimInput.magnitude > 0)
            //{
            //    throwDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;
            //}
            //else
            //{
            throwDirection = transform.forward;
            //}
            //commentend cause we maybe want this in a later build


            // Spawn gem slightly in front of player
            Vector3 spawnPos = transform.position + transform.forward * 1f;
            GameObject thrownGem = Instantiate(gemPrefab, spawnPos, Quaternion.identity);
            Rigidbody gemRb = thrownGem.GetComponent<Rigidbody>();

            // Apply throw force with slight upward arc
            Vector3 finalThrowDirection = throwDirection + Vector3.up * ThrowArcUp;
            gemRb.AddForce(finalThrowDirection.normalized * throwForce, ForceMode.VelocityChange);
            hasGem = false;
        }
    }


    public void getStunned()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        isStunned = true;
        stunnedTimer = maxStunnedTimer;
    }

    //grounded and moving checks
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
        if (isDashingActive && collision.gameObject.CompareTag("object/wall"))
        {
            isDashingActive = false;
            getStunned();
        }
        else if (isDashingActive && collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement otherPlayer = collision.gameObject.GetComponent<PlayerMovement>();
            if (otherPlayer != null&& !otherPlayer.hasGem)
            {
                otherPlayer.getStunned();
                getStunned();
                isDashingActive = false;
            } else if (otherPlayer != null && otherPlayer.hasGem)
            {
                otherPlayer.hasGem = false;
                hasGem = true;
                isDashingActive = false;
            }
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.useGravity = true;
        }
    }

    public void TeamSelect(bool isTeam)
    {
        team = isTeam;
    }

    public (bool, bool) BroadcastGemStatus()
    {
        if (hasGem)
        {
            //Debug.Log("Player " + team + " has the gem!");
        }
        return (hasGem, team);
    }

    private void OnDrawGizmosSelected()
    {
        float dashStunRadius = 1.5f;
        float dashGemStealRadius = 1.5f;

        // Draw stun hitbox (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashStunRadius);

        // Draw gem steal hitbox (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dashGemStealRadius);

        // Highlight active dash zones
        if (invinsibleTimer > 0f)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, dashStunRadius);

            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, dashGemStealRadius);
        }
    }
}