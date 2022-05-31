using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    SphereCollider groundCollider;
    int hasJump;
    public int maxJump = 1;
    public GameObject camara;
    public float speed = 20f;
    public float maxSpeed;
    public float jumpForce = 100f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundCollider = GetComponentInChildren<SphereCollider>();
        hasJump = maxJump;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(Vector3.forward * speed * Time.deltaTime, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.S)){
            rb.AddForce(Vector3.back * speed * Time.deltaTime, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector3.left * speed * Time.deltaTime, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector3.right * speed * Time.deltaTime, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.Space) && hasJump > 0)
        {
            rb.AddForce(Vector3.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
            hasJump--;
        }
        
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Ground")
        {
            hasJump = maxJump;
        }
    }
}
