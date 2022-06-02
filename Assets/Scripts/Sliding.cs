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
    public float maxSlideTimer;
    public float slideForce;
    float slideTimer;

    public float slideYScale;
    float startYScale;

    public float horizontalInput;
    public float verticalInput;

    bool sliding;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();

        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftControl) && horizontalInput != 0 || verticalInput != 0)
        {
            StartSlide();
        }

        if(Input.GetKeyUp(KeyCode.LeftControl) && sliding)
        {
            StopSlide();
        }
    }

    void StartSlide()
    {
        sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
    }

    void SlidingMovement()
    {

    }

    void StopSlide()
    {

    }
}
