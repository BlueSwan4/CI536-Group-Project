using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject InGamePanel;
    public string mMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true) {
            MenuPanel.SetActive(true);
            InGamePanel.SetActive(false);
            Time.timeScale = 0;
        };

    }

    public void Exit()
    {
        SceneManager.LoadScene(mMenu);
        Time.timeScale = 1;

    }

    public void Return()
    {
        MenuPanel.SetActive(false);
        InGamePanel.SetActive(true);
        Time.timeScale = 1;
    }
}
