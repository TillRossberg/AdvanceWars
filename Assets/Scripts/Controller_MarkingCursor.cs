using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_MarkingCursor : MonoBehaviour
{
    //References
    public Manager _manager;
    public MapCreator _mapCreator;
    //Fields
    private float inputDelay = 0.145f;
    private bool horAxisInUse;
    private bool vertAxisInUse;
    private int xGraph = 0;
    private int yGraph = 0;

    public void init(int x, int y)
    {
        _manager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Manager>();
        _mapCreator = _manager.getMapCreator();
        xGraph = x;
        yGraph = y;
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
                goTo(xGraph, yGraph + 1);                    
                resetAxisInUseDelayed();
            }
        }
        else
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (!vertAxisInUse)
            {
                vertAxisInUse = true;
                goTo(xGraph, yGraph - 1);
                resetAxisInUseDelayed();
            }
        }

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            if (!horAxisInUse)
            {
                horAxisInUse = true;
                goTo(xGraph - 1, yGraph);
                resetAxisInUseDelayed();
            }
        }
        else
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (!horAxisInUse)
            {
                horAxisInUse = true;                
                goTo(xGraph + 1, yGraph);                
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
                    if (_mapCreator.getTile(xGraph, yGraph).getUnitHere() != null)
                    {
                         unitHere = _mapCreator.getTile(xGraph, yGraph).getUnitHere().GetComponent<Unit>();
                        //Select own unit
                        if (_manager.getTurnManager().getActiveTeam().isInMyTeam(unitHere))
                        {
                            _manager.getGameFunctions().selectUnit(unitHere);
                            //Calculate reachable area and instantiate the graphics for the tiles.
                            unitHere.calcReachableArea(xGraph, yGraph, unitHere.moveDist, unitHere.myMoveType, null);
                            //Debug.Log("Reachable iterations: " + counter);
                            _manager.getMapCreator().createReachableTiles();
                            unitHere.calcVisibleArea();
                            _manager.getGameFunctions().setCurrentMode(GameFunctions.mode.move);
                        }
                        //Show range of enemy unit
                        else
                        {
                            _manager.getGameFunctions().selectEnemyUnit(unitHere);
                            unitHere.displayAttackableTiles(true);
                        }

                    }
                    break;
                case GameFunctions.mode.fire:
                    break;
                case GameFunctions.mode.move:
                    if (_mapCreator.getTile(xGraph, yGraph).getUnitHere() != null)
                    {
                        unitHere = _mapCreator.getTile(xGraph, yGraph).getUnitHere().GetComponent<Unit>();
                        if (unitHere.isSelected)
                        {
                            //Decide if the menu with firebutton and wait button is opened ...
                            if (_manager.getGameFunctions().getSelectedUnit().attackableUnits.Count > 0)
                            {
                                //...if the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
                                if (_graphMatrix[xPos][yPos].GetComponent<Tile>().isOccupyable(this))
                                {
                                    _manager.getContextMenu().openContextMenu(xPos, yPos, 3);
                                }
                                else
                                {
                                    _manager.getContextMenu().openContextMenu(xPos, yPos, 1);
                                }
                            }
                            //...OR if only the wait button is to display.
                            else
                            {
                                //If the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
                                if (_graphMatrix[xPos][yPos].GetComponent<Tile>().isOccupyable(this))
                                {
                                    _manager.getContextMenu().openContextMenu(xPos, yPos, 2);
                                }
                                else
                                {
                                    _manager.getContextMenu().openContextMenu(xPos, yPos, 0);
                                }
                            }
                        }
                    }
                    break;
                case GameFunctions.mode.menu:
                    break;
            }
        }

        //Menu
        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Cancel");
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

    private void setCursorsPosition(int x, int y)
    {
        this.transform.position = new Vector3(_mapCreator.getTile(x, y).transform.position.x, 0, _mapCreator.getTile(x, y).transform.position.z);
        xGraph = x;
        yGraph = y;
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
}
