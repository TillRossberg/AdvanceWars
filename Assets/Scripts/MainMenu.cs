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

    //UI
    public Canvas mainCanvas;
    public Canvas multiplayerCanvas;
    public GameObject multiplayerMapSelect;
    public GameObject playerSelect;
    public GameObject multiplayerOptions;
    public Canvas optionsCanvas;
    public Canvas levelSelectCanvas;
    public Text moneyIncreaseText;
    public Text battleDurationText;
    public Text propertiesToWinText;

    private void Start()
    {
        levelManager = GameObject.Find("LevelManager");
        container = GameObject.FindGameObjectWithTag("Container").GetComponent<Container>();

        mainCanvas.gameObject.SetActive(true);
        optionsCanvas.gameObject.SetActive(false);
        levelSelectCanvas.gameObject.SetActive(false);
        multiplayerCanvas.gameObject.SetActive(false);

        initDropdownMenus();
    }

    //Fills the dropdown menus with data.
    private void initDropdownMenus()
    {
        multiplayerCanvas.gameObject.SetActive(true);
        setupLevelSelectionDropdown();
        setupWeatherSelectionDropdown();
        multiplayerCanvas.gameObject.SetActive(false);
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

    //Next-buttons: when pressed get to the NEXT fitting menu level.
    public void goToMenuLevel(int levelIndex)
    {
        switch(levelIndex)
        {
            case 1://Multiplayer: set map
                multiplayerMapSelect.gameObject.SetActive(true);
                playerSelect.gameObject.SetActive(false);
                multiplayerOptions.SetActive(false);
                break;
            case 2://Multiplayer: set players
                multiplayerMapSelect.gameObject.SetActive(false);
                playerSelect.gameObject.SetActive(true);
                multiplayerOptions.SetActive(false);
                break;
            case 3://Multiplayer: set options
                multiplayerMapSelect.gameObject.SetActive(false);
                playerSelect.gameObject.SetActive(false);
                multiplayerOptions.SetActive(true);
                break;

            default:
                Debug.Log("MainMenu: Menu level not found!");
                break;
        }
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
        SceneManager.LoadScene("Battlefield");
    }

    public void level02Button()
    {
        container.setNextLevel(1);
        SceneManager.LoadScene("Battlefield");
    }

    //Multiplayer
    public void multiplayerButton()
    {
        multiplayerCanvas.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false);        
        multiplayerMapSelect.gameObject.SetActive(true);
        playerSelect.gameObject.SetActive(false);
        multiplayerOptions.SetActive(false);
    }

    public void startGameButton()
    {
        container.setNextLevel(container.getNextLevel());
        SceneManager.LoadScene("Battlefield");
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
