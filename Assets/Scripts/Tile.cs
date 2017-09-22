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
    public int weight;
    
    public List<Transform> nachbarn = new List<Transform>();

    //Graphic
    public Transform markingCursor;
    public Unit unitStandingHere;

    //Movement
    public int footCost;
	public int mechCost;
	public int treadsCost;
	public int wheelsCost;
	public int landerCost;
	public int shipCost;
	public int airCost;

    public bool isReachable = false;
    public bool isSelected = false;
    public bool isBlockedByBlue = false;
    public bool isBlockedByRed = false;
    public bool isPartOfArrowPath = false;
    public bool isAttackable = false;

    //Battle
    //Kampfhintergrund
    public int cover;

    public Tile(string terrainName, int xPos, int yPos)
    {
        this.terrainName = terrainName;
        this.xPos = xPos;
        this.xPos = yPos;        
    }

    public void setTerrainName(string name)
    {
        this.terrainName = name;
    }

	// Use this for initialization
	void Start () 
	{
        myLevelManager = GameObject.FindGameObjectWithTag("LevelManager");
    }	

    private void OnMouseDown()
    {
        //Actions are only perfomed, if the menu is not opened.
        if(!myLevelManager.GetComponent<ContextMenu>().isOpened)
        {
            if(myLevelManager.GetComponent<MainFunctions>().moveMode)
            {
                if(isPartOfArrowPath)
                {
                    //Move to the position and try to find units that can be attacked.
                    myLevelManager.GetComponent<MainFunctions>().selectedUnit.GetComponent<Unit>().moveUnitTo(this.xPos, this.yPos);
                    myLevelManager.GetComponent<MainFunctions>().selectedUnit.GetComponent<Unit>().findAttackableTiles();
                    //Delete the reachable tiles and the movement arrow.
                    myLevelManager.GetComponent<Graph>().resetReachableTiles();
                    myLevelManager.GetComponent<ArrowBuilder>().resetAll();
                    //Decide if the menu with firebutton and wait button is opened OR if only the wait button is to display.
                    if(myLevelManager.GetComponent<MainFunctions>().selectedUnit.attackableUnits.Count > 0)
                    {
                        //Open context menu at the position you want to go to.
                        myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 1);
                    }
                    else
                    {
                        //Open context menu at the position you want to go to.
                        myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 0);
                    }
                }
                else
                //If no unit stands here...
                if (!isBlockedByBlue && !isBlockedByRed)
                {
                    //...select the tile.
                    myLevelManager.GetComponent<MainFunctions>().selectTile(this);         
                }
            }
            if(myLevelManager.GetComponent<MainFunctions>().normalMode)
            {
                //If no unit stands here...
                if (!isBlockedByBlue && !isBlockedByRed)
                {
                    //...select the tile.
                    myLevelManager.GetComponent<MainFunctions>().selectTile(this);
                }
               
            }
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
                if (this == myLevelManager.GetComponent<ArrowBuilder>().arrowPath[0].myTileProperties && myLevelManager.GetComponent<ArrowBuilder>().arrowPath.Count > 2)
                {
                    myLevelManager.GetComponent<ArrowBuilder>().resetArrowPath();
                }
            }

        }
    }

    //Set the unit that stands on this tile.
    public void setUnitHere(Unit unit)
    {
        unitStandingHere = unit;
    }

    //If a unit moves on or dies, clear the unit that was standing on this tile
    public void clearUnit()
    {
        unitStandingHere = null;
    }
}
