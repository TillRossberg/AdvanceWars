using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Menu_MultiPlayerOptions : MonoBehaviour
{
    //Required datastructures
    private Transform SceneManager;
    private Container container;
    //References    
    public RectTransform mapSelection;

    public RectTransform playerSelection;
    public RectTransform playerOnePanel;
    public RectTransform playerTwoPanel;

    public RectTransform options;    
    public RectTransform moneyIncreaseText;
    public RectTransform battleDurationText;
    public RectTransform propertiesToWinText;
    //Dropdowns
    public RectTransform weatherDropdown;
    public RectTransform mapSelectDropdown;

    //Fields
    public enum menuType { multiplayer_setMap, multiplayer_setPlayers, multiplayer_setOptions };

    public void init(Transform sceneManager)
    {        
        this.SceneManager = sceneManager;
        container = sceneManager.GetComponent<Menu_Master>().getContainer();
        initDropdownMenus();
    }

    //Fills the dropdown menus with data.
    private void initDropdownMenus()
    {
        setupWeatherSelectionDropdown();
        setupLevelSelectionDropdown();
        setupGeneralSelectionDropdown();
    }
        
    //Sets up the dropdown list for the weather selection.
    private void setupWeatherSelectionDropdown()
    {
        weatherDropdown.GetComponent<Dropdown>().AddOptions(SceneManager.GetComponent<Database>().getWeatherNames());//Get the available weather types from the database.
        weatherDropdown.GetComponent<Dropdown>().captionText.text = "Select Weather...";//Add title to the weather select drop down menu.
    }
    private void setupLevelSelectionDropdown()
    {
        mapSelectDropdown.GetComponent<Dropdown>().AddOptions(SceneManager.GetComponent<Database>().getLevelNames());
        mapSelectDropdown.GetComponent<Dropdown>().captionText.text = "Select Map...";
    }
    private void setupGeneralSelectionDropdown()
    {
        playerOnePanel.Find("CommanderSelectionDropdown").GetComponent<Dropdown>().AddOptions(SceneManager.GetComponent<Database>().getCommanderNames());
        playerTwoPanel.Find("CommanderSelectionDropdown").GetComponent<Dropdown>().AddOptions(SceneManager.GetComponent<Database>().getCommanderNames());
        playerTwoPanel.Find("CommanderSelectionDropdown").GetComponent<Dropdown>().value = 2;
    }

    public void displayMenu(int index)
    {
        switch (index)
        {
            case 0:
                displayMenu(menuType.multiplayer_setMap);
                break;
            case 1:
                displayMenu(menuType.multiplayer_setPlayers);
                break;
            case 2:
                displayMenu(menuType.multiplayer_setOptions);
                break;            
            default:
                Debug.Log("Menu_MultiPlayerOptions: displayMenu: no such index found!");
                break;
        }
    }
    public void displayMenu(menuType type)
    {
        hideAllMenus();
        getMenu(type).gameObject.SetActive(true);
    }

    public void hideAllMenus()
    {
        foreach (menuType type in Enum.GetValues(typeof(menuType)))
        {
            getMenu(type).gameObject.SetActive(false);
        }
    }

    public RectTransform getMenu(menuType myMenuType)
    {
        switch (myMenuType)
        {            
            case menuType.multiplayer_setMap: return mapSelection;
            case menuType.multiplayer_setPlayers: return playerSelection;
            case menuType.multiplayer_setOptions: return options;
            default: Debug.Log("MenuMaster: getMenu: menuType missing!"); return null;
        }
    }
    //Player selection
    public void setCommanderTeamOne(int index)
    {
        container.setCommanderPlayerOne(index);
        updateCommanderPics();
    }

    public void setCommanderTeamTwo(int index)
    {
        container.setCommanderPlayerTwo(index);
        updateCommanderPics();
    }

    public void updateCommanderPics()
    {
        playerOnePanel.Find("CommanderPic").GetComponent<Image>().sprite = SceneManager.GetComponent<Database>().getCommanderThumb(container.getTeamCommander(0));
        playerTwoPanel.Find("CommanderPic").GetComponent<Image>().sprite = SceneManager.GetComponent<Database>().getCommanderThumb(container.getTeamCommander(1));
    }

    public void setTeamColors()
    {
        container.setTeamColor(0, playerOnePanel.Find("Picker/ColorField/Color/Fill").GetComponent<Image>().color);
        container.setTeamColor(1, playerTwoPanel.Find("Picker/ColorField/Color/Fill").GetComponent<Image>().color);
    }

    public void setTeamNamePlayerOne(string name)
    {
        container.setTeamName(0, name);
    }

    public void setTeamNamePlayerTwo(string name)
    {
        container.setTeamName(1, name);
    }

    //Game Options
    public void setLevel(int value)
    {
        container.setNextLevel(value);
    }

    public void setWeather(int value)
    {
        container.setWeather(value);
    }

    public void setFogOfWar(bool value)
    {
        container.setFogOfWar(value);
    }

    public void setAbility(bool value)
    {
        container.setAbility(value);
    }

    public void setMoneyIncrease(float value)
    {
        int myMoneyIncrease = (int)(value * 500 + 1000);
        moneyIncreaseText.GetComponent<Text>().text = myMoneyIncrease.ToString() + " $";
        container.setMoneyIncrement(myMoneyIncrease);
    }

    public void setBattleDuration(float value)
    {
        container.setBattleDuration(value);

        if (value == 4)
        {
            battleDurationText.GetComponent<Text>().text = "Duration of battle: OFF";
        }
        else
        {
            battleDurationText.GetComponent<Text>().text = "Duration of battle: " + value + " days";
        }
    }

    public void setPropertiesToWin(float value)
    {
        container.setPropertiesToWin(value);

        if (value == 11)
        {
            propertiesToWinText.GetComponent<Text>().text = "Properties to Win: OFF";
        }
        else
        {
            propertiesToWinText.GetComponent<Text>().text = "Properties to Win: " + value;
        }
    }
}
