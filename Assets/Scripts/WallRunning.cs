using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    float wallRunTimer;

    bool upwardsRunning;
    bool downwardsRunning;
    float horizontalInput;
    float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    RaycastHit leftWallHit;
    RaycastHit rightWallHit;
    bool wallLeft;
    bool wallRight;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    float exitWallTimer;

    [Header("Gravitiy")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("Input")]
    public KeyCode upwardsRunningKey = KeyCode.LeftShift;
    public KeyCode downwardsRunningKey = KeyCode.LeftControl;

    [Header("References")]
    public Transform orientation;
    PlayerController playerController;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();

        wallRunForce = 200f;
        maxWallRunTime = 1f;
        wallCheckDistance = 0.7f;
        minJumpHeight = 2;
        wallClimbSpeed = 3;
        wallJumpUpForce = 7;
        wallJumpSideForce = 12;
        exitWallTime = 0.2f;
        useGravity = true;
        gravityCounterForce = 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForWall();
        StateMachine();
    }

    void FixedUpdate()
    {
        if (playerController.wallrunning)
        {
            WallRunningMovement();
        }
    }

    void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunningKey);
        downwardsRunning = Input.GetKey(downwardsRunningKey);

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!playerController.wallrunning)
            {
                StartWallRun();
            }

            if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if(wallRunTimer <= 0 && playerController.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (Input.GetKeyDown(playerController.jumpKey))
            {
                WallJump();
            }
        }
        else if (exitingWall)
        {
            if (playerController.wallrunning)
            {
                StopWallRun();
            }

            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if(exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        else
        {
            if (playerController.wallrunning)
            {
                StopWallRun();
            }
        }
    }

    void StartWallRun()
    {
        playerController.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward + wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }
        if (downwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }

        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    void StopWallRun()
    {
        playerController.wallrunning = false;
    }

    void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
