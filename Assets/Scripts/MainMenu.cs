//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Data structures
    Container container;
    GameObject levelManager;

    public Canvas mainCanvas;
    public Canvas multiplayerCanvas;
    public Canvas optionsCanvas;
    public Canvas levelSelectCanvas;
    public Text moneyIncreaseText;
    public Text battleDurationText;
    public Text propertiesToWinText;

    private void Awake()
    {
        //TODO: only acitvate main Canvas
        optionsCanvas.gameObject.SetActive(false);
        levelSelectCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        levelManager = GameObject.Find("LevelManager");
        container = GameObject.FindGameObjectWithTag("Container").GetComponent<Container>();
        //Find a better place to do this
        setupLevelSelectionDropdown();
        setupWeatherSelectionDropdown();
    }

    //Sets up the dropdown list for the level selection.
    public void setupLevelSelectionDropdown()
    {        
        GameObject.Find("LevelDropdown").GetComponent<Dropdown>().AddOptions(levelManager.GetComponent<Database>().getLevels());//Get the available levels from the database.
        GameObject.Find("LevelDropdown").GetComponent<Dropdown>().captionText.text = "Select Level...";//Add title to the level select drop down menu.
    }

    //Sets up the dropdown list for the weather selection.
    public void setupWeatherSelectionDropdown()
    {
        GameObject.Find("WeatherDropdown").GetComponent<Dropdown>().AddOptions(levelManager.GetComponent<Database>().getWeatherOptions());//Get the available weather types from the database.
        GameObject.Find("WeatherDropdown").GetComponent<Dropdown>().captionText.text = "Select Weather...";//Add title to the weather select drop down menu.
    }

    //Play
    public void playButton()
    {
        mainCanvas.gameObject.SetActive(false);
        levelSelectCanvas.gameObject.SetActive(true);
    }

    

    public void level01Button()
    {
        container.setNextLevel(0);
        SceneManager.LoadScene("Scene01");
    }

    public void level02Button()
    {
        container.setNextLevel(1);
        SceneManager.LoadScene("Scene01");
    }

    //Multiplayer
    public void multiplayerButton()
    {
        multiplayerCanvas.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false);
    }
    //Game Options
    public void setLevel(int value)
    {
        container.setNextLevel(value);
        Debug.Log("Level set to: " + value);
    }

    public void setFogOfWar(bool value)
    {
        container.setFogOfWar(value);
    }

    public void setWeather(int value)
    {
        container.setWeather(value);        
    }

    public void setMoneyIncrease(float value)
    {
        int myMoneyIncrease = (int)(value * 500 + 1000);
        moneyIncreaseText.text = myMoneyIncrease.ToString() + " $";
        container.setMoneyIncrement(myMoneyIncrease);
    }
    public void setBattleDuration(float value)
    {
        container.setBattleDuration(value);

        if (value == 4)
        {
            battleDurationText.text = "Duration of battle: OFF";
        }
        else
        {
            battleDurationText.text = "Duration of battle: " + value + " days";
        }
    }
    public void setPropertiesToWin(float value)
    {
        container.setPropertiesToWin(value);
        
        if (value == 11)
        {
            propertiesToWinText.text = "Properties to Win: OFF";
        }
        else
        {
            propertiesToWinText.text = "Properties to Win: " + value;
        }
    }
    public void setAbility(bool value)
    {
        container.setAbility(value);
    }

    //Tech options
    public void optionsButton()
    {
        optionsCanvas.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false);
    }

    public void backToMainMenuButton()
    {
        optionsCanvas.gameObject.SetActive(false);
        multiplayerCanvas.gameObject.SetActive(false);
        levelSelectCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    }

    //Exit
    public void exitGameButton()
    {
        Application.Quit();
    }

    
}
