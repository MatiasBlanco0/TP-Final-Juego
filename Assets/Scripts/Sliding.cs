using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    Rigidbody rb;
    PlayerController playerController;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    float slideTimer;

    public float slideYScale;
    float startYScale;

    [Header("Inputs")]
    public KeyCode slideKey = KeyCode.LeftControl;
    float horizontalInput;
    float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();

        startYScale = playerObj.localScale.y;

        maxSlideTime = 0.75f;
        slideForce = 200;
        slideYScale = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController.won)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            {
                StartSlide();
            }

            if (Input.GetKeyUp(slideKey) && playerController.sliding)
            {
                StopSlide();
            }
        }
    }

    void FixedUpdate()
    {
        if (playerController.sliding)
        {
            SlidingMovement();
        }
    }

    void StartSlide()
    {
        playerController.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(!playerController.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(playerController.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }

    void StopSlide()
    {
        playerController.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
