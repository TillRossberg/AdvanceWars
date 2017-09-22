using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainFunctions : MonoBehaviour
{
    //Selectstuff
    //public Transform selectedObject;
    public Tile selectedTile;
    public Unit selectedUnit;
    public Transform markingCursor;

    bool isTile = false;
    bool isUnit = false;
    public bool normalMode = true;
    public bool fireMode = false;
    public bool moveMode = false;

	// Use this for initialization
	void Start ()
    {
        

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

    //If you click on a new object, drop the old one, return to normal mode, delete the marking cursor and reset all data relying to this object.
    public void deselectObject()
    {
        if(isTile)
        {            
            deselectTile();
        }
        if(isUnit)
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
        isUnit = false;//Necessary to decide whether we have a tile or an unit selected.
        selectedUnit = null;
    }
    //Deselect a Tile.
    public void deselectTile()
    {
        selectedTile.GetComponent<Tile>().isSelected = false;        
        isTile = false;//Necessary to decide whether we have a tile or an unit selected.
        selectedTile = null;
    }

    //Select a tile.
    public void selectTile(Tile myObject)
    {
        deselectObject();//Previous selected object out!
        selectedTile = myObject.GetComponent<Tile>(); ;//Handover the tile.
        this.GetComponent<ContextMenu>().closeMenu();//Make sure the menu is not visible, when you click on a unit.
        selectedTile.isSelected = true;
        isTile = true;
        //Open menu with info button about the tile.
        this.GetComponent<ContextMenu>().openContextMenu(selectedTile.xPos, selectedTile.yPos, 2);
        //Create marking cursor
        Instantiate(markingCursor, new Vector3(selectedTile.transform.position.x, - 0.1f, selectedTile.transform.position.z), Quaternion.identity, this.transform);
    }

    //Select an unit.
    public void selectUnit(Unit myObject)
    {
        //If you click the same unit again, the menu should open...
        if(selectedUnit == myObject)
        {
            //Open the menu
            this.GetComponent<ContextMenu>().openContextMenu(selectedUnit.xPos, selectedUnit.yPos, 0);
        }
        //...otherwise, simply select the unit and initiate the arrow path.
        else
        {
            deselectObject(); //Previous selected object out!
            selectedUnit = myObject;//Handover the object
            this.GetComponent<ContextMenu>().closeMenu();//Make sure the menu is not visible, when you click on a unit.
            this.GetComponent<StatusWindow>().showStatus(false);
            selectedUnit.isSelected = true;
            isUnit = true;
            //Create marking cursor
            Instantiate(markingCursor, new Vector3(selectedUnit.transform.position.x, selectedUnit.transform.position.y - 0.2f, selectedUnit.transform.position.z), Quaternion.identity, selectedUnit.transform);
            //Show the status of the unit.
            this.GetComponent<StatusWindow>().showStatus(true);
            int myCover = this.GetComponent<Graph>().getTile(selectedUnit.xPos, selectedUnit.yPos).cover;
            string tileName = this.GetComponent<Graph>().getTile(selectedUnit.xPos, selectedUnit.yPos).terrainName;
            Sprite tileThumb = this.GetComponent<Graph>().getTile(selectedUnit.xPos, selectedUnit.yPos).thumbnail;
            this.GetComponent<StatusWindow>().changeStatus(selectedUnit.unitName, selectedUnit.thumbNail, selectedUnit.health, selectedUnit.ammo, selectedUnit.fuel, tileName, tileThumb, myCover);

            //The logic that draws an arrow, that shows where the unit can go.
            Tile tilePropTheUnitStandsOn = this.GetComponent<Graph>().getGraph()[selectedUnit.xPos][selectedUnit.yPos].GetComponent<Tile>();
            tilePropTheUnitStandsOn.isPartOfArrowPath = true;
            ArrowPart myArrowPart = ScriptableObject.CreateInstance("ArrowPart") as ArrowPart;
            myArrowPart.init("First node", null, tilePropTheUnitStandsOn);
            this.GetComponent<ArrowBuilder>().arrowPath.Add(myArrowPart);//Set this tile as startpoint of the arrowPath
            this.GetComponent<ArrowBuilder>().momMovementPoints = selectedUnit.moveDist;//Handover the maximum movement points of the unit.
            this.GetComponent<ArrowBuilder>().maxMovementPoints = selectedUnit.moveDist;//Set a maximum for the movement points (for resetting purposes).
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
}
