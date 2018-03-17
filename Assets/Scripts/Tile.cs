//created by Till Roßberg, 2017-18
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile: MonoBehaviour
{
    //Required Structures
    GameObject myLevelManager;

	//General
	public string terrainName;
    public Sprite thumbnail;
	public int xPos;
	public int yPos;
	public int rotation;
    public Transform unitStandingHere = null;

    //Tile types
    public enum type  { Road , Plain, RoadStraight, RoadCurve, RoadBridge, Mountain, Forest, Sea, Shoal, Reef, Port, Facility, Airport, City, HQ, River};
    public type myTileType;
       
    public List<Transform> neighbors = new List<Transform>();

    //Graphic
    public Transform markingCursor;

    //Movement
    public int footCost;
	public int mechCost;
	public int treadsCost;
	public int wheelsCost;
	public int landerCost;
	public int shipCost;
	public int airCost;

    //Property stuff (i.e. is a building/facility that can be occupied)
    private int takeOverCounter = 20;//For each lifepoint of the infantry/mech unit this value is lowered by one, if the takeover action is performed.
    
    public Team owningTeam;

    //States
    public bool isVisible = false;
    public Transform fogOfWar;
    public bool isReachable = false;
    public bool isSelected = false;
    public bool isPartOfArrowPath = false;
    public bool isAttackable = false;

    //Battle
    public int cover;
    
	// Use this for initialization
	void Awake () 
	{
        myLevelManager = GameObject.FindGameObjectWithTag("LevelManager");
    }

    private void OnMouseDown()
    {      
        //myLevelManager.GetComponent<AnimController>().boom(xPos, yPos);
        //Actions are only perfomed, if no menu is opened.
        if (!myLevelManager.GetComponent<ContextMenu>().isOpened && !myLevelManager.GetComponent<Menu_BuyUnits>().isOpened)
        {            
            //Move mode
            if(myLevelManager.GetComponent<MainFunctions>().moveMode)
            {
                Unit selectedUnit = myLevelManager.GetComponent<MainFunctions>().selectedUnit.GetComponent<Unit>();
                if((isPartOfArrowPath && unitStandingHere == null) || (isPartOfArrowPath && !isVisible))
                {
                    //Move to the position and try to find units that can be attacked.
                    selectedUnit.moveUnitTo(this.xPos, this.yPos);
                    selectedUnit.findAttackableTiles();
                    selectedUnit.findAttackableEnemies();
                    //Delete the reachable tiles and the movement arrow.
                    myLevelManager.GetComponent<MapCreator>().resetReachableTiles();
                    myLevelManager.GetComponent<ArrowBuilder>().resetAll();

                    //Decide if the menu with firebutton and wait button is opened ...
                    if(selectedUnit.attackableUnits.Count > 0)
                    {
                        //If the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
                        if (isOccupyable(selectedUnit))
                        {
                            myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 3);
                        }
                        else
                        {
                            myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 1);
                        }
                    }
                    //...OR if only the wait button is to display.
                    else
                    {
                        //If the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
                        if (isOccupyable(selectedUnit))
                        {
                            myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 2);
                        }
                        else
                        {
                            myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 0);
                        }
                    }
                }
                else
                //Even in move mode, you can still click on buildings.
                //If no unit stands here...
                if (unitStandingHere == null)
                {
                    //...select the tile.
                    myLevelManager.GetComponent<MainFunctions>().selectTile(this);         
                }
            }
            //Normal mode
            if(myLevelManager.GetComponent<MainFunctions>().normalMode)
            {
                //If no unit stands here...
                if (unitStandingHere == null)
                {
                    //...select the tile.
                    myLevelManager.GetComponent<MainFunctions>().selectTile(this);
                }                
            }
            //Fire mode
            if(myLevelManager.GetComponent<MainFunctions>().fireMode)
            {

            }
        }
    }    

    private void OnMouseEnter()
    {
        //Actions are only perfomed, if the menu is not opened.
        if (!myLevelManager.GetComponent<ContextMenu>().isOpened && myLevelManager.GetComponent<MainFunctions>().moveMode)
        {
            //Draws an Arrow on the tile, if it is reachable
            if (isReachable && !isPartOfArrowPath )
            {
                myLevelManager.GetComponent<ArrowBuilder>().createArrowPath(this);
            
            }
            //If you go back, make the arrow smaller.
            if (isPartOfArrowPath)
            {
                myLevelManager.GetComponent<ArrowBuilder>().tryToGoBack(this);
                
                //Resets the arrowPath if you hover over the unit again. (If this is the tile the unit stands on and an arrow has been drawn.)
                if (this == myLevelManager.GetComponent<ArrowBuilder>().arrowPath[0].getTile() && myLevelManager.GetComponent<ArrowBuilder>().arrowPath.Count > 2)
                {
                    myLevelManager.GetComponent<ArrowBuilder>().resetArrowPath();
                }
            }

        }
    }

    //Check if a unit can occupy this tile
    //TODO: Check if the owning team is part of our teamteam.
    public bool isOccupyable(Unit unit)
    {
        if ((owningTeam != unit.myTeam)
            && (unit.myUnitType == Unit.type.Infantry || unit.myUnitType == Unit.type.Mech)
            && (myTileType == type.Airport || myTileType == type.Facility || myTileType == type.HQ || myTileType == type.City || myTileType == type.Port))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Set the unit that stands on this tile.
    public void setUnitHere(Transform unit)
    {
        unitStandingHere = unit;
    }

    //If a unit moves on or dies, clear the unit that was standing on this tile
    public void clearUnitHere()
    {
        unitStandingHere = null;
    }

    //If the tile is a property, set its color to the occuping team color
    public void setMaterial(Material newMaterial)
    {
        Material[] tempMats = this.transform.Find("Building").GetComponent<MeshRenderer>().materials;
        tempMats[0] = newMaterial;
        this.transform.Find("Building").GetComponent<MeshRenderer>().materials = tempMats;
    }

    //Returns the movement cost for a certain movement type.
    public int getMovementCost(Unit.moveType myMoveType)
    {
        switch(myMoveType)
        {
            case Unit.moveType.Air: return airCost;
            case Unit.moveType.Foot: return footCost;
            case Unit.moveType.Lander: return landerCost;
            case Unit.moveType.Mech: return mechCost;
            case Unit.moveType.Ship: return shipCost;
            case Unit.moveType.Treads: return treadsCost;
            case Unit.moveType.Wheels: return wheelsCost;
            default:
                Debug.Log("Tile: No such moveType" + myMoveType + "was found!");
                return -1;
        }
    }

    //Sets the visiblity of this tile.
    public void setVisibility(bool value)
    {
        isVisible = value;        
    }

    //Subtract the life points of the unit that tries to occupy from the take over counter.
    //If that counter reaches zero, your team captures the property.
    public void occupy(int unitHealth)
    {
        takeOverCounter -= unitHealth;
        //TODO: play some animation for taking over.
        if (takeOverCounter <= 0)
        {
            takeOverCounter = 0;
            //TODO: play some animation for a successful take over.
            myLevelManager.GetComponent<TeamManager>().occupyProperty(myLevelManager.GetComponent<MainFunctions>().selectedUnit.myTeam, this);            
        }
    }

    //Reset the take over counter to 20.
    public void resetTakeOverCounter()
    {
        takeOverCounter = 20;
    }
}
