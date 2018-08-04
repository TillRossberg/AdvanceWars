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
    public Manager_Team teamManager;
    public AudioManager audioManager;
    public GameFunctions gameFunctions;
    public UnitCreator unitCreator;
    public Manager_Turn turnManager;
    public BattleMode battleMode;
    public MapCreator mapCreator;
    public ArrowBuilder arrowBuilder;
    public Database dataBase;
    public AnimController animationController;
    public SceneLoader sceneLoader;
    //Prefabs
    public Transform cursor;

    //Fields

	// Use this for initialization
	void Start ()
    {
        int xStart = 12;
        int yStart = 6;
        initContainer();
        unitCreator.init();
        teamManager.init();
        battleMode.init();
        getBuyMenu().init();
        mapCreator.init();
        gameFunctions.init();
        teamManager.initTeamsFromContainer();
        gameFunctions.loadLevel(container.getNextLevelIndex());
        turnManager.init();
        getContextMenu().init();
        getStatusWindow().init();
        getStatusWindow().displayCommanderInfo();
        cursor.GetComponent<Controller_MarkingCursor>().setCursorPosition(13, 7);
        getStatusWindow().updateStatusPanel(xStart, yStart);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private Transform createNewMarkingCursor(int x, int y)
    {
        Transform newCursor = Instantiate(cursor, this.transform);
        newCursor.GetComponent<Controller_MarkingCursor>().init(x,y);        
        return newCursor;
    }

    //Check if the data container with a game setup created in the main menu is existing and return it. If not, we create a default container.
    private void initContainer()
    {
        if (GameObject.FindWithTag("Container") != null)
        {
            this.container = GameObject.FindWithTag("Container").GetComponent<Container>();
        }
        else
        {
            Debug.Log("MasterClass: No container found, loading default container!");
            Container container = Instantiate(containerPrefab).GetComponent<Container>();
            container.initTestContainer01();
            this.container = container;
        }
    }
    public Controller_MarkingCursor getCursor()
    {
        return cursor.GetComponent<Controller_MarkingCursor>();
    }

    public Container getContainer()
    {
        return container;
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

    public Panel_UnitStatus getStatusWindow()
    {
        return statusWindow.GetComponent<Panel_UnitStatus>();
    }

    public Manager_Team getTeamManager()
    {
        return teamManager;
    }

    public AudioManager getAudioManager()
    {
        return audioManager;
    }

    public GameFunctions getGameFunctions()
    {
        return gameFunctions;
    }

    public UnitCreator getUnitCreator()
    {
        return unitCreator;
    }

    public Manager_Turn getTurnManager()
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
