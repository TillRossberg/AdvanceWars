using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Menu_Main : MonoBehaviour
{
    //Required datastructures
    private Transform SceneManager;

    //Refrences
    public RectTransform optionsMenu;
    public RectTransform singlePlayerMenu;
    public RectTransform multiPlayerMenu;

    //Fields
    public enum menuType { singlePlayer, multiPlayerOptions, options, mainMenu };


    public void init(Transform sceneManager)
    {
        this.SceneManager = sceneManager;
        multiPlayerMenu.GetComponent<Menu_MultiPlayerOptions>().init(SceneManager);
        optionsMenu.GetComponent<Menu_Options>().init(SceneManager);
    }

    public void displayMenu(menuType myMenuType)
    {
        hideAllMenus();
        getMenu(myMenuType).gameObject.SetActive(true);
    }

    public void displayMenu(int index)
    {
        switch (index)
        {
            case 0:
                displayMenu(menuType.mainMenu);
                break;
            case 1:
                displayMenu(menuType.options);
                break;
            case 2:
                displayMenu(menuType.multiPlayerOptions);
                break;
            case 3:
                displayMenu(menuType.singlePlayer);
                break;
            default:
                Debug.Log("Menu__Master: displayMenu: no such index found!");
                break;
        }
    }
    public RectTransform getMenu(menuType myMenuType)
    {
        switch (myMenuType)
        {
            case menuType.multiPlayerOptions: return multiPlayerMenu;
            case menuType.options: return optionsMenu;
            case menuType.mainMenu: return this.transform as RectTransform;
            case menuType.singlePlayer: return singlePlayerMenu;
            default: Debug.Log("MenuMaster: getMenu: menuType missing!"); return null;
        }
    }

    public void hideAllMenus()
    {
        foreach (menuType type in Enum.GetValues(typeof(menuType)))
        {
            getMenu(type).gameObject.SetActive(false);
        }
    }


}
