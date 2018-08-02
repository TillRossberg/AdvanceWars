//created by Till Roßberg, 2017-18
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile: MonoBehaviour
{
    //Required Structures
    private Manager _manager;

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
    public bool isVisible = true;
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
        _manager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Manager>();
    }
    public void init()
    {
    }

    private void OnMouseDown()
    {
        //myLevelManager.GetComponent<AnimController>().boom(xPos, yPos);
        //Actions are only perfomed, if no menu is opened.
                 
            //Move mode
            if(_manager.getGameFunctions().getCurrentMode() == GameFunctions.mode.move)
            {
                Unit selectedUnit = _manager.getGameFunctions().getSelectedUnit().GetComponent<Unit>();
                if((isPartOfArrowPath && unitStandingHere == null) || (isPartOfArrowPath && !isVisible))
                {
                    //Move to the position and try to find units that can be attacked.
                    selectedUnit.moveUnitTo(this.xPos, this.yPos);
                    selectedUnit.findAttackableTiles();
                    selectedUnit.findAttackableEnemies();
                    //Delete the reachable tiles and the movement arrow.
                    _manager.getMapCreator().resetReachableTiles();
                    _manager.getArrowBuilder().resetAll();                    
                }
            }       
   
        
    }    

    private void OnMouseEnter()
    {
        //Actions are only perfomed, if the menu is not opened.
        if (!_manager.getContextMenu().isOpened && _manager.getGameFunctions().getCurrentMode() == GameFunctions.mode.move)
        {
            //Draws an Arrow on the tile, if it is reachable
            if (isReachable && !isPartOfArrowPath )
            {
                _manager.getArrowBuilder().createArrowPath(this);
            }
            //If you go back, make the arrow smaller.
            if (isPartOfArrowPath)
            {
                _manager.getArrowBuilder().tryToGoBack(this);
                
                //Resets the arrowPath if you hover over the unit again. (If this is the tile the unit stands on and an arrow has been drawn.)
                if (this == _manager.getArrowBuilder().getArrowPath()[0].getTile() && _manager.getArrowBuilder().getArrowPath().Count > 2)
                {
                    _manager.getArrowBuilder().resetArrowPath();
                }
            }
        }
    }

    //Check if a unit can occupy this tile
    //TODO: Check if the owning team is part of our teamteam. (much later)
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

    public Transform getUnitHere()
    {        
        return unitStandingHere;      
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

    public void setColor(Color color)
    {
        Material[] tempMats = this.transform.Find("Building").GetComponent<MeshRenderer>().materials;
        tempMats[0].color = color;
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

    //Set/get the visiblity of this tile.
    public void setVisible(bool value)
    {
        isVisible = value;        
    }
    public bool getVisibility()
    {
        return isVisible;
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
            _manager.getTeamManager().occupyProperty(_manager.getGameFunctions().getSelectedUnit().myTeam, this);            
        }
    }

    //Reset the take over counter to 20.
    public void resetTakeOverCounter()
    {
        takeOverCounter = 20;
    }
}
