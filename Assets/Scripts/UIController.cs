using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    Text txtSpeed;
    Text txtTimer;
    public int resolucion;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        txtSpeed = GetComponentInChildren<Text>();
        txtTimer = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        txtSpeed.text = ((Mathf.Floor(rb.velocity.magnitude * resolucion) / resolucion)).ToString();
    }
}
