using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject levelSelector;
    public GameObject menu;

    void Start()
    {
        levelSelector.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        levelSelector.SetActive(true);
        menu.SetActive(false);
    }

    public void Back()
    {
        menu.SetActive(true);
        levelSelector.SetActive(false);
    }

    public void LoadLevel(string lvl)
    {
        SceneManager.LoadScene(lvl);
    }
}
