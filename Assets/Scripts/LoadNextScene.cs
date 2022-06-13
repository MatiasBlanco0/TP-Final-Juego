using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public PlayerController playerController;
    float timer = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.won)
        {
            if(timer <= 0)
            {
                string sceneName = SceneManager.GetActiveScene().name;
                Cursor.lockState = CursorLockMode.None;
                if (sceneName == "Sandbox")
                {
                    SceneManager.LoadScene("Menu");
                }
                else
                {
                    int lvlNum = int.Parse(sceneName[6].ToString());

                    string nextLVLName = sceneName.Substring(0, 5) + lvlNum.ToString();

                    SceneManager.LoadScene(nextLVLName);
                }
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
