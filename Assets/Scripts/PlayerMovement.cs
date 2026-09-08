using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;

    [SerializeField] private float speed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float mouseSensitivity;

    Rigidbody rb;
    Camera playerCamera;

    float cameraRotationX = 0f;
    float mouseX;
    float mouseY;


    [SerializeField] private LayerMask floor;

    [SerializeField] private float jumpForce;
    private bool isGrounded;
    private bool jump;
    [SerializeField] private float fallFactor;

    [SerializeField] private int maxJumps;
    [SerializeField] private float coyoteTimerDuration;
    private float coyoteTimer;
    private int jumpCounter;

    private float jumpBufferTimer;
    [SerializeField] private float jumpBufferDuration;

    private enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling
    }

    private PlayerState currentState;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        coyoteTimer = coyoteTimerDuration;
        jumpBufferTimer = jumpBufferDuration;

        currentState = PlayerState.Falling;

        //Avoids character rotation issues with physics && Objects
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        CameraRotation();

        UpdateTimers();
        HandleJumpInput();
        UpdateState();
    }

    private void UpdateTimers()
    {
        if (!isGrounded)
        {
            coyoteTimer += Time.deltaTime;
        }

        if (jumpBufferTimer < jumpBufferDuration)
        {
            jumpBufferTimer += Time.deltaTime;
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = 0f;
        }

        if (jumpBufferTimer < jumpBufferDuration)
        {
            if (isGrounded || coyoteTimer < coyoteTimerDuration || (jumpCounter > 0 && jumpCounter < maxJumps))
            {
                jump = true;
            }
        }
    }

    private void UpdateState() 
    {
        if (isGrounded)
        {
            if (horizontalInput != 0 || verticalInput != 0)
            {
                if (Input.GetButton("Sprint"))
                {
                    ChangeState(PlayerState.Running);
                }
                else
                {
                    ChangeState(PlayerState.Walking);
                }
            }
            else
            {
                ChangeState(PlayerState.Idle);
            }

            return;
        }

        if (rb.linearVelocity.y > 0.1f)
        {
            ChangeState(PlayerState.Jumping);
        }
        else if (rb.linearVelocity.y < -0.1f)
        {
            ChangeState(PlayerState.Falling);
        }
    }

    private void ChangeState(PlayerState newState) //State Change Logic with Debug Log
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        Debug.Log("State: " + currentState);
    }

    private void CameraRotation() //Camera Rotation Logic with Mouse and Character Logic
    {
        transform.Rotate(Vector3.up * mouseX);

        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -60f, 60f);

        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }

    private void FixedUpdate() //physics update
    {
        Movement();
        Jump();
        Gravity();

        //Avoids physics issues with RB rotation, since we are using linear velocity for movement
        rb.angularVelocity = Vector3.zero;
    }

    private void Movement() //movement logic, uses linear velocity to avoid physics with RB
    {
        float currentSpeed = speed;

        if (currentState == PlayerState.Running)
        {
            currentSpeed = runningSpeed;
        }

        Vector3 direction = (transform.forward * verticalInput + transform.right * horizontalInput) * currentSpeed;

        rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);
    }

    private void Jump() //Jump Physics and State Change
    {
        if (jump)
        {
            jump = false;
            jumpCounter++;

            coyoteTimer = coyoteTimerDuration;
            jumpBufferTimer = jumpBufferDuration;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

            ChangeState(PlayerState.Jumping);
        }
    }

    private void Gravity() // gravity multiplier for better jump feel
    {
        if (rb.linearVelocity.y < -0.1f)
        {
            rb.linearVelocity += Physics.gravity * (fallFactor - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0.1f)
        {
            rb.linearVelocity += Physics.gravity * (fallFactor - 1) * Time.deltaTime;
        }
    }

    public void RespawnData()
    {
        isGrounded = false;
        jumpCounter = 0;
        coyoteTimer = coyoteTimerDuration;
        jumpBufferTimer = jumpBufferDuration;
        ChangeState(PlayerState.Falling);
    }

    private bool IsFloor(GameObject objectToCheck) //floor Layer Check
    {
        return ((1 << objectToCheck.layer) & floor) != 0;
    }
    private bool IsGroundSurface(Collision collision)
    {
        if (!IsFloor(collision.gameObject))
        {
            return false;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.contacts[i].normal.y > 0.5f)
            {
                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision) //ground check
    {
        if (IsGroundSurface(collision))
        {
            isGrounded = true;

            jumpCounter = 0;
            coyoteTimer = 0f;

            UpdateState();

            Debug.Log("Grounded");
        }
    }

    private void OnCollisionStay(Collision collision) //stay on ground check
    {
        if (IsGroundSurface(collision))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision) //exit ground check
    {
        if (IsFloor(collision.gameObject))
        {
            isGrounded = false;

            UpdateState();

            Debug.Log("Left Ground");
        }
    }
}