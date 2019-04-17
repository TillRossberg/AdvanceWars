using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBuilder
{
    public ArrowBuilder(Transform arrowPathParent)
    {
        this.arrowPathParent = arrowPathParent;
    }
    #region Fields
    Transform arrowPathParent;
    int maxMovementPoints = 0; //Maximum MovementPoints or the maximum length of the arrow
    int momMovementPoints = 0; //Momentary MovementPoints.    
    List<ArrowPart> arrowPath = new List<ArrowPart>();//The path of the movement arrow.
    bool _isInterrupted = false;
    Tile _interruptTile;
    #endregion


    #region Arrow Path
    //Initiate the arrowBuilder with the start point of the path.
    public void StartArrowPath(Tile tile, int moveDist)
    {
        _isInterrupted = false;
        tile.isPartOfArrowPath = true;
        ArrowPart firstNode = GameObject.Instantiate(Core.Model.Database.arrowPartPrefab, arrowPathParent).GetComponent<ArrowPart>();
        firstNode.Init(ArrowPart.Type.firstNode, tile);
        arrowPath.Add(firstNode);//Set this tile as startpoint of the arrowPath
        
        momMovementPoints = maxMovementPoints = moveDist;//Handover the maximum movement points of the unit.        
    }

    public void CreateNextPart(Tile tile)
    {        
        ArrowPart newPart = CreatePart(ArrowPart.Type.arrow, tile);
        arrowPath.Add(newPart);
        tile.isPartOfArrowPath = true;
        momMovementPoints -= tile.data.GetMovementCost(Core.Controller.SelectedUnit.data.moveType);
        Tile preTile = GetPredecessorTile();
        float angle = GetFacingDirection(tile, preTile);
        newPart.SetRotation(angle);
        //If there are more than two parts always change the last part.
        if(arrowPath.Count > 2)
        {
            Tile prePreTile = GetPrePreDecessorTile();
            ChangePart(arrowPath[arrowPath.Count - 2], tile, preTile, prePreTile);
        }      
    }

    //If you hover with the mouse over the predecessor of the arrow, the arrow should become smaller.
    public void TryToGoBack(Tile tile)
    {
        if (arrowPath.Count > 1)//Dont't touch the first entry, because that's the tile where the unit stands on.
        {
            //Reset the arrow path if you touch the starting tile.
            if (arrowPath[0].AssignedTile == tile)
            {
                //Debug.Log("Reducing path");
                ReducePathToFirstNode();
            }
            else if (GetPredecessorTile() == tile) //If you touch the predecessor shorten the path by one.
            {
                //Debug.Log("Deleting part");
                momMovementPoints += tile.data.GetMovementCost(Core.Controller.SelectedUnit.data.moveType);//If you go back, you have more movement points available.
                //Delete last entry of the arrow path, because that is the arrowhead.
                ArrowPart lastPart = arrowPath[arrowPath.Count - 1];
                lastPart.AssignedTile.isPartOfArrowPath = false;//Is no longe part of the arrow path.
                GameObject.Destroy(lastPart.gameObject);
                arrowPath.Remove(lastPart);//Delete it from the list.
                //Change the gfx of the new last part to be an arrow and face the right direction.
                arrowPath[arrowPath.Count - 1].SetGfx(ArrowPart.Type.arrow);
                arrowPath[arrowPath.Count - 1].SetRotation(GetFacingDirection(tile, GetPredecessorTile()));
            }
            else
            {
                //If you touch any of the reachable tiles, the shortest path to this tile will be calculated.
                //TODO: calculate shortest path to this tile... will follow if A* is implemented
            }
        }
    }
    //TODO: goes out, if A* is implemented
    public bool CanGoBack(Tile tile)
    {
        if (tile == GetPredecessorTile()) return true;
        else return false;
    }

    ArrowPart CreatePart(ArrowPart.Type type, Tile tile)
    {
        ArrowPart newPart = GameObject.Instantiate(Core.Model.Database.arrowPartPrefab, arrowPathParent).GetComponent<ArrowPart>();
        newPart.Init(type, tile);
        return newPart;
    }
    void ChangePart(ArrowPart part, Tile tile, Tile preTile, Tile prePreTile)
    {
        ArrowPart.Type newType = ArrowPart.Type.firstNode;
        float newRotation = 0;
        //Vertical...
        if(tile.position.x == preTile.position.x)
        {            
            if(tile.position.x == prePreTile.position.x)//Top/bottom in a straight line.
            {
                newType = ArrowPart.Type.straight;
                newRotation = 0;
            }
            else if(tile.position.y > preTile.position.y)//...with one step up...
            {
                newType = ArrowPart.Type.curve;
                if (tile.position.x > prePreTile.position.x) newRotation = 90;//...and the prepredecessor on the left.      
                else newRotation = 180; //...and the prepredecessor on the right.               
            }            
            else if (tile.position.y < preTile.position.y)//...with one step down...
            {
                newType = ArrowPart.Type.curve;
                if (tile.position.x > prePreTile.position.x)newRotation = 0;//...and the prepredecessor on the left.
                else newRotation = 270;//...and the prepredecessor on the right.
            }
        }
        //Horizontal...
        else if(tile.position.y == preTile.position.y)
        {            
            if (tile.position.y == prePreTile.position.y)//Left/Right in a straight line.
            {
                newType = ArrowPart.Type.straight;
                newRotation = 90;
            }            
            else if(tile.position.x > preTile.position.x)//...with one step to the right...
            {
                newType = ArrowPart.Type.curve;                
                if (tile.position.y < prePreTile.position.y)newRotation = 180;//...and the prepredecessor upwards.
                else newRotation = 270;  //...and the prepredecessor downwards.
            }            
            else if(tile.position.x < preTile.position.x)//...with one step to the left...
            {
                newType = ArrowPart.Type.curve;               
                if (tile.position.y < prePreTile.position.y)newRotation = 90; //...and the prepredecessor upwards.
                else newRotation = 0;//...and the prepredecessor downwards.
            }
        }       
        else throw new System.Exception("No matching case for arrow change found!");
        part.SetGfx(newType);
        part.SetRotation(newRotation);
    }

    float GetFacingDirection(Tile currentTile, Tile lastTile)
    {
        if (lastTile.position.x == currentTile.position.x)
        {
            if (lastTile.position.y < currentTile.position.y) return 180;
            else return 0;
        }
        else if (lastTile.position.y == currentTile.position.y)
        {
            if (lastTile.position.x < currentTile.position.x) return 270;
            else return 90;
        }
        else throw new System.Exception("Error in direction calculation!");
    }

    Tile GetPredecessorTile()
    {
        return arrowPath[arrowPath.Count - 2].AssignedTile;
    }
    Tile GetPrePreDecessorTile()
    {
        return arrowPath[arrowPath.Count - 3].AssignedTile;
    }
 
    //Clear the arrow path, inform the tiles that they are no longer part of the arrow path and reset the movement points.
    //Also we set the first entry of the arrow path to be the position of the unit, so we can start building a new path from there.
    void ReducePathToFirstNode()
    {
        momMovementPoints = maxMovementPoints;
        ArrowPart firstNode = arrowPath[0];        
        for(int i = 0; i < arrowPath.Count; i++)
        {
            arrowPath[i].AssignedTile.isPartOfArrowPath = false;
            GameObject.Destroy(arrowPath[i].gameObject);
        }
        arrowPath.Clear();
        arrowPath.Add(firstNode);
    }

    public bool EnoughMovePointsRemaining(Tile tile, Unit unit)
    {
        if ((momMovementPoints - tile.data.GetMovementCost(unit.data.moveType)) >= 0) return true;
        else return false;            
    }
    #endregion
    #region Movement Path
    //Calculates a direct path from the arrow path. I.e.: combine arrow parts that are in a straight line to be just one checkpoint for the movement.
    public List<Vector3> CreateMovementPath()
    {
        List<Vector3> movementPath = new List<Vector3>();
        AddWaypoint(movementPath, arrowPath[0].AssignedTile.position);//Set the starting position of the movement path as the position we are on right now.
        //TODO: implement the height for mountains and rivers!--> Linerenderer!!!
        for (int i = 1; i < arrowPath.Count; i++)
        {
            Tile tile = arrowPath[i].AssignedTile;
            if(!IsEnemyUnitHere(tile))
            {
                if (arrowPath[i].type == ArrowPart.Type.curve) AddWaypoint(movementPath, tile.position);      
            }
            else
            {
                Tile preTile = arrowPath[i - 1].AssignedTile;
                AddWaypoint(movementPath, preTile.position);
                _isInterrupted = true;
                _interruptTile = preTile;
            }
        }
        if(!_isInterrupted) AddWaypoint(movementPath, arrowPath[arrowPath.Count - 1].AssignedTile.position);//Endpoint        
        return movementPath;
    }
    void AddWaypoint(List<Vector3> wayPointList, Vector2Int position)
    {
        wayPointList.Add(new Vector3(position.x, 0, position.y));
    }

    bool IsEnemyUnitHere(Tile tile)
    {
        if (tile.unitStandingHere != null && Core.Controller.SelectedUnit.IsEnemy(tile.unitStandingHere)) return true;
        else return false;
    }   
    public bool GetInterruption(){return _isInterrupted;}
    public Tile GetInterruptionTile() { return _interruptTile; }

    #endregion
    public void ResetAll()
    {        
        momMovementPoints = 0;
        maxMovementPoints = 0;
        if(arrowPath.Count > 0)
        {
            foreach (ArrowPart part in arrowPath)
            {
                if(part != null)
                {
                    part.AssignedTile.isPartOfArrowPath = false;
                    GameObject.Destroy(part.gameObject);
                }
            }            
            arrowPath.Clear();
        }        
    }
}
