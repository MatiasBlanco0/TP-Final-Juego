using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject countdownObj;
    public Text txtCountdown;
    public float countdown = 3f;

    public GameObject hud;
    public Text txtSpeed;
    float timeElapssed = 0;
    public Text txtTimer;

    public int resolution;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        hud.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (hud.activeInHierarchy)
        {
            // Multiplico la velocidad por la resolucion, hago floor y despues lo divido por la resolucion,
            // eliminando algunos numeros decimales que no son necesarios
            txtSpeed.text = (Mathf.Floor(rb.velocity.magnitude * resolution) / resolution).ToString();

            txtTimer.text = Mathf.Floor(timeElapssed).ToString();

            timeElapssed += Time.deltaTime;
        }

        if(countdown <= 0)
        {
            hud.SetActive(true);
            countdownObj.SetActive(false);
        }

        if (countdownObj.activeInHierarchy)
        {
            countdown -= Time.deltaTime;

            txtCountdown.text = Mathf.Floor(countdown).ToString();
        }
    }
}
