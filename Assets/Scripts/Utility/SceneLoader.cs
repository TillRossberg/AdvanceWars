using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	// Use this for initialization
	void Start () {
		
	}

    public void loadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }


    //Loads the screen that shows who won and displays the statistics. Delay is required, so we see the destruction of the last unit.
    public void loadGameFinishedScreenWithDelay()
    {
        Invoke("loadGameFinishedScreen", 2);
        
    }
    public void loadGameFinishedScreen()
    {
        loadLevel("GameFinishedScreen");
    }


}
