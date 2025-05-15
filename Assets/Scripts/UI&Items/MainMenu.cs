using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string overworld;
    public GameObject MenuPanel;
    public GameObject OptionsPanel;
    public GameObject go_transitions;
    public Animator anim_transitions; 

    public void StartGame()
    {
        anim_transitions.SetTrigger("ExitTriggered");
        Debug.Log("loading level");
        StartCoroutine(EnterLevel());

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
    



    IEnumerator EnterLevel()
    {

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(overworld);
    }
}
