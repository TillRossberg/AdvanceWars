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
    //

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
        //Selection
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
                        if (_manager.getTurnManager().getActiveTeam().isInMyTeam(unitHere) && !unitHere.getHasMoved())
                        {
                            _manager.getGameFunctions().selectUnit(unitHere);
                            //Calculate reachable area and instantiate the graphics for the tiles.
                            unitHere.calcReachableArea(xPos, yPos, unitHere.moveDist, unitHere.myMoveType, null);
                            //Debug.Log("Reachable iterations: " + counter);
                            _manager.getMapCreator().createReachableTiles();
                            unitHere.calcVisibleArea();
                            //displayCursorGfx(false); will be activated when tha a* allows us to draw a movement arrow to any given spot
                            _manager.getGameFunctions().setCurrentMode(GameFunctions.mode.move);
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
                    break;
                case GameFunctions.mode.move:
                    if (_mapCreator.getTile(xPos, yPos).getUnitHere() != null)
                    {
                        unitHere = _mapCreator.getTile(xPos, yPos).getUnitHere().GetComponent<Unit>();
                        if (unitHere.isSelected)//If you click on yourself.
                        {
                            _manager.getContextMenu().openContextMenu(xPos, yPos);
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
                        Unit selectedUnit = _manager.getGameFunctions().getSelectedUnit();
                        //Go here...
                        selectedUnit.moveUnitTo(xPos, yPos);
                       
                        //...and open the context menu
                        _manager.getContextMenu().openContextMenu(xPos, yPos);
                    }
                    break;
                case GameFunctions.mode.menu:
                    Debug.Log("Menu mode");
                    break;
            }
        }

        //Menu
        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Cancel");
            //displayCursorGfx(true);reactivate later
            buttonSelected = false;
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
                    setCursorsPosition(x, y);
                    break;
                case GameFunctions.mode.fire:
                    //cycle through the attackable enemies
                    break;
                case GameFunctions.mode.move:
                    //Draws an Arrow on the tile, if it is reachable
                    Tile tile = _manager.getMapCreator().getTile(x, y);
                    if (tile.isReachable && !tile.isPartOfArrowPath)
                    {
                        //We wont move outside the bounds of the reachable area.
                        setCursorsPosition(x, y);
                        _manager.getArrowBuilder().createArrowPath(tile);
                    }
                    //If you go back, make the arrow smaller.
                    if (tile.isPartOfArrowPath)
                    {
                        //We wont move outside the bounds of the reachable area.
                        setCursorsPosition(x, y);
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
    

    public void setCursorsPosition(int x, int y)
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
}
