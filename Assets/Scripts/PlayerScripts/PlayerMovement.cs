using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("MovementSettings")]
    public float moveSpeed;
    public float gravityMultiplier = 2f;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;
    [Header("JumpSettings")]

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("SwingingSettings")]
    public float swingSpeed = 50f;
    public float swingControlMultiplier = 1f;
    public bool activeGrapple;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        swinging,
        walking,
        sprinting,
        sliding,
        air
    }

    public bool swinging;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {   
        
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.6f, whatIsGround);

        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.6f), grounded ? Color.green : Color.red);

        myInput();
        SpeedControl();
        StateHandler();

        if(grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        if (!grounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier * 10f, ForceMode.Acceleration);
        }
    }

    private void myInput()
    {   
        //Movement

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jumping
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        if(swinging)
        {
            state = MovementState.swinging;
            moveSpeed = swingSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }

    }

    private void MovePlayer()
    {   
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Swinging movement 
        if(swinging)
        {
            rb.AddForce(moveDirection.normalized * swingSpeed * swingControlMultiplier, ForceMode.Force);
        }
        // On slope movement
        else if(OnSlope() && !exitingSlope)
        {
            if (horizontalInput != 0 || verticalInput != 0)
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 5f, ForceMode.Force);
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        // Ground movement
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 5f, ForceMode.Force);
        }
        // Air movement
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 5f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if(swinging)
        {
            return;
        }

        if(OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); 

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

    }

    private void Jump()
    {   
        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(OnSlope())
        {
            rb.AddForce(slopeHit.normal * jumpForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.7f, whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}