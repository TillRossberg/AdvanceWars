using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller_MarkingCursor : MonoBehaviour
{
    //References
    public Manager _manager;
    public MapCreator _mapCreator;

    public EventSystem eventSystem;
    public GameObject selectedObject;
    //Fields
    private float inputDelay = 0.145f;
    private bool horAxisInUse;
    private bool vertAxisInUse;
    private int xPos = 0;
    private int yPos = 0;
    private bool buttonSelected = false;
    public List<Mesh> cursorMeshes = new List<Mesh>();
    private int enemyIndex = 0;

    public void init(int x, int y)
    {
        _manager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Manager>();
        _mapCreator = _manager.getMapCreator();
        xPos = x;
        yPos = y;
        this.transform.position = new Vector3(x, 0, y);
    }

    private void Update()
    {
        //Movement
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            if(!vertAxisInUse)
            {
                vertAxisInUse = true;
                goTo(xPos, yPos + 1);                    
                resetAxisInUseDelayed();
            }
        }
        else
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (!vertAxisInUse)
            {
                vertAxisInUse = true;
                goTo(xPos, yPos - 1);
                resetAxisInUseDelayed();
            }
        }

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            if (!horAxisInUse)
            {
                horAxisInUse = true;
                goTo(xPos - 1, yPos);
                resetAxisInUseDelayed();
            }
        }
        else
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (!horAxisInUse)
            {
                horAxisInUse = true;                
                goTo(xPos + 1, yPos);                
                resetAxisInUseDelayed();
            }
        }
        //Action Button
        if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            Unit unitHere;
            switch (_manager.getGameFunctions().getCurrentMode())
            {
                case GameFunctions.mode.normal:
                    if (_mapCreator.getTile(xPos, yPos).getUnitHere() != null)
                    {
                         unitHere = _mapCreator.getTile(xPos, yPos).getUnitHere().GetComponent<Unit>();
                        //Select own unit
                        if (_manager.getTurnManager().getActiveTeam().isInMyTeam(unitHere) && unitHere.getCanMove())
                        {
                            _manager.getGameFunctions().selectUnit(unitHere);                           
                        }
                        else
                        {
                            //Play dörp sound, unit has aleady moved
                        }
                        //Show range of enemy unit
                        if(!_manager.getTurnManager().getActiveTeam().isInMyTeam(unitHere))
                        {
                            _manager.getGameFunctions().selectEnemyUnit(unitHere);
                            unitHere.displayAttackableTiles(true);
                        }
                    }
                    break;
                case GameFunctions.mode.fire:
                    Unit attacker = _manager.getGameFunctions().getSelectedUnit();
                    Unit defender = _manager.getGameFunctions().getSelectedUnit().attackableUnits[enemyIndex].GetComponent<Unit>();
                    //Align the units to face each other.
                    attacker.alignUnit(this.xPos, this.yPos);
                    defender.alignUnit(attacker.xPos, attacker.yPos);
                    //Puts the health indicator in the right position.
                    attacker.displayHealth(true);
                    defender.displayHealth(true);

                    _manager.getBattleMode().fight(attacker, defender);//Battle
                    //Resets
                    //TODO: reset the cursor graphic and position to normal and the attacker after the fighting animation
                    setCursorPosition(attacker.xPos, attacker.yPos);
                    setCursorGfx(0);
                    enemyIndex = 0;
                    _manager.getGameFunctions().deselectObject();//Deselect the current unit
                    break;
                case GameFunctions.mode.move:
                    if (_mapCreator.getTile(xPos, yPos).getUnitHere() != null)
                    {
                        unitHere = _mapCreator.getTile(xPos, yPos).getUnitHere().GetComponent<Unit>();
                        if (unitHere.isSelected)//If you click on yourself.
                        {
                            _manager.getContextMenu().open(xPos, yPos);
                        }
                        else if(_manager.getTurnManager().getActiveTeam().isInMyTeam(unitHere))//If you click on a friendly unit
                        {
                            //Try to unite units
                        }
                        else//If you click on an enemy.
                        {
                            //Play dörp sound, you cant go there
                        }
                    }
                    else
                    {
                        _manager.getGameFunctions().getSelectedUnit().moveUnitTo(xPos, yPos);                        
                    }
                    break;
                case GameFunctions.mode.menu:
                    //apply choice
                    break;
            }
        }

        //Cancel button
        if (Input.GetButtonDown("Cancel"))
        {
            //displayCursorGfx(true); reactivate later
            buttonSelected = false;
            if(_manager.getGameFunctions().getSelectedUnit() != null)
            {
                _manager.getGameFunctions().getSelectedUnit().resetPosition();
            }
            _manager.getGameFunctions().deselectObject();
        }
    }

    private void goTo(int x, int y)
    {
        if(_manager.getMapCreator().isInsideGraph(x,y))
        {
            switch (_manager.getGameFunctions().getCurrentMode())
            {
                case GameFunctions.mode.normal:
                    setCursorPosition(x, y);
                    break;
                case GameFunctions.mode.fire:
                    //cycle through the attackable enemies
                    cycleAttackableEnemies();
                    break;
                case GameFunctions.mode.move:
                    //Draws an Arrow on the tile, if it is reachable
                    Tile tile = _manager.getMapCreator().getTile(x, y);
                    if (tile.isReachable && !tile.isPartOfArrowPath)
                    {
                        //We wont move outside the bounds of the reachable area.
                        setCursorPosition(x, y);
                        _manager.getArrowBuilder().createArrowPath(tile);
                    }
                    //If you go back, make the arrow smaller.
                    if (tile.isPartOfArrowPath)
                    {
                        //We wont move outside the bounds of the reachable area.
                        setCursorPosition(x, y);
                        _manager.getStatusWindow().updateStatusPanel(x, y);
                        _manager.getArrowBuilder().tryToGoBack(tile);
                    }
                    break;
                case GameFunctions.mode.menu:
                    
                    //navigate menu
                    break;
                default:
                    Debug.Log("Controller_MarkingCursor: goTo: mode not implemented yet!");
                    break;
            }
        }
    }
    //Goes through the list of enemies and positions the attack cursor over them.
    //TODO: find a way to move through the enemies depending on their position (above, below and so on)
    public void cycleAttackableEnemies()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Vertical") > 0)
        {
            enemyIndex--;
            if(enemyIndex < 0)
            {
                enemyIndex = _manager.getGameFunctions().getSelectedUnit().attackableUnits.Count - 1;
            }
            int x = _manager.getGameFunctions().getSelectedUnit().attackableUnits[enemyIndex].xPos;
            int y = _manager.getGameFunctions().getSelectedUnit().attackableUnits[enemyIndex].yPos;

            setCursorPosition(x, y);
        }
        else
        if (Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("Vertical") < 0)
        {
            enemyIndex++;
            if (enemyIndex > _manager.getGameFunctions().getSelectedUnit().attackableUnits.Count - 1)
            {
                enemyIndex = 0;
            }
            int x = _manager.getGameFunctions().getSelectedUnit().attackableUnits[enemyIndex].xPos;
            int y = _manager.getGameFunctions().getSelectedUnit().attackableUnits[enemyIndex].yPos;

            setCursorPosition(x, y);
        }
    }

    public void setCursorPosition(int x, int y)
    {
        this.transform.position = new Vector3(_mapCreator.getTile(x, y).transform.position.x, 0, _mapCreator.getTile(x, y).transform.position.z);
        xPos = x;
        yPos = y;
        _manager.getStatusWindow().updateStatusPanel(x, y);
    }
    private void resetAxisInUse()
    {
        horAxisInUse = false;
        vertAxisInUse = false;
    }
    
    private void resetAxisInUseDelayed()
    {
        Invoke("resetAxisInUse", inputDelay);
    }

    private void displayCursorGfx(bool value)
    {
        this.GetComponent<MeshRenderer>().enabled = value;
    }

    public void setCursorGfx(int value)
    {
        this.GetComponent<MeshFilter>().mesh = cursorMeshes[value];        
    }
}
