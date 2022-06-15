using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadNextScene : MonoBehaviour
{
    public GameObject winScreen;
    public Text txtFinalTime;
    public GameObject btnNextLVL;
    public GameObject hud;

    public PlayerController playerController;
    public UIController uiController;

    string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        winScreen.SetActive(false);

        sceneName = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.won)
        {
            hud.SetActive(false);

            float miliseconds = (uiController.timeElapssed * 1000) - (Mathf.Floor(uiController.timeElapssed - (uiController.timeElapssed / 60)) * 1000) - ((uiController.timeElapssed / 60) * 1000);
            string text = string.Format("{0:0}:{1:00}:{2:000}", Mathf.Floor(uiController.timeElapssed / 60), Mathf.Floor(uiController.timeElapssed - (Mathf.Floor(uiController.timeElapssed / 60)) * 60), miliseconds);
            txtFinalTime.text = text;

            winScreen.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;

            if (sceneName == "Sandbox")
            {
                btnNextLVL.SetActive(false);
            }
        }
    }

    public void LoadNextLevel()
    {
        int lvlNum = int.Parse(sceneName[5].ToString()) + 1;

        string nextLVLName = sceneName.Substring(0, 5) + lvlNum.ToString();

        if (SceneUtility.GetBuildIndexByScenePath(nextLVLName) != -1)
        {
            SceneManager.LoadScene(nextLVLName);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
