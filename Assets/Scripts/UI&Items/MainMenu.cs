using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string overworld;
    public GameObject MenuPanel;
    public GameObject OptionsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(overworld);
        Debug.Log("loading level");

    }

    public void OptionsOpen()
    {
        MenuPanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    public void OptionsClose()
    {
        MenuPanel.SetActive(true);
        OptionsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("exiting game");
    }

    public void Fullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    

}
