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
    #endregion
    #region Position & Rotation
    public Vector2Int Position;
    public Tile CurrentTile;
    public int rotation;
    public Direction direction;
    float counter = 0;//Counts the iterations of the calcReachableArea algorithm.
    #endregion
    #region States
    public bool hasTurn = false;//The unit is ready for action.
    public bool CanMove = false;//States if the unit has already moved this turn.
    public bool CanFire = false;//Some units can't fire after they have moved.
    public bool IsInterrupted { get; private set; }//If we move through terrain that is covered by fog of war, we can be interrupted by an invisible enemy unit that is on our arrowpath.   
    Tile _interruptionTile;
    #endregion
    #region Tiles
    List<Unit> _attackableUnits = new List<Unit>();
    List<Tile> _attackableTiles = new List<Tile>();
    List<GameObject> _attackableTilesGfx = new List<GameObject>();
    bool _isShowingAttackableTiles = false;
    List<Tile> _reachableTiles = new List<Tile>();
    List<GameObject> _reachableTilesGfx = new List<GameObject>();
    #endregion
    #region Properties
    public int health = 100;
    public int ammo;
    public int fuel;
    public bool WantsToBeLoaded = false;
    public bool WantsToUnite = false;
    #endregion    

    #region Basic Methods
    public void Init()
    {
        AnimationController = this.GetComponent<Unit_AnimationController>();
        ammo = data.primaryAmmo;
        fuel = data.maxFuel;
        AnimationController.OnReachedLastWayPoint += MoveToFinished;
    }
    private void OnDestroy()
    {
        AnimationController.OnReachedLastWayPoint -= MoveToFinished;
    }
    //Ends the turn for the unit.
    public void Wait()
    {
        Deactivate();
        CalcVisibleArea();
        ConfirmPosition(Core.Controller.GetSelectedPosition());
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
        //Material[] mats = gfx.GetComponent<MeshRenderer>().materials;

        //foreach (Material material in mats)
        //{            
        //    if (material.name == "TeamColor (Instance)") material.color = color;
        //}
        gfx.GetComponent<MeshRenderer>().materials[0].color = color;
    }
    public bool IsInMyTeam(Unit unit)
    {
        if (team.units.Contains(unit)) return true;
        else return false;
    }
    public bool IsGroundUnit()
    {
        if (data.type == UnitType.AntiAir ||
            data.type == UnitType.APC ||
            data.type == UnitType.Artillery ||
            data.type == UnitType.Infantry ||
            data.type == UnitType.MdTank ||
            data.type == UnitType.Mech ||
            data.type == UnitType.Missiles ||
            data.type == UnitType.Recon ||
            data.type == UnitType.Rockets ||
            data.type == UnitType.Tank ||
            data.type == UnitType.Titantank
          ) return true;
        else return false;
    }
    #endregion
    #region Health Methods
    public void AddHealth(int healing)
    {
        if ((health + healing) <= 100) health += healing;
        else health = 100;
    }
    public void SubtractHealth(int healthToSubtract)
    {
        health = health - healthToSubtract;
        DisplayHealth(true);
        if (health <= 0)
        {
            health = 0;
            KillUnit();
        }
    }
    public void SetHealth(int amount)
    {
        health = amount;
        UpdateHealth();
    }
    //Displays the actual lifepoints in the "3D"TextMesh
    public void DisplayHealth(bool value)
    {
        if (value)
        {
            healthText.gameObject.SetActive(true);
            UpdateHealth();
        }
        else
        {
            healthText.gameObject.SetActive(false);
        }
    }
    //If the health goes below five the value will be rounded to 0, but we dont want that!
    public int GetCorrectedHealth()
    {
        if (health > 10) return (int)(health / 10);
        else return 1;
    }
    void UpdateHealth()
    {
        //Reposition the health text so it is always at the lower left corner of the tile.
        healthText.transform.SetPositionAndRotation(new Vector3(this.transform.position.x - 0.4f, this.transform.position.y, this.transform.position.z - 0.2f), Quaternion.Euler(90, 0, 0));
        healthText.text = GetCorrectedHealth().ToString();
    }
    #endregion
    #region Kill Unit Methods
    //Destroys the unit
    public void KillUnit()
    {
        //Set the unit standing on this tile as null.
        Core.Model.GetTile(Position).SetUnitHere(null);
        //TODO: Boom animation

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
    public void MoveTo(Vector2Int newPos)
    {
        //Setup the sequencer for the movement animation.
        AnimationController.InitMovement();
        IsInterrupted = Core.Controller.ArrowBuilder.GetInterruption();
        DisplayHealth(false);//While moving we dont want to see the health.
        //Delete the reachable tiles and the movement arrow.
        ClearReachableTiles();
        _interruptionTile = Core.Controller.ArrowBuilder.GetInterruptionTile();
        Core.Controller.ArrowBuilder.ResetAll();
        if (data.rangeAttack) CanFire = false;
        CanMove = false;
    }
    public void MoveToFinished()
    {
        if (!IsInterrupted)
        {
            //TODO: add event to inform that we reached the last waypoint.
            if (WantsToBeLoaded) Core.View.ContextMenu.ShowLoadButton();
            else if (WantsToUnite) Core.View.ContextMenu.ShowUniteButton();
            else
            {
                FindAttackableEnemies(Core.Controller.SelectedTile.Position);
                Core.View.ContextMenu.Show(this);
            }
        }
        else GetInterrupted();
        WantsToBeLoaded = false;
        WantsToUnite = false;
    }
    public void MoveUnitToLoad(Vector2Int newPos)
    {
        WantsToBeLoaded = true;
        MoveTo(newPos);
    }
    public void MoveUnitToUnite(Vector2Int newPos)
    {
        WantsToUnite = true;
        MoveTo(newPos);
    }
    public void SetPosition(Vector2Int pos)
    {
        this.transform.position = new Vector3(pos.x, 0, pos.y);
        this.Position = pos;
    }
    public void ConfirmPosition(Vector2Int pos)
    {
        Core.Model.GetTile(Position).SetUnitHere(null);
        SetPosition(pos);
        SetDirection(this.transform.eulerAngles.y);
        SetCurrentTile(pos);
        Core.View.StatusPanel.UpdateDisplay(this);
        Core.Controller.CurrentMode = Controller.Mode.Normal;
    }
    public void SetCurrentTile(Vector2Int pos)
    {
        this.CurrentTile = Core.Model.GetTile(pos);
        Core.Model.GetTile(pos).SetUnitHere(this);
    }
    //Resets the position and rotation of the unit to where it was before. (If we click the right mouse button or close the menu after we successfully moved it somewhere.)
    public void ResetPosition()
    {
        SetPosition(this.Position);
        Core.View.StatusPanel.UpdateDisplay(this);//When the unit moves back, the display of the cover should be set to the old tile.
        DisplayHealth(true);//Repostition the health indicator.   
        CanFire = true;
        CanMove = true;
    }
    public void ResetRotation()
    {
        RotateUnit(direction);
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
    public void SetDirection(float angle)
    {
        if (-2 < angle && angle < 2) direction = Direction.North;
        if (88 < angle && angle < 92) direction = Direction.East;
        if (178 < angle && angle < 182) direction = Direction.South;
        if (268 < angle && angle < 272) direction = Direction.West;
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
    public void FindAttackableEnemies(Vector2Int pos)
    {
        CalcAttackableArea(pos);
        foreach (Tile tile in _attackableTiles)
        {
            if (IsVisibleEnemyHere(tile))
            {
                Unit enemy = tile.unitStandingHere;
                if (data.GetDamageAgainst(enemy.data.type) > 0) _attackableUnits.Add(enemy);
            }
        }
    }
    public void ClearAttackableEnemies()
    {
        _attackableUnits.Clear();
    }

    public bool IsMyEnemy(Unit unit)
    {
        if (enemyTeams.Contains(unit.team)) return true;
        else return false;
    }
    //Checks if an enemy is standing on this tile.
    bool IsVisibleEnemyHere(Tile tile)
    {
        if (tile.GetUnitHere() != null && tile.isVisible)
        {
            Unit possibleEnemy = tile.GetUnitHere();
            if (IsMyEnemy(possibleEnemy)) return true;
        }
        return false;
    }
    public List<Tile> GetAttackableEnemyTiles()
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Unit enemy in _attackableUnits)
        {
            tempList.Add(enemy.CurrentTile);
        }
        return tempList;
    }
    public bool CanAttack()
    {
        if (CanFire && _attackableUnits.Count > 0) return true;
        else return false;
    }
    #endregion
    #region Attackable Area
    public void ShowAttackableTiles(Vector2Int pos)
    {
        CalcAttackableArea(pos);
        CreateAttackableTilesGfx();
    }
    public void ClearAttackableTiles()
    {
        foreach (GameObject gfx in _attackableTilesGfx) Destroy(gfx.gameObject);
        _attackableTilesGfx.Clear();
        _attackableTiles.Clear();
    }
    public void ToggleAttackableTilesVisiblity()
    {
        if(_attackableTiles.Count > 0)
        {

        }
        else
        {

        }
    }
    //Creates a list of tiles the unit can attack.
    void CalcAttackableArea(Vector2Int position)
    {
        if (data.directAttack) FindAttackableTilesForDirectAttack(position);
        else if (data.rangeAttack) FindAttackableTilesForRangedAttack();
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
        Vector2Int Bottom = new Vector2Int(position.x, position.y - 1);
        TryToAddAttackableTile(Bottom);
    }
    void FindAttackableTilesForRangedAttack()
    {
        //First we mark every visible tile as attackable...
        for (int i = 1; i <= data.maxRange; i++)
        {
            //Right...
            Vector2Int testPosition = new Vector2Int(this.Position.x + i, this.Position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToAddAttackableTile(testPosition);
                for (int j = 1; j <= data.maxRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.Position.x + i, this.Position.y + j);
                    TryToAddAttackableTile(testPosition);
                    //... and down.
                    testPosition = new Vector2Int(this.Position.x + i, this.Position.y - j);
                    TryToAddAttackableTile(testPosition);
                }
            }
            //Left...
            testPosition = new Vector2Int(this.Position.x - i, this.Position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToAddAttackableTile(testPosition);
                for (int j = 1; j <= data.maxRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.Position.x - i, this.Position.y + j);
                    TryToAddAttackableTile(testPosition);
                    //... and down.
                    testPosition = new Vector2Int(this.Position.x - i, this.Position.y - j);
                    TryToAddAttackableTile(testPosition);
                }
            }
            testPosition = new Vector2Int(this.Position.x, this.Position.y + i);
            TryToAddAttackableTile(testPosition);
            testPosition = new Vector2Int(this.Position.x, this.Position.y - i);
            TryToAddAttackableTile(testPosition);
        }
        //...then we remove all tiles in the minimum range. (Need a better solution for this!)
        for (int i = 1; i < data.minRange; i++)
        {
            //Right...
            Vector2Int testPosition = new Vector2Int(this.Position.x + i, this.Position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToRemoveAttackableTile(testPosition);
                for (int j = 1; j < data.minRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.Position.x + i, this.Position.y + j);
                    TryToRemoveAttackableTile(testPosition);
                    //... and down.
                    testPosition = new Vector2Int(this.Position.x + i, this.Position.y - j);
                    TryToRemoveAttackableTile(testPosition);
                }
            }
            //Left...
            testPosition = new Vector2Int(this.Position.x - i, this.Position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToRemoveAttackableTile(testPosition);
                for (int j = 1; j < data.minRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.Position.x - i, this.Position.y + j);
                    TryToRemoveAttackableTile(testPosition);
                    //... and down.
                    testPosition = new Vector2Int(this.Position.x - i, this.Position.y - j);
                    TryToRemoveAttackableTile(testPosition);
                }
            }
            //Up
            testPosition = new Vector2Int(this.Position.x, this.Position.y + i);
            if (Core.Model.IsOnMap(testPosition)) TryToRemoveAttackableTile(testPosition);
            //Down
            testPosition = new Vector2Int(this.Position.x, this.Position.y - i);
            if (Core.Model.IsOnMap(testPosition)) TryToRemoveAttackableTile(testPosition);
        }
    }
    void TryToAddAttackableTile(Vector2Int position)
    {
        if (Core.Model.IsOnMap(position))
        {
            Tile tile = Core.Model.GetTile(position);
            _attackableTiles.Add(tile);
        }
    }
    void TryToRemoveAttackableTile(Vector2Int position)
    {
        if (Core.Model.IsOnMap(position))
        {
            Tile tile = Core.Model.GetTile(position);
            _attackableTiles.Remove(tile);
        }
    }

    //Creates the graphics for the tiles, that can be attacked by the unit.
    void CreateAttackableTilesGfx()
    {
        foreach (Tile tile in _attackableTiles) _attackableTilesGfx.Add(Instantiate(Core.Model.Database.attackableTilePrefab, new Vector3(tile.Position.x, 0.1f, tile.Position.y), Quaternion.identity, this.transform));        
    }    

    #endregion
    #region Reachable Area
    public void ShowReachableArea()
    {
        CalcReachableArea(this.Position, data.moveDist, data.moveType, null);
        CreateReachableTilesGfx();
    }
    public void ClearReachableTiles()
    {
        foreach (GameObject gfx in _reachableTilesGfx) Destroy(gfx.gameObject);        
        _reachableTilesGfx.Clear();
        _reachableTiles.Clear();
    }
    //Calculate how far you can move from a certain position depending on your movement points.
    void CalcReachableArea(Vector2Int position, int movementPoints, UnitMoveType moveType, Tile cameFromTile)
    {
        Tile tile = Core.Model.GetTile(position);

        if (cameFromTile != null)
        {
            movementPoints = movementPoints - tile.data.GetMovementCost(moveType);
            //TODO: add fuel
        }

        //If enough movement points are left and the tile is passable (we can move through our own units, but are blocked by enemies), do the recursion.
        if ((movementPoints >= 0) && (tile.data.GetMovementCost(moveType) > 0) && !IsVisibleEnemyHere(tile))
        {
            if (!_reachableTiles.Contains(tile)) _reachableTiles.Add(tile);

            //The tile was reached, so test all its neighbors for reachability. Ignore the tile you came from.
            foreach (Tile neighbor in tile.neighbors)
            {
                if (neighbor != cameFromTile) CalcReachableArea(neighbor.Position, movementPoints, moveType, tile);
            }
        }
    }
    //Draws the tiles, that can be reached.
    void CreateReachableTilesGfx()
    {
        foreach (Tile tile in _reachableTiles)_reachableTilesGfx.Add(Instantiate(Core.Model.Database.reachableTilePrefab, new Vector3(tile.Position.x, 0, tile.Position.y), Quaternion.identity, this.transform));       
    }
    public bool CanReachTile(Tile tile)
    {
        if (_reachableTiles.Contains(tile)) return true;
        else return false;
    }
    #endregion
    #region Visible Area
    //Calculates the visible area of this unit depending on its vision range and marks the visible tiles in the graph.
    public void CalcVisibleArea()
    {
        if (Core.Model.MapSettings.fogOfWar)
        {
            Core.Model.SetVisibility(Position, true);//Mark own position as visible.
            //Adjacent tiles are always visible
            Vector2Int left = new Vector2Int(Position.x - 1, Position.y);
            if (Core.Model.IsOnMap(left)) Core.Model.SetVisibility(left, true);
            Vector2Int right = new Vector2Int(Position.x + 1, Position.y);
            if (Core.Model.IsOnMap(right)) Core.Model.SetVisibility(right, true);
            Vector2Int up = new Vector2Int(Position.x, Position.y + 1);
            if (Core.Model.IsOnMap(up)) Core.Model.SetVisibility(up, true);
            Vector2Int down = new Vector2Int(Position.x, Position.y - 1);
            if (Core.Model.IsOnMap(down)) Core.Model.SetVisibility(down, true);

            for (int i = 1; i <= data.visionRange; i++)
            {
                //Right...
                Vector2Int testPosition = new Vector2Int(this.Position.x + i, this.Position.y);
                if (Core.Model.IsOnMap(testPosition))
                {
                    Core.Model.SetVisibility(testPosition, true);
                    for (int j = 1; j <= data.visionRange - i; j++)
                    {
                        //... and up.
                        testPosition = new Vector2Int(this.Position.x + i, this.Position.y + j);
                        TryToSetVisiblity(testPosition);
                        //... and down.
                        testPosition = new Vector2Int(this.Position.x + i, this.Position.y - j);
                        TryToSetVisiblity(testPosition);
                    }
                }
                //Left...
                testPosition = new Vector2Int(this.Position.x - i, this.Position.y);
                if (Core.Model.IsOnMap(testPosition))
                {
                    Core.Model.SetVisibility(testPosition, true);
                    for (int j = 1; j <= data.visionRange - i; j++)
                    {
                        //... and up.
                        testPosition = new Vector2Int(this.Position.x - i, this.Position.y + j);
                        TryToSetVisiblity(testPosition);
                        //... and down.
                        testPosition = new Vector2Int(this.Position.x - i, this.Position.y - j);
                        TryToSetVisiblity(testPosition);
                    }
                }

                testPosition = new Vector2Int(this.Position.x, this.Position.y + i);
                TryToSetVisiblity(testPosition);
                testPosition = new Vector2Int(this.Position.x, this.Position.y - i);
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
    #region Load Methods
    public bool CanLoadtUnits()
    {
        if (GetComponent<Unit_Transporter>()) return true;
        else return false;
    }   
    public void GetLoaded()
    {
        CurrentTile = null;
        Core.Model.GetTile(this.Position).SetUnitHere(null);

    }
    public void GetUnloaded(Tile tile)
    {
        this.SetPosition(tile.Position);
        CurrentTile = tile;
        tile.SetUnitHere(this);
        CalcVisibleArea();
        Deactivate();
    }
    
    public bool IsCopterUnit()
    {
        if (data.type == UnitType.BCopter || data.type == UnitType.TCopter) return true;
        else return false;
    }
    public bool IsInfantryUnit()
    {
        if (data.type == UnitType.Infantry || data.type == UnitType.Mech) return true;
        else return false;
    }
    #endregion
    #region Unite Methods
    public void Unite(Unit unit)
    {       
        unit.AddHealth(this.health);
        unit.UpdateHealth();
        this.KillUnit();
    }
    public bool CanUnite(Unit unit)
    {
        if (unit.data.type == data.type && health < 100) return true;
        else return false;
    }
    #endregion
    #region Interuption
    public void GetInterrupted()
    {
        ConfirmPosition(_interruptionTile.Position);
        _interruptionTile = null;
        CalcVisibleArea();
        Deactivate();
        //TODO: Show exclamation mark.
        //TODO: Stop move sound.
        //TODO: Play interruption sound
    }


    #endregion
}
