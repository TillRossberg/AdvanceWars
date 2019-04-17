//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour 
{
    #region References
    public Data_Unit data;
    public Unit_AnimationController AnimationController { get; private set; }
    public GameObject gfx;
    public TextMesh healthText;
    #endregion
  
    #region General Fields
    public Team team;
    public List<Team> enemyTeams = new List<Team>();
    public Tile targetTile;
    #endregion
    #region Position & Rotation
    public Vector2Int position;    
    public int rotation;
    public Direction direction;    
    float counter = 0;//Counts the iterations of the calcReachableArea algorithm.
    #endregion
    #region States
    public bool hasTurn = false;//The unit is ready for action.
    public bool CanMove = false;//States if the unit has already moved this turn.
    public bool CanFire = false;//Some units can't fire after they have moved.
    public bool IsInterrupted { get; private set; }//If we move through terrain that is covered by fog of war, we can be interrupted by an invisible enemy unit that is on our arrowpath.    
    public bool isSelected = false;
    #endregion
    #region Tiles
    public List<Tile> attackableTiles = new List<Tile>();
    public List<Unit> attackableUnits = new List<Unit>();
    public List<Tile> reachableTiles = new List<Tile>();
    #endregion
    #region Properties
    public int health = 100;
	public int ammo;    
    public int fuel;
    #endregion
    
    #region Basic Methods
    public void Init()
    {
        AnimationController = this.GetComponent<Unit_AnimationController>();
        ammo = data.maxAmmo;
        fuel = data.maxFuel;
    }
    //Ends the turn for the unit.
    public void Wait()
    {
        Deactivate();
        CalcVisibleArea();
        Tile currentTile = Core.Model.GetTile(this.position);       
        if(targetTile != null)
        {
            currentTile.SetUnitHere(null);//Reset the unitStandingHere property of the old tile to null
            currentTile.ResetTakeOverCounter();//Reset the take over counter 
            position = targetTile.position;
            targetTile.SetUnitHere(this);
        }
        targetTile = null;
    }

    //Activate the unit so it has turn, can fire and move.
    public void Activate()
    {
        CanMove = true;
        CanFire = true;
        hasTurn = true;
        IsInterrupted = false;       
    }
    public void Deactivate()
    {
        CanMove = false;
        CanFire = false;
        hasTurn = false;
    }
    

    public void SetVisibility(bool value)
    {
        gfx.GetComponent<MeshRenderer>().enabled = value;
        healthText.GetComponent<MeshRenderer>().enabled = value;
        GetComponent<BoxCollider>().enabled = value;
    }
    public void SetTeamColor(Color color)
    {
        gfx.GetComponent<MeshRenderer>().materials[0].color = color;
    }
    #endregion
    #region Damage Methods
    public void SubtractHealth(int healthToSubtract)
    {
        health = health - healthToSubtract;
        DisplayHealth(true);
        if(health <= 0)
        {
            health = 0;
            KillUnit();
        }
    }    

    //Displays the actual lifepoints in the "3D"TextMesh
    public void DisplayHealth(bool value)
    {
        if(value)
        {
            healthText.gameObject.SetActive(true);
            //Reposition the health text so it is always at the lower left corner of the tile.
            healthText.transform.SetPositionAndRotation(new Vector3(this.transform.position.x - 0.4f,this.transform.position.y, this.transform.position.z - 0.2f), Quaternion.Euler(90, 0, 0));
            healthText.text = GetCorrectedHealth().ToString();
        }
        else
        {
            healthText.gameObject.SetActive(false);
        }
    }
    //If the health goes below five the value will be rounded to 0, but we dont want that!
    public int GetCorrectedHealth()
    {
        if (health > 10)return (int)(health / 10);      
        else return 1;       
    }

    //Destroys the unit
    public void KillUnit()
    {
        //Set the unit standing on this tile as null.
        Core.Model.GetTile(position).SetUnitHere(null);
        //Boom animation
        
        //Remove unit from team list
        team.units.Remove(this);
        //If this was the last unit of the player the game is lost.
        if (team.units.Count <= 0)
        {
            //TODO: Mit nem event lösen!
            Debug.Log(enemyTeams[0] + " won by killing all their foes!");
        }
        //Finally delete the unit.
        Destroy(this.gameObject);
    }
    public IEnumerator KillUnitDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        KillUnit();
    }   
    #endregion
    #region Movement
    //Move the unit to a field and align it so it faces away, from where it came.
    public void MoveUnitTo(Vector2Int newPos)
    {
        //Setup the sequencer for the movement animation.
        AnimationController.Init();
        IsInterrupted = Core.Controller.ArrowBuilder.GetInterruption();
        if (IsInterrupted) targetTile = Core.Controller.ArrowBuilder.GetInterruptionTile();
        else targetTile = Core.Model.GetTile(newPos);
        DisplayHealth(false);//While moving we dont want to see the health.
        //Delete the reachable tiles and the movement arrow.
        Core.Controller.ClearReachableArea(this);
        Core.Controller.ArrowBuilder.ResetAll();

        if(data.rangeAttack) CanFire = false;           
        CanMove = false;        
    }

    //Resets the position and rotation of the unit to where it was before. (If we click the right mouse button or close the menu after we successfully moved it somewhere.)
    public void ResetPosition()
    {
        targetTile = null;
        //Set the position and rotation of the unit to where it was before
        this.transform.position = new Vector3(position.x, 0, position.y);
        //this.position = prePosition;
        RotateUnit(direction);
        //this.direction = preDirection;        
        Core.View.statusPanel.UpdateDisplay(this);//When the unit moves back, the display of the cover should be set to the old tile.
        //Core.Model.GetTile(position).SetUnitHere(this);//Inform the old tile, that we are back.
        DisplayHealth(true);//Repostition the health indicator.   
        CanFire = true;
        CanMove = true;
    }

    
    #endregion
    #region Rotation
    //Rotate the unit so it faces north, east, south or west.
    public void RotateUnit(Direction newDirection)
    {       
        direction = newDirection;
        switch (newDirection)
        {
            case Direction.North: this.transform.rotation = Quaternion.Euler(0, 0, 0); break;
            case Direction.East: this.transform.rotation = Quaternion.Euler(0, 90, 0); break;
            case Direction.South: this.transform.rotation = Quaternion.Euler(0, 180, 0); break;
            case Direction.West: this.transform.rotation = Quaternion.Euler(0, 270, 0); break;
            default:
                break;
        }
        DisplayHealth(true);
    }

    //Sets the facing direction depending on the transforms rotation.
    public void SetFacingDirection(float angle)
    {
        if(-1 < angle && angle < 1)
        {
            direction = Direction.North;
        }
        if (89 < angle && angle < 91)
        {
            direction = Direction.East;
        }
        if (179 < angle && angle < 181)
        {
            direction = Direction.South;
        }
        if (269 < angle && angle < 271)
        {
            direction = Direction.West;
        }
    }
    //Aligns the unit so it faces the direction of the given coordinates. 
    //TODO: add alignment for range units
    public void AlignUnit(Vector2Int fromPos, Vector2Int toPos)
    {
        if (data.directAttack)
        {
            //Face right
            if (fromPos.x < toPos.x && fromPos.y == toPos.y)
            {
                RotateUnit(Direction.East);
            }
            //Face left
            else if (fromPos.x > toPos.x && fromPos.y == toPos.y)
            {
                RotateUnit(Direction.West);
            }
            //Face up
            else if (fromPos.y < toPos.y && fromPos.x == toPos.x)
            {
                RotateUnit(Direction.North);
            }
            //Face down
            else if (fromPos.y > toPos.y && fromPos.x == toPos.x)
            {
                RotateUnit(Direction.South);
            }
        }
        if (data.rangeAttack)
        {
            //How to decide, if the unit to face is e.g. more to the left than to the bottom ?
            //Compare distances
            //For direct attack only!
            //Face right
            if (fromPos.x < toPos.x && fromPos.y == toPos.y)
            {
                RotateUnit(Direction.East);
            }
            //Face left
            else if (fromPos.x > toPos.x && fromPos.y == toPos.y)
            {
                RotateUnit(Direction.West);
            }
            //Face up
            else if (fromPos.y < toPos.y && fromPos.x == toPos.x)
            {
                RotateUnit(Direction.North);
            }
            //Face down
            else if (fromPos.y > toPos.y && fromPos.x == toPos.x)
            {
                RotateUnit(Direction.South);
            }
        }
        DisplayHealth(true);
    }
    #endregion
    #region Enemy
    //Checks the attackable tiles for enemies.
    public void FindAttackableEnemies()
    {
        foreach (Tile tile in attackableTiles)
        {
            if(IsVisibleEnemyHere(tile))
            {
                Unit enemy = tile.unitStandingHere;
                if (data.GetDamageAgainst(enemy.data.type) > 0) attackableUnits.Add(enemy);
            }
        }        
    }
    public bool IsEnemy(Unit unit)
    {
        if (enemyTeams.Contains(unit.team)) return true;
        else return false;
    }
    //Checks if an enemy is standing on this tile.
    bool IsVisibleEnemyHere(Tile tile)
    {
        if(tile.GetUnitHere() != null && tile.isVisible)
        {
            Unit possibleEnemy = tile.GetUnitHere();
            if (IsEnemy(possibleEnemy)) return true;            
        }
        return false;
    }   
    
    #endregion       
    #region Attackable Area
    //Sets the attackable tiles to active or inactive, so they are visible or not.
    public void DisplayAttackableTiles(bool value)
    {        
        if (attackableTiles.Count > 0)
        {
            foreach (Tile tile in attackableTiles)
            {
                if(tile != null) tile.gameObject.SetActive(value);
            }
        }
        else throw new System.Exception("Attackable tiles list is empty!");       
    } 

    //Creates a list of tiles the unit can attack.
    public void CalcAttackableArea(Vector2Int position)
    {
        attackableTiles.Clear();
        if(data.directAttack) FindAttackableTilesForDirectAttack(position);           
        else if(data.rangeAttack) FindAttackableTilesForRangedAttack();       
    }

    //Calculates the attackable tiles for direct attack units.
    void FindAttackableTilesForDirectAttack(Vector2Int position)
    {
        Vector2Int left = new Vector2Int(position.x - 1, position.y);
        TryToAddAttackableTile(left);
        Vector2Int right = new Vector2Int(position.x + 1, position.y);
        TryToAddAttackableTile(right);
        Vector2Int Top = new Vector2Int(position.x, position.y + 1);
        TryToAddAttackableTile(Top);
        Vector2Int Bottom = new Vector2Int(position.x, position.y -1);
        TryToAddAttackableTile(Bottom);       
    }

    void FindAttackableTilesForRangedAttack()
    {
        //First we mark every visible tile as attackable...
        for (int i = 1; i <= data.maxRange; i++)
        {
            //Right...
            Vector2Int testPosition = new Vector2Int(this.position.x + i, this.position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToAddAttackableTile(testPosition);
                for (int j = 1; j <= data.maxRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.position.x + i, this.position.y + j);
                    TryToAddAttackableTile(testPosition);
                    //... and down.
                    testPosition = new Vector2Int(this.position.x + i, this.position.y - j);
                    TryToAddAttackableTile(testPosition);
                }
            }
            //Left...
            testPosition = new Vector2Int(this.position.x - i, this.position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToAddAttackableTile(testPosition);
                for (int j = 1; j <= data.maxRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.position.x - i, this.position.y + j);
                    TryToAddAttackableTile(testPosition);
                    //... and down.
                    testPosition = new Vector2Int(this.position.x - i, this.position.y - j);
                    TryToAddAttackableTile(testPosition);
                }
            }
            testPosition = new Vector2Int(this.position.x, this.position.y + i);
            TryToAddAttackableTile(testPosition);
            testPosition = new Vector2Int(this.position.x, this.position.y - i);
            TryToAddAttackableTile(testPosition);
        }
        //...then we remove all tiles in the minimum range. (Need a better solution for this!)
        for (int i = 1; i < data.minRange; i++)
        {
            //Right...
            Vector2Int testPosition = new Vector2Int(this.position.x + i, this.position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToRemoveAttackableTile(testPosition);
                for (int j = 1; j < data.minRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.position.x + i, this.position.y + j);
                    TryToRemoveAttackableTile(testPosition);
                    //... and down.
                    testPosition = new Vector2Int(this.position.x + i, this.position.y - j);
                    TryToRemoveAttackableTile(testPosition);
                }
            }
            //Left...
            testPosition = new Vector2Int(this.position.x - i, this.position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToRemoveAttackableTile(testPosition);
                for (int j = 1; j < data.minRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.position.x - i, this.position.y + j);
                    TryToRemoveAttackableTile(testPosition);
                    //... and down.
                    testPosition = new Vector2Int(this.position.x - i, this.position.y - j);
                    TryToRemoveAttackableTile(testPosition);
                }
            }
        }        
    }

    void TryToAddAttackableTile(Vector2Int position)
    {
        if(Core.Model.IsOnMap(position))
        {
            Tile tile = Core.Model.GetTile(position);
            attackableTiles.Add(tile);
        }
    }
    void TryToRemoveAttackableTile(Vector2Int position)
    {
        if (Core.Model.IsOnMap(position))
        {
            Tile tile = Core.Model.GetTile(position);
            attackableTiles.Remove(tile);
        }
    }
    
    
    #endregion
    #region Reachable Area
    //Calculate how far you can move from a certain position depending on your movement points.
    public void CalcReachableArea(Vector2Int position, int movementPoints, UnitMoveType moveType, Tile cameFromTile)
    {
        //Debug.Log(counter);
        //counter += Time.deltaTime;
        Tile tile = Core.Model.GetTile(position);

        movementPoints = movementPoints - tile.data.GetMovementCost(moveType);

        //If enough movement points are left and the tile is passable (we can move through our own units, but are blocked by enemies), do the recursion.
        if ((movementPoints >= 0) && (tile.data.GetMovementCost(moveType) > 0) && !IsVisibleEnemyHere(tile))
        {
            if(!reachableTiles.Contains(tile)) reachableTiles.Add(tile);        

            //The tile was reached, so test all its neighbors for reachability. Ignore the tile you came from.
            foreach (Tile neighbor in tile.neighbors)
            {
                if(neighbor != cameFromTile) CalcReachableArea(neighbor.position, movementPoints, moveType, tile);
            }           
        }
    }
    #endregion
    #region Visible Area
    //Calculates the visible area of this unit depending on its vision range and marks the visible tiles in the graph.
    public void CalcVisibleArea()
    {
        if (Core.Model.MapSettings.fogOfWar)
        {
            Core.Model.SetVisibility(position, true);//Mark own position as visible.
            //Adjacent tiles are always visible
            Vector2Int left = new Vector2Int(position.x - 1, position.y);
            if (Core.Model.IsOnMap(left)) Core.Model.SetVisibility(left, true);
            Vector2Int right = new Vector2Int(position.x + 1, position.y);
            if (Core.Model.IsOnMap(right)) Core.Model.SetVisibility(right, true);
            Vector2Int up = new Vector2Int(position.x, position.y + 1);
            if (Core.Model.IsOnMap(up)) Core.Model.SetVisibility(up, true);
            Vector2Int down = new Vector2Int(position.x, position.y - 1);
            if (Core.Model.IsOnMap(down)) Core.Model.SetVisibility(down, true);

            for (int i = 1; i <= data.visionRange; i++)
            {
                //Right...
                Vector2Int testPosition = new Vector2Int(this.position.x + i, this.position.y);
                if(Core.Model.IsOnMap(testPosition))
                {
                    Core.Model.SetVisibility(testPosition, true);                    
                    for (int j = 1; j <= data.visionRange - i; j++)
                    {
                        //... and up.
                        testPosition = new Vector2Int(this.position.x + i, this.position.y + j);
                        TryToSetVisiblity(testPosition);
                        //... and down.
                        testPosition = new Vector2Int(this.position.x + i, this.position.y - j);
                        TryToSetVisiblity(testPosition);                       
                    }
                }
                //Left...
                testPosition = new Vector2Int(this.position.x - i, this.position.y);
                if (Core.Model.IsOnMap(testPosition))
                {
                    Core.Model.SetVisibility(testPosition, true);
                    for (int j = 1; j <= data.visionRange - i; j++)
                    {
                        //... and up.
                        testPosition = new Vector2Int(this.position.x - i, this.position.y + j);
                        TryToSetVisiblity(testPosition);
                        //... and down.
                        testPosition = new Vector2Int(this.position.x - i, this.position.y - j);
                        TryToSetVisiblity(testPosition);
                    }
                }

                testPosition = new Vector2Int(this.position.x, this.position.y + i);
                TryToSetVisiblity(testPosition);
                testPosition = new Vector2Int(this.position.x, this.position.y - i);
                TryToSetVisiblity(testPosition);

            }
        }        
    }
    void TryToSetVisiblity(Vector2Int position)
    {
        if (Core.Model.IsOnMap(position))
        {
            if (Core.Model.GetTile(position).data.type != TileType.Forest) Core.Model.SetVisibility(position, true);
        }
    }
    #endregion 
    

}
