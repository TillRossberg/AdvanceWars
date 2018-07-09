//created by Till Roßberg, 2017-18
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    //Datastructures    
    public Container container;
    public GameObject containerPrefab;

    //References
    public Canvas mainCanvas;
    public RectTransform buyMenu;
    public RectTransform contextMenu;
    public RectTransform statusWindow;
    public TeamManager teamManager;
    public AudioManager audioManager;
    public GameFunctions mainFunctions;
    public UnitCreator unitCreator;
    public TurnManager turnManager;
    public BattleMode battleMode;
    public MapCreator mapCreator;
    public ArrowBuilder arrowBuilder;
    public Database dataBase;
    public AnimController animationController;
    public SceneLoader sceneLoader;


	// Use this for initialization
	void Start ()
    {
        container = getContainer();
        mapCreator.init();
        mainFunctions.init();
        teamManager.initTeamsFromContainer();
        mainFunctions.loadLevel(container.getNextLevelIndex());
        turnManager.init();
        getContextMenu().init();
        getStatusWindow().init();
        getStatusWindow().displayGeneralInfo();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //Check if the data container with a game setup created in the main menu is existing and return it. If not, we create a default container.
    public Container getContainer()
    {
        if (GameObject.FindWithTag("Container") != null)
        {
            return  GameObject.FindWithTag("Container").GetComponent<Container>();
        }
        else
        {
            Debug.Log("MasterClass: No container found, loading default container!");
            Container container = Instantiate(containerPrefab).GetComponent<Container>();
            container.initTestContainer01();
            return container;
        }
    }

    public Canvas getCanvas()
    {
        return mainCanvas;
    }

    public Menu_BuyUnits getBuyMenu()
    {
        return buyMenu.GetComponent<Menu_BuyUnits>();
    }

    public Menu_Context getContextMenu()
    {
        return contextMenu.GetComponent<Menu_Context>();
            
    }

    public StatusWindow getStatusWindow()
    {
        return statusWindow.GetComponent<StatusWindow>();
    }

    public TeamManager getTeamManager()
    {
        return teamManager;
    }

    public AudioManager getAudioManager()
    {
        return audioManager;
    }

    public GameFunctions getGameFunctions()
    {
        return mainFunctions;
    }

    public UnitCreator getUnitCreator()
    {
        return unitCreator;
    }

    public TurnManager getTurnManager()
    {
        return turnManager;
    }

    public BattleMode getBattleMode()
    {
        return battleMode;
    }

    public MapCreator getMapCreator()
    {
        return mapCreator;
    }

    public ArrowBuilder getArrowBuilder()
    {
        return arrowBuilder;
    }
        
    public Database getDatabase()
    {
        return dataBase;
    }
    public AnimController getAnimationController()
    {
        return animationController;
    }

    public SceneLoader getSceneLoader()
    {
        return sceneLoader;
    }
}
