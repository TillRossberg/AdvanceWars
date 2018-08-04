using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu_Context : MonoBehaviour
{
    //References
    private Manager _manager;
    public EventSystem eventSystem;

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

    public bool showingAttackableTiles = false;
    public bool showingReachableTiles = false;
    
    public void init()
    {
        _manager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Manager>();
        contextMenu.gameObject.SetActive(false);
    }
	

    //Opens the specific context menu at the given position.
    public void openContextMenu(int menuType)
    {
        _manager.getGameFunctions().setCurrentMode(GameFunctions.mode.menu);
        eventSystem.SetSelectedGameObject(null);
        Invoke("highlightFirstMenuButton", 0.01f);
        contextMenu.gameObject.SetActive(true);
        setMenuType(menuType);
        isOpened = true;
        //Store where the user clicked on, for dynamic positioning.
    }


    public void open(int xPos, int yPos)
    {
        Unit unitHere = _manager.getMapCreator().getTile(xPos, yPos).getUnitHere().GetComponent<Unit>();
        //Decide if the menu with firebutton and wait button is opened ...
        if (_manager.getGameFunctions().getSelectedUnit().attackableUnits.Count > 0 && _manager.getGameFunctions().getSelectedUnit().canFire)
        {
            //...if the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
            if (_manager.getMapCreator().getTile(xPos, yPos).isOccupyable(unitHere))
            {
               openContextMenu(3);
            }
            else
            {
                openContextMenu(1);
            }
        }
        //...OR if only the wait button is to display.
        else
        {
            //If the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
            if (_manager.getMapCreator().getTile(xPos, yPos).isOccupyable(unitHere))
            {
                openContextMenu(2);
            }
            else
            {
                openContextMenu(0);
            }
        }
    }

    //Postitions the context menu, where it is not visible.
    public void resetContextMenu()
    {
        deactivateAllButtons();
        contextMenu.gameObject.SetActive(false);
    }

    //Close the menu.
    public void closeMenu()
    {
        resetContextMenu();
        isOpened = false;
        showingAttackableTiles = false;
    }
    
    //Activate the fire mode, show the enemies that can be attacked and close the menu.
    public void Button_Fire()
    {
        Invoke("Button_FireDelayed", 0.01f);//Unity is a bit too fast :)
    }

    private void Button_FireDelayed()
    {
        _manager.getGameFunctions().setCurrentMode(GameFunctions.mode.fire);
        _manager.getMapCreator().showReachableTiles(false);
        _manager.getGameFunctions().getSelectedUnit().showAttackableEnemies();
        //Set cursor gfx to attack 
        _manager.getCursor().setCursorGfx(1);
        //Set cursor position to the first attackable unit
        int x = _manager.getGameFunctions().getSelectedUnit().attackableUnits[0].xPos;
        int y = _manager.getGameFunctions().getSelectedUnit().attackableUnits[0].yPos;
        _manager.getCursor().setCursorPosition(x, y);

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
        _manager.getMapCreator().getTile(selectedUnit.xPos, selectedUnit.yPos).occupy(selectedUnit.getHealthAsInt());//The take over action.
        selectedUnit.canFire = false;
        selectedUnit.hasTurn = false;
        _manager.getTurnManager().updateFogOfWar(selectedUnit.myTeam);
        _manager.getGameFunctions().deselectObject();
    }

    //Toggle the visiblity of the attackable tiles of the unit.
    public void Button_Range()
    {
        //if(_manager.getGameFunctions().getSelectedUnit().transform.Find("attackableTiles").transform.childCount < 1)
        //{
        //    Debug.Log("attackable tiles empty");
        //    _manager.getGameFunctions().getSelectedUnit().createAttackableTiles();
        //}
        if (!showingAttackableTiles)
        {
            _manager.getGameFunctions().getSelectedUnit().displayAttackableTiles(true);
            showingAttackableTiles = true;
        }
        else
        {
            _manager.getGameFunctions().getSelectedUnit().displayAttackableTiles(false);
            showingAttackableTiles = false;           
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

    public Transform getFirstActiveButton()
    {
        Transform buttons = GameObject.Find("Buttons").transform;
        foreach (Transform child in buttons)
        {
            if(child.gameObject.activeSelf)
            {
                return child;                
            }            
        }
        Debug.Log("menu cotext: getfirstactivebutton: no active button found!");
        return null;
    }
    private void highlightFirstMenuButton()
    {
        eventSystem.SetSelectedGameObject(getFirstActiveButton().gameObject);
    }
}
