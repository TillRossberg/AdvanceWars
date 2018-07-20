using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Context : MonoBehaviour
{
    private Manager _manager;
    private int height = 1;//Heigth above the playground.
    public RectTransform contextMenu;//Graphics
    public RectTransform waitButton;
    public RectTransform fireButton;
    public RectTransform rangeButton;
    public RectTransform tileInfoButton;
    public RectTransform endTurnButton;
    public RectTransform occupyButton;
    public Transform exclamationMark;
    public bool isOpened = false;
    public int clickedHereX;
    public int clickedHereY;

    public bool showAttackableTiles = false;
    public bool showReachableTiles = false;


	// Use this for initialization
	void Start ()
    {
        //contextMenu = GameObject.FindGameObjectWithTag("ContextMenu").GetComponent<Canvas>();
	}

    public void init()
    {
        _manager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Manager>();
        contextMenu.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //Opens the specific context menu at the given position.
    public void openContextMenu(int x, int y, int menuType)
    {
        contextMenu.gameObject.SetActive(true);
        setMenuType(menuType);
        isOpened = true;
        //Store where the user clicked on, for dynamic positioning.
        clickedHereX = x;
        clickedHereY = y;
    }

    //Postitions the context menu, where it is not visible.
    public void resetContextMenu()
    {
        deactivateAllButtons();
        contextMenu.gameObject.SetActive(false);
        clickedHereX = 0;
        clickedHereY = 0;
    }

    //Close the menu.
    public void closeMenu()
    {
        resetContextMenu();
        isOpened = false;
    }
    
    //Activate the fire mode, show the enemies that can be attacked and close the menu.
    public void Button_Fire()
    {
        _manager.getGameFunctions().setCurrentMode(GameFunctions.mode.fire);
        _manager.getMapCreator().showReachableTiles(false);
        _manager.getGameFunctions().getSelectedUnit().showAttackableEnemies();
        closeMenu();
    }

    //Wait here and perform no actions.
    public void Button_Wait()
    {
        _manager.getGameFunctions().getSelectedUnit().wait();         
    }

    //Perform the occupy action on a property.
    public void Button_Occupy()
    {
        Debug.Log("Occupying...");
        Unit selectedUnit = _manager.getGameFunctions().getSelectedUnit();
        selectedUnit.moveUnitTo(clickedHereX, clickedHereY);
        _manager.getMapCreator().getTile(selectedUnit.xPos, selectedUnit.yPos).occupy(selectedUnit.getHealthAsInt());//The take over action.
        selectedUnit.canFire = false;
        selectedUnit.hasTurn = false;
        _manager.getTurnManager().updateFogOfWar(selectedUnit.myTeam);
        _manager.getGameFunctions().deselectObject();
    }

    //Switch the visiblity of the attackable tiles of the unit.
    public void Button_Range()
    {
        if (showAttackableTiles)
        {
            _manager.getMapCreator().showAttackableTiles(false);
            showAttackableTiles = false;
        }
        else
        {
            //For direct attack
            if (_manager.getGameFunctions().getSelectedUnit().directAttack)
            {
                _manager.getMapCreator().showAttackableTiles(true);
                showAttackableTiles = true;
            }
            //For range attack
            if (_manager.getGameFunctions().getSelectedUnit().rangeAttack)
            {
                _manager.getMapCreator().showAttackableTiles(true);
                showAttackableTiles = true;
            }
        }
    }

    //End the turn for the active player and proceed to the next player in the succession.
    public void Button_EndTurn()
    {
        _manager.getTurnManager().endTurn();
    }

    public void Button_TestFunction()
    {
        Debug.Log("--Testing--");
        for (int i = 0; i < _manager.getTeamManager().getTeams().Count; i++)
        {
            Debug.Log(_manager.getTeamManager().getTeams()[i]);
        }
    }
   

    //Show the tile info.
    public void Button_TileInfo()
    {

    }
    //Creates an exclamation mark that indicates important things!
    //TODO: maybe find a better place for this.
    public void showExclamationMark(int x, int y)
    {
        Transform mark = Instantiate(exclamationMark, this.transform);
        mark.position = new Vector3(x, 1, y);
    }

    //Activates the different menu types.
    //TODO: Make the positions generic!
    public void setMenuType(int menuType)
    {
        switch(menuType)
        {
            //Wait button & range button
            case 0:
                deactivateAllButtons();
                waitButton.gameObject.SetActive(true);
                rangeButton.gameObject.SetActive(true);
                break;

            //Fire button & wait button & range button
            case 1:
                deactivateAllButtons();
                fireButton.gameObject.SetActive(true);
                waitButton.gameObject.SetActive(true);
                rangeButton.gameObject.SetActive(true);
                break;

            //Wait button & occupy button & range button
            case 2:
                deactivateAllButtons();
                waitButton.gameObject.SetActive(true);
                rangeButton.gameObject.SetActive(true);
                occupyButton.gameObject.SetActive(true);

                break;

            //Firebutton, waitbutton, occupy button & range button
            case 3:
                deactivateAllButtons();
                fireButton.gameObject.SetActive(true);
                waitButton.gameObject.SetActive(true);
                rangeButton.gameObject.SetActive(true);
                occupyButton.gameObject.SetActive(true);
                break;

            //Infobutton about a tile
            case 5:
                deactivateAllButtons();
                tileInfoButton.gameObject.SetActive(true);
                break;

            default:
                Debug.Log("Invalid menu type chosen!");
                break;
        }
    }

    //Sets all buttons to inactive.
    public void deactivateAllButtons()
    {
        fireButton.gameObject.SetActive(false);
        waitButton.gameObject.SetActive(false);
        rangeButton.gameObject.SetActive(false);
        tileInfoButton.gameObject.SetActive(false);
        occupyButton.gameObject.SetActive(false);
    }

    public void killAllEnemies()
    {
        List<Team> enemyTeams = _manager.getTurnManager().getActiveTeam().getEnemyTeams();
        for (int i = 0; i < enemyTeams.Count; i++)
        {
            for (int j = 0; j < enemyTeams[i].getUnits().Count; j++)
            {
                Unit unit = enemyTeams[i].getUnits()[j].GetComponent<Unit>();
                unit.killUnitDelayed();
            }            
        }
    }
}
