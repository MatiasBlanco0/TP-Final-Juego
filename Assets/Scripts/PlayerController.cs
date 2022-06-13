using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float maxYVelocity;

    float desiredMoveSpeed;
    float lastDesiredMoveSpeed;

    public float dashForce;
    public float dashCooldown;
    bool readyToDash;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    bool readyToDoubleJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    float startYScale;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool isOnGround;

    [Header("Inputs")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.Q;

    [Header("Sounds")]
    public AudioClip dashSound;
    public AudioClip jumpSound;
    public AudioClip winSound;
    public AudioSource audioSource;

    [Header("Win")]
    public bool won = false;

    [Header("References")]
    public Transform orientation;
    WallRunning wallRunning;
    public GameObject confeti;
    public UIController uiController;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        wallrunning,
        crouching,
        sliding,
        air
    }

    public bool sliding;
    public bool wallrunning;

    // Start is called before the first frame update
    void Start()
    {
        walkSpeed = 7f;
        sprintSpeed = 10f;
        slideSpeed = 30f;
        wallrunSpeed = 8.5f;
        maxYVelocity = 30f;
        speedIncreaseMultiplier = 1.5f;
        slopeIncreaseMultiplier = 2.5f;
        dashForce = 3f;
        dashCooldown = 0.75f;   
        readyToDash = true;
        groundDrag = 7f;

        jumpForce = 11f;
        jumpCooldown = 0.25f;
        airMultiplier = 0.4f;

        crouchSpeed = 3.5f;
        crouchYScale = 0.5f;

        maxSlopeAngle = 55f;

        playerHeight = 2f;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        wallRunning = GetComponent<WallRunning>();

        audioSource = GetComponent<AudioSource>();

        readyToJump = true;
        readyToDoubleJump = true;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (uiController.countdown <= 0 || !won)
        {
            isOnGround = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

            MyInput();
            SpeedControl();
            StateHandler();

            if (wallrunning)
            {
                if (!readyToDoubleJump)
                {
                    ResetDoubleJump();
                }
            }
            if (isOnGround)
            {
                rb.drag = groundDrag;
                if (!readyToDoubleJump)
                {
                    ResetDoubleJump();
                }
            }
            else
            {
                rb.drag = 0;
            }
        }
    }

    void FixedUpdate()
    {
        if (uiController.countdown <= 0 || !won)
        {
            MovePlayer();
        }
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(dashKey) && readyToDash)
        {
            readyToDash = false;

            Dash();

            Invoke(nameof(ResetDash), dashCooldown);
        }

        if (Input.GetKeyDown(jumpKey) && readyToJump && isOnGround && !wallrunning)
        {
            readyToJump = false;

            Jump(true);

            Invoke(nameof(ResetJump),jumpCooldown);
        }
        else if(Input.GetKeyDown(jumpKey) && readyToDoubleJump && !isOnGround && !wallrunning)
        {
            readyToDoubleJump = false;

            Jump(false);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    void StateHandler()
    {
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }

        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }

        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if(isOnGround && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (isOnGround)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }

        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4 && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }
            yield return null;
        }
        moveSpeed = desiredMoveSpeed;
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f,ForceMode.Force);
            }
        }

        if (isOnGround)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isOnGround)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        if (!wallrunning)
        {
            rb.useGravity = !OnSlope();
        }
    }

    void SpeedControl()
    {
        if(rb.velocity.y > maxYVelocity)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYVelocity, rb.velocity.z);
        }

        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    void Jump(bool isNormalJump)
    {
        if (isNormalJump)
        {
            exitingSlope = true;
        }

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        if (!isNormalJump)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    void ResetDoubleJump()
    {
        readyToDoubleJump = true;
    }

    void Dash()
    {
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection.normalized) * dashForce * 10f, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * dashForce * 100f, ForceMode.Impulse);
        }

        if (moveDirection != Vector3.zero)
        {
            audioSource.PlayOneShot(dashSound);
        }
    }

    void ResetDash()
    {
        readyToDash = true;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Limit")
        {
            transform.position = new Vector3(0, 0, 0);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Meta")
        {
            won = true;

            for(int i = 0; i <= 5;i++)
            {
                GameObject clon = Instantiate(confeti, this.transform.position + new Vector3(Random.Range(-5f, 5f), -1f, Random.Range(-5f, 5f)), Quaternion.Euler(-90,0,0));

                Destroy(clon, 5f);
            }

            audioSource.PlayOneShot(winSound);
            Debug.Log("Ganaste");
        }
    }
}
