using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject btnNextLVL;

    public PlayerController playerController;
    float timer = 5f;

    // Start is called before the first frame update
    void Start()
    {
        winScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.won)
        {
            winScreen.SetActive(true);
        }
    }

    public void LoadNextLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Cursor.lockState = CursorLockMode.None;

        if(sceneName == "Sandbox")
        {
            btnNextLVL.SetActive(false);
        }
        else
        {
            int lvlNum = int.Parse(sceneName[6].ToString());

            string nextLVLName = sceneName.Substring(0, 5) + lvlNum.ToString();

            SceneManager.LoadScene(nextLVLName);
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
