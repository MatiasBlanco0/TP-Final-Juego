using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text txtSpeed;
    public Text txtTimer;
    public int resolucion;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Multiplico la velocidad por la resolucion, hago floor y despues lo divido por la resolucion,
        // eliminando algunos numeros decimales que no son necesarios
        txtSpeed.text = ((Mathf.Floor(rb.velocity.magnitude * resolucion) / resolucion)).ToString();

        txtTimer.text = (Mathf.Floor(Time.time)).ToString();
    }
}
