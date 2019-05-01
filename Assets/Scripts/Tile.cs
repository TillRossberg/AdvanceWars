//created by Till Roßberg, 2017-18
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile: MonoBehaviour
{
    #region References
    public Data_Tile data;
    public GameObject Gfx;
    public GameObject fogOfWarGfx;
    #endregion
    #region General Fields
    public Vector2Int position;	
	public int rotation;
    public Unit unitStandingHere = null;   
    public List<Tile> neighbors;        
    public int takeOverCounter;//For each lifepoint of the infantry/mech unit this value is lowered by one, if the takeover action is performed.
    public Team owningTeam;
    #endregion    
    #region States
    public bool isVisible = true;
    public bool isPartOfArrowPath = false;

    #endregion
    #region Base Methods
    public void Init()
    {
        takeOverCounter = data.maxTakeOverPoints;
    }
    private void OnMouseDown()
    {
        //myLevelManager.GetComponent<AnimController>().boom(xPos, yPos);
        //Actions are only perfomed, if no menu is opened.
                 
            ////Move mode
            //if(_manager.getGameFunctions().getCurrentMode() == GameFunctions.mode.move)
            //{
            //    Unit selectedUnit = _manager.getGameFunctions().getSelectedUnit().GetComponent<Unit>();
            //    if((isPartOfArrowPath && unitStandingHere == null) || (isPartOfArrowPath && !isVisible))
            //    {
            //        //Move to the position and try to find units that can be attacked.
            //        selectedUnit.moveUnitTo(this.xPos, this.yPos);
            //        selectedUnit.FindAttackableTiles();
            //        selectedUnit.FindAttackableEnemies();
            //        //Delete the reachable tiles and the movement arrow.
            //        _manager.getMapCreator().ResetReachableTiles();
            //        _manager.getArrowBuilder().resetAll();                    
            //    }
            //}       
   
        
    }    

    private void OnMouseEnter()
    {
        ////Actions are only perfomed, if the menu is not opened.
        //if (!_manager.getContextMenu().isOpened && _manager.getGameFunctions().getCurrentMode() == GameFunctions.mode.move)
        //{
        //    //Draws an Arrow on the tile, if it is reachable
        //    if (isReachable && !isPartOfArrowPath )
        //    {
        //        _manager.getArrowBuilder().createArrowPath(this);
        //    }
        //    //If you go back, make the arrow smaller.
        //    if (isPartOfArrowPath)
        //    {
        //        _manager.getArrowBuilder().tryToGoBack(this);
                
        //        //Resets the arrowPath if you hover over the unit again. (If this is the tile the unit stands on and an arrow has been drawn.)
        //        if (this == _manager.getArrowBuilder().getArrowPath()[0].getTile() && _manager.getArrowBuilder().getArrowPath().Count > 2)
        //        {
        //            _manager.getArrowBuilder().resetArrowPath();
        //        }
        //    }
        //}
    }

    public void SetNeighbors()
    {
        Vector2Int testPos;
        testPos = new Vector2Int(position.x - 1, position.y);
        if (Core.Model.IsOnMap(testPos)) neighbors.Add(Core.Model.GetTile(testPos));
        testPos = new Vector2Int(position.x + 1, position.y);
        if (Core.Model.IsOnMap(testPos)) neighbors.Add(Core.Model.GetTile(testPos));
        testPos = new Vector2Int(position.x, position.y + 1);
        if (Core.Model.IsOnMap(testPos)) neighbors.Add(Core.Model.GetTile(testPos));
        testPos = new Vector2Int(position.x, position.y - 1);
        if (Core.Model.IsOnMap(testPos)) neighbors.Add(Core.Model.GetTile(testPos));
    }
    #endregion


    

    //Set the unit that stands on this tile.
    public void SetUnitHere(Unit unit){unitStandingHere = unit;}
    public Unit GetUnitHere(){ return unitStandingHere; }
    
    //If the tile is a property, set its color to the occuping team color
    public void SetMaterial(Material material)
    {
        Material[] tempMats = Gfx.GetComponent<MeshRenderer>().materials;
        tempMats[0] = material;
        Gfx.GetComponent<MeshRenderer>().materials = tempMats;
    }
    public void SetColor(Color color)
    {
        Material[] tempMats = Gfx.GetComponent<MeshRenderer>().materials;
        tempMats[0].color = color;
        Gfx.GetComponent<MeshRenderer>().materials = tempMats;
    }    

    

    #region Occupation
   
    //Reset the take over counter to 20.
    public void ResetTakeOverCounter()
    {
        takeOverCounter = data.maxTakeOverPoints;
    }

    //Check if a unit can occupy this tile
    //TODO: Check if the owning team is part of our alliance. (much later)
    public bool IsOccupyableBy(Unit unit)
    {
        if (data.isProperty && (owningTeam != unit.team) && (unit.data.type == UnitType.Infantry || unit.data.type == UnitType.Mech)) return true;
        else return false;
    }

    #endregion
    #region Conditions
    public bool CanProduceUnits()
    {
        switch (data.type)
        {         
            case TileType.Facility: return true;
            case TileType.Airport: return true;
            case TileType.Port: return true;
            default: return false;
        }
    }
    #endregion
}
