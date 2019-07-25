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
    List<Tile> Path = new List<Tile>();//The path of the movement arrow.
    List<GameObject> PathGFX = new List<GameObject>();
    bool _isInterrupted = false;
    Tile _interruptTile;
    #endregion


    #region Arrow Path
    //Initiate the arrowBuilder with the start point of the path.
    public void StartPath(Unit unit)
    {
        _isInterrupted = false;
        Path.Add(unit.CurrentTile);//Set this tile as startpoint of the arrowPath        
        momMovementPoints = maxMovementPoints = unit.data.moveDist;//Handover the maximum movement points of the unit.        
    }
    public void Add(Tile tile)
    {
        Path.Add(tile);
        momMovementPoints -= tile.data.GetMovementCost(Core.Controller.SelectedUnit.data.moveType);
        UpdatePathGFX(Path);
    }
    public void Remove(Tile tile)
    {
        Path.Remove(tile);
        momMovementPoints += tile.data.GetMovementCost(Core.Controller.SelectedUnit.data.moveType);
        UpdatePathGFX(Path);
    }
    public void UpdatePathGFX(List<Tile> path)
    {
        ResetPathGFX();
        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0) continue;
            if(i == path.Count - 1)
            {
                PathGFX.Add(CreateArrowHead(path[path.Count - 1], path[path.Count - 2]));
            }
            if(i >= 2)
            {
                Tile current = path[i];
                Tile previous = path[i - 1];
                Tile prePrevious = path[i - 2];
                if (IsInStraightLine(current, prePrevious)) PathGFX.Add(CreateArrowPart(Core.Model.Database.ArrowTail,current, previous, prePrevious));
                else PathGFX.Add(CreateArrowPart(Core.Model.Database.ArrowCurve, current, previous, prePrevious));
            }
        }        
    }
    void ResetPathGFX()
    {
        foreach (GameObject item in PathGFX) GameObject.Destroy(item);      
        PathGFX.Clear();
    }
    GameObject CreateArrowHead(Tile current, Tile previous)
    {
        //Debug.Log("arrow angle: " + GetArrowHeadAngle(current, previous));
        Vector3 position = new Vector3(current.Position.x, Core.Model.Database.arrowPathHeight, current.Position.y);
        Vector3 rotation = new Vector3(0, GetArrowHeadAngle(current, previous), 0);
        return GameObject.Instantiate(Core.Model.Database.ArrowHead, position, Quaternion.Euler(rotation));
    }
    float GetArrowHeadAngle(Tile current, Tile previous)
    {     
        if (previous.Position.x == current.Position.x)
        {
            if (previous.Position.y < current.Position.y) return 180;
            else return 0;
        }
        else if (previous.Position.y == current.Position.y)
        {
            if (previous.Position.x < current.Position.x) return 270;
            else return 90;
        }
        else throw new System.Exception("Error in direction calculation!");
    }
    GameObject CreateArrowPart(GameObject prefab, Tile current, Tile previous, Tile prePrevious)
    {
        Vector3 position = new Vector3(previous.Position.x, Core.Model.Database.arrowPathHeight, previous.Position.y);
        Vector3 rotation = new Vector3(0, GetArrowPartRotation(current, previous, prePrevious));
        return GameObject.Instantiate(prefab, position, Quaternion.Euler(rotation));
    }  
    float GetArrowPartRotation(Tile current, Tile previous, Tile prePrevious)
    {
        float newRotation = 0;
        //Vertical...
        if (current.Position.x == previous.Position.x)
        {
            if (current.Position.x == prePrevious.Position.x)//Top/bottom in a straight line.
            {
                newRotation = 0;
            }
            else if (current.Position.y > previous.Position.y)//...with one step up...
            {
                if (current.Position.x > prePrevious.Position.x) newRotation = 90;//...and the prepredecessor on the left.      
                else newRotation = 180; //...and the prepredecessor on the right.               
            }
            else if (current.Position.y < previous.Position.y)//...with one step down...
            {
                if (current.Position.x > prePrevious.Position.x) newRotation = 0;//...and the prepredecessor on the left.
                else newRotation = 270;//...and the prepredecessor on the right.
            }
        }
        //Horizontal...
        else if (current.Position.y == previous.Position.y)
        {
            if (current.Position.y == prePrevious.Position.y)//Left/Right in a straight line.
            {
                newRotation = 90;
            }
            else if (current.Position.x > previous.Position.x)//...with one step to the right...
            {
                if (current.Position.y < prePrevious.Position.y) newRotation = 180;//...and the prepredecessor upwards.
                else newRotation = 270;  //...and the prepredecessor downwards.
            }
            else if (current.Position.x < previous.Position.x)//...with one step to the left...
            {
                if (current.Position.y < prePrevious.Position.y) newRotation = 90; //...and the prepredecessor upwards.
                else newRotation = 0;//...and the prepredecessor downwards.
            }
        }
        else throw new System.Exception("No matching case for arrow change found!");
        return newRotation;
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
        //TODO: implement the height for mountains and rivers!--> Linerenderer!!!
        AddWaypoint(movementPath, Path[0].Position);
        for (int i = 1; i < Path.Count; i++)
        {
            Tile tile = Path[i];
            if (!IsEnemyUnitHere(tile))
            {
                if (!IsInStraightLine(tile, Path[i - 1])) AddWaypoint(movementPath, tile.Position);      
            }
            else
            {
                AddWaypoint(movementPath, Path[i - 1].Position);
                _isInterrupted = true;
                _interruptTile = Path[i - 1];
            }
        }
        if(!_isInterrupted) AddWaypoint(movementPath, Path[Path.Count - 1].Position);//Endpoint        
        return movementPath;
    }
    void AddWaypoint(List<Vector3> wayPointList, Vector2Int position)
    {
        wayPointList.Add(new Vector3(position.x, 0, position.y));
    }
  
    public bool GetInterruption(){return _isInterrupted;}
    public Tile GetInterruptionTile() { return _interruptTile; }

    #endregion
    #region Conditions
    bool IsInStraightLine(Tile current, Tile previous)
    {
        if (current.Position.x == previous.Position.x) return true;
        else if (current.Position.y == previous.Position.y) return true;
        else return false;
    }
    bool IsEnemyUnitHere(Tile tile)
    {
        if (tile.UnitHere != null && Core.Controller.SelectedUnit.IsMyEnemy(tile.UnitHere)) return true;
        else return false;
    }  
    public bool IsPartOfArrowPath(Tile tile)
    {
        if (Path.Contains(tile)) return true;
        else return false;
    }
    #endregion
    public void ResetAll()
    {
        _interruptTile = null;
        _isInterrupted = false;
        momMovementPoints = 0;
        maxMovementPoints = 0;
        Path.Clear();
        ResetPathGFX();
    }
    
   
}
