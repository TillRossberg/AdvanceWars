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
                if (_mapCreator.isInsideGraph(xGraph, yGraph + 1))
                {
                    goTo(xGraph, yGraph + 1);
                    _manager.getStatusWindow().updateStatusPanel(xGraph, yGraph);
                }
                resetAxisInUseDelayed();
            }
        }
        else
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (!vertAxisInUse)
            {
                vertAxisInUse = true;
                if (_mapCreator.isInsideGraph(xGraph, yGraph - 1))
                {
                    goTo(xGraph, yGraph - 1);
                    _manager.getStatusWindow().updateStatusPanel(xGraph, yGraph);
                }
                resetAxisInUseDelayed();
            }
        }

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            if (!horAxisInUse)
            {
                horAxisInUse = true;
                if (_mapCreator.isInsideGraph(xGraph - 1, yGraph))
                {
                    goTo(xGraph - 1, yGraph);
                    _manager.getStatusWindow().updateStatusPanel(xGraph, yGraph);
                }
                resetAxisInUseDelayed();
            }
        }
        else
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (!horAxisInUse)
            {
                horAxisInUse = true;
                if (_mapCreator.isInsideGraph(xGraph + 1, yGraph))
                {
                    goTo(xGraph + 1, yGraph);
                    _manager.getStatusWindow().updateStatusPanel(xGraph, yGraph);
                }
                resetAxisInUseDelayed();
            }
        }
        //Selection
        if(Input.GetButtonDown("Fire1"))
        {
            switch (_manager.getGameFunctions().getCurrentMode())
            {
                case GameFunctions.mode.normal:
                    if (_mapCreator.getTile(xGraph, yGraph).getUnitHere() != null)
                    {
                        Unit unitHere = _mapCreator.getTile(xGraph, yGraph).getUnitHere().GetComponent<Unit>();
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

                    }
                    break;
                case GameFunctions.mode.fire:
                    break;
                case GameFunctions.mode.move:
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
        this.transform.position = new Vector3(_mapCreator.getTile(x, y).transform.position.x, 0, _mapCreator.getTile(x, y).transform.position.z);
        xGraph = x;
        yGraph = y;
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
