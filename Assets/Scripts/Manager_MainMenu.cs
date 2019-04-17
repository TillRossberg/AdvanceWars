//created by Till Roßberg, 2017-18
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager_MainMenu : MonoBehaviour
{
    //Data structures
    private Container container;
    //References
    public Canvas canvas;
    public RectTransform mainMenu; 

    private void Start()
    {
       
    }

    public Container getContainer()
    {
        return container;
    }

    public void level01Button()
    {
        container.setNextLevel(0);
        SceneManager.LoadScene("Battlefield");
    }

    public void level02Button()
    {
        container.setNextLevel(1);
        SceneManager.LoadScene("Battlefield");
    }

    public void startGameButton()
    {
        SceneManager.LoadScene("Battlefield");
    }
    //Exit
    public void exitGameButton()
    {
        Application.Quit();
    }
}
