﻿//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainFunctions : MonoBehaviour
{
    //Data structures
    private Container container;
    TeamManager teamManager;

    //Selectstuff
    public Tile selectedTile;
    public Unit selectedUnit;

    //Gameplaystuff (Who hast turn? What weather is it? What day is it? And so on...)
    

    //States    
    //These bools should help to decide if we selected a unit or a tile.
    bool isTile = false;
    bool isUnit = false;

    public bool normalMode = true;
    public bool fireMode = false;
    public bool moveMode = false;
    
    //Gfx
    public Transform markingCursor;

	// Use this for initialization
	void Start ()
    {
        if (GameObject.FindWithTag("Container") != null)
        {
            container = GameObject.FindWithTag("Container").GetComponent<Container>();
         
        }
        else
        {
            Debug.Log("MainFunctions: No container found!");
        }
        teamManager = GetComponent<TeamManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        rightMouseClick();
	}

    //Actions to perform on right mouse down.
    public void rightMouseClick()
    {
        if (Input.GetMouseButtonDown(1))
        {            
            deselectObject();
            if(this.GetComponent<Menu_BuyUnits>().isOpened)
            {
                this.GetComponent<Menu_BuyUnits>().closeMenu();
            }
        }
    }
    
    //Toggle between the modes
    public void activateNormalMode()
    {
        fireMode = false;
        moveMode = false;
        normalMode = true;
    }
    public void activateFireMode()
    {
        moveMode = false;
        normalMode = false;
        fireMode = true;
    }
    public void activateMoveMode()
    {
        normalMode = false;
        fireMode = false;
        moveMode = true;
    }

    //Select an unit.
    public void selectUnit(Unit myUnit)
    {
        deselectObject(); //Previous selected object out!
        selectedUnit = myUnit;//Handover the object.
        selectedUnit.isSelected = true;
        isUnit = true;
        this.GetComponent<ContextMenu>().closeMenu();//Make sure the menu is not visible, when you click on a unit.
        createMarkingCursor(selectedUnit);//Draw a marking cursor
        this.GetComponent<StatusWindow>().showStatus(true);//Show Unit status

        //The logic that draws an arrow, that shows where the unit can go.
        Tile tileTheUnitStandsOn = this.GetComponent<Graph>().getGraph()[selectedUnit.xPos][selectedUnit.yPos].GetComponent<Tile>();
        this.GetComponent<ArrowBuilder>().init(tileTheUnitStandsOn, selectedUnit.moveDist);
    }
    //Select a tile.
    public void selectTile(Tile myObject)
    {
        deselectObject();//Previous selected object out!
        selectedTile = myObject.GetComponent<Tile>(); ;//Handover the tile.
        this.GetComponent<ContextMenu>().closeMenu();//Make sure the menu is not visible, when you click on a tile.
        selectedTile.isSelected = true;
        isTile = true;
        //Decide wich menu to open.
        //Facility
        if (myObject.myTileType == Tile.type.Facility) { this.GetComponent<Menu_BuyUnits>().openMenu(1); }
        else
        //Airport
        if (myObject.myTileType == Tile.type.Airport) { this.GetComponent<Menu_BuyUnits>().openMenu(2); }
        else
        //Harbor
        if (myObject.myTileType == Tile.type.Port) { this.GetComponent<Menu_BuyUnits>().openMenu(3); }
        else
        {
            //Open menu with info button about the tile.
            this.GetComponent<ContextMenu>().openContextMenu(selectedTile.xPos, selectedTile.yPos, 2);
        }
        //Create marking cursor
        Instantiate(markingCursor, new Vector3(selectedTile.transform.position.x, -0.1f, selectedTile.transform.position.z), Quaternion.identity, this.transform);
    }
    //If you click on a new object, drop the old one, return to normal mode, delete the marking cursor and reset all data referring to this object.
    public void deselectObject()
    {
        if (isTile)
        {
            deselectTile();
        }
        if (isUnit)
        {
            deselectUnit();
        }
        deleteMarkingCursor();
        this.gameObject.GetComponent<ContextMenu>().closeMenu();//Hide context menu
        this.GetComponent<StatusWindow>().showStatus(false);
        activateNormalMode();
    }
    //Deselect a Unit.
    public void deselectUnit()
    {
        //If the unit has moved and still can fire, reset it to where it was before.
        if (selectedUnit.hasMoved && selectedUnit.canFire)
        {
            this.GetComponent<MainFunctions>().selectedUnit.resetPosition();
        }
        selectedUnit.isSelected = false;//Deselect Unit
        selectedUnit.resetBattleInformation();//Reset the attackableTiles-list and the attackableUnits-list.
        this.GetComponent<Graph>().resetReachableTiles();//Resets all tiles to not reachable
        this.GetComponent<Graph>().resetAttackableTiles();//Resets all tiles to not attackable
        this.GetComponent<ArrowBuilder>().resetAll();//Resets the movement arrow.        
        isUnit = false;
        selectedUnit = null;
    }
    //Deselect a Tile.
    public void deselectTile()
    {
        selectedTile.GetComponent<Tile>().isSelected = false;
        isTile = false;
        selectedTile = null;
    }
   
    //Load a specified level and spawn untis.
    public void loadLevel(int value)
    {
        switch (value)
        {
            case 0:
                this.GetComponent<Graph>().createLevel01();
                this.GetComponent<UnitCreator>().createUnitSet01();
                break;

            case 1:
                this.GetComponent<Graph>().createLevel02();
                this.GetComponent<UnitCreator>().createUnitSet02();

                break;

            default:
                Debug.Log("MasterClass: No such level found!");
                break;

        }
    }

   

    //Deletes all instances of the marking cursor.
    public void deleteMarkingCursor()
    {
        foreach(GameObject markingCursor in GameObject.FindGameObjectsWithTag("MarkingCursor"))
        {
            Destroy(markingCursor);
        }        
    }
    //Creates a marking cursor at the position of the given unit.
    public void createMarkingCursor(Unit myUnit)
    {
        Instantiate(markingCursor, new Vector3(myUnit.transform.position.x, myUnit.transform.position.y - 0.2f, myUnit.transform.position.z), Quaternion.identity, myUnit.transform);
    }
}
