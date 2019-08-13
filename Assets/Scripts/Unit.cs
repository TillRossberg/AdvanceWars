//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    public bool HasTurn = false;//The unit is ready for action.
    public bool CanMove = false;//States if the unit has already moved this turn.
    public bool CanFire = false;//Some units can't fire after they have moved.
    public bool IsInterrupted { get; private set; }//If we move through terrain that is covered by fog of war, we can be interrupted by an invisible enemy unit that is on our arrowpath.   
    Tile _interruptionTile;
    #endregion
    #region Tiles
    List<Unit> _attackableUnits = new List<Unit>();
    List<Tile> attackableTiles = new List<Tile>();
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
        AnimationController.OnRotationComplete += RotateToFinished;
    }
    private void OnDestroy()
    {
        AnimationController.OnReachedLastWayPoint -= MoveToFinished;
        AnimationController.OnRotationComplete -= RotateToFinished;

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
        HasTurn = true;
        IsInterrupted = false;
    }
    public void Deactivate()
    {
        CanMove = false;
        CanFire = false;
        HasTurn = false;
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
        if (team.Units.Contains(unit)) return true;
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
            AnimationController.PlayDestroyEffect();
            StartCoroutine(KillDelayed(this, AnimationController.DestroyEffect.main.duration / 2));
        }
    }
    IEnumerator KillDelayed(Unit unit, float delay)
    {
        yield return new WaitForSeconds(delay);
        Core.Controller.KillUnit(this);
    }
    public void SetHealth(int amount)
    {
        health = amount;
        UpdateHealth();
    }
    //Displays the actual lifepoints in the "3D"TextMesh
    public void DisplayHealth(bool value)
    {
        healthText.gameObject.SetActive(value);
        UpdateHealth();      
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
    #region Movement    
    //Move the unit to a field and align it so it faces away, from where it came.
    public void MoveTo(Vector2Int newPos)
    {
        //Calculate path
        List<Tile> path = Core.Model.AStar.GetPath(this, CurrentTile, Core.Model.GetTile(newPos), true);
        Core.Controller.ArrowBuilder.SetPath(path);
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
                GetAttackableEnemies(Core.Controller.SelectedTile.Position);
                if (!team.IsAI) Core.View.ContextMenu.Show(this);
            }
        }
        else BeInterrupted();
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
        Core.Model.GetTile(Position).UnitHere = null;
        SetPosition(pos);
        SetDirection(this.transform.eulerAngles.y);
        SetCurrentTile(pos);
        Core.View.StatusPanel.UpdateDisplay(this);
        Core.Controller.CurrentMode = Controller.Mode.Normal;
    }
    public void SetCurrentTile(Vector2Int pos)
    {
        this.CurrentTile = Core.Model.GetTile(pos);
        Core.Model.GetTile(pos).UnitHere = this;
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
    public void RotateAndAttack(Unit unit)
    {
        DisplayHealth(false);
        AnimationController.InitRotation(unit);
        Wait();
    }
    void RotateToFinished(Unit unit)
    {
        Attack(unit);
    }
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
    public List<Unit> GetAttackableEnemies(Vector2Int pos)
    {
        List<Unit> tempList = new List<Unit>();
        CalcAttackableArea(pos);
        ClearAttackableEnemies();
        foreach (Tile tile in attackableTiles)
        {
            if (IsVisibleEnemyHere(tile))
            {
                Unit enemy = tile.UnitHere;
                if (data.GetDamageAgainst(enemy.data.type) > 0) tempList.Add(enemy);
            }
        }
        _attackableUnits = tempList;
        return tempList;
    }
    public List<Unit> GetAttackableUnits()
    {
        return _attackableUnits;
    }
    public void ClearAttackableEnemies()
    {
        _attackableUnits.Clear();
    }

    public bool IsMyEnemy(Unit unit)
    {
        if (unit == null) return false;
        if (enemyTeams.Contains(unit.team)) return true;
        else return false;
    }
    //Checks if an enemy is standing on this tile.
    public bool IsVisibleEnemyHere(Tile tile)
    {
        if (tile.UnitHere != null && tile.IsVisible)
        {
            Unit possibleEnemy = tile.UnitHere;
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
    #region Attack
    public void Attack(Unit unit)
    {
        Core.Model.BattleCalculations.Fight(this, unit);
        unit.AnimationController.PlayDamageEffect();
        this.AnimationController.PlayAttackAnimation();
        Core.Controller.Cursor.SetPosition(Position);
        Core.Controller.Cursor.SetCursorGfx(0);
    }
   
    #endregion
    #region Attackable Area
    public void ShowAttackableTiles(Vector2Int pos)
    {
        ClearAttackableTiles();
        if (data.directAttack) CreateAttackableTilesGfx(GetAttackableTilesDirectAttack2(pos));
        else if (data.rangeAttack) CreateAttackableTilesGfx(GetAttackableTilesRangedAttack());
    }
    //Creates a list of tiles the unit can attack.
    void CalcAttackableArea(Vector2Int position)
    {
        ClearAttackableTiles();
        if (data.directAttack) attackableTiles = GetAttackableTilesDirectAttack1(position);
        else if (data.rangeAttack) attackableTiles = GetAttackableTilesRangedAttack();
    }       
    public void ClearAttackableTiles()
    {
        foreach (GameObject gfx in _attackableTilesGfx) Destroy(gfx.gameObject);
        _attackableTilesGfx.Clear();
        attackableTiles.Clear();
    }
    //Calculates the attackable tiles for direct attack units.
    List<Tile> GetAttackableTilesDirectAttack1(Vector2Int position)
    {
        List<Tile> tempList = new List<Tile>();
        Vector2Int left = new Vector2Int(position.x - 1, position.y);
        TryToAddAttackableTile(left, tempList);
        Vector2Int right = new Vector2Int(position.x + 1, position.y);
        TryToAddAttackableTile(right, tempList);
        Vector2Int Top = new Vector2Int(position.x, position.y + 1);
        TryToAddAttackableTile(Top, tempList);
        Vector2Int Bottom = new Vector2Int(position.x, position.y - 1);
        TryToAddAttackableTile(Bottom, tempList);
        return tempList;
    }
    public List<Tile> GetAttackableTilesDirectAttack2(Vector2Int pos)
    {
        ClearAttackableTiles();
        ClearReachableTiles();
        CalcReachableArea(pos, data.moveDist, data.moveType, null);
        return AddUpAttackableTile(_reachableTiles);
    }
    List<Tile> AddUpAttackableTile(List<Tile> reachableTiles)
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile item in reachableTiles)
        {
            foreach (Tile neighbor in item.Neighbors)
                if (!reachableTiles.Contains(neighbor) && !tempList.Contains(neighbor))
                    tempList.Add(neighbor);
            tempList.Add(item);
        }      
        return tempList;
    }
    public List<Tile> GetAttackableTilesRangedAttack()
    {
        List<Tile> tempList = new List<Tile>();
        //First we mark every visible tile as attackable...
        for (int i = 1; i <= data.maxRange; i++)
        {
            //Right...
            Vector2Int testPosition = new Vector2Int(this.Position.x + i, this.Position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToAddAttackableTile(testPosition, tempList);
                for (int j = 1; j <= data.maxRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.Position.x + i, this.Position.y + j);
                    TryToAddAttackableTile(testPosition, tempList);
                    //... and down.
                    testPosition = new Vector2Int(this.Position.x + i, this.Position.y - j);
                    TryToAddAttackableTile(testPosition, tempList);
                }
            }
            //Left...
            testPosition = new Vector2Int(this.Position.x - i, this.Position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToAddAttackableTile(testPosition, tempList);
                for (int j = 1; j <= data.maxRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.Position.x - i, this.Position.y + j);
                    TryToAddAttackableTile(testPosition, tempList);
                    //... and down.
                    testPosition = new Vector2Int(this.Position.x - i, this.Position.y - j);
                    TryToAddAttackableTile(testPosition, tempList);
                }
            }
            testPosition = new Vector2Int(this.Position.x, this.Position.y + i);
            TryToAddAttackableTile(testPosition, tempList);
            testPosition = new Vector2Int(this.Position.x, this.Position.y - i);
            TryToAddAttackableTile(testPosition, tempList);
        }
        //...then we remove all tiles in the minimum range. (Need a better solution for this!)
        for (int i = 1; i < data.minRange; i++)
        {
            //Right...
            Vector2Int testPosition = new Vector2Int(this.Position.x + i, this.Position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToRemoveAttackableTile(testPosition, tempList);
                for (int j = 1; j < data.minRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.Position.x + i, this.Position.y + j);
                    TryToRemoveAttackableTile(testPosition, tempList);
                    //... and down.
                    testPosition = new Vector2Int(this.Position.x + i, this.Position.y - j);
                    TryToRemoveAttackableTile(testPosition, tempList);
                }
            }
            //Left...
            testPosition = new Vector2Int(this.Position.x - i, this.Position.y);
            if (Core.Model.IsOnMap(testPosition))
            {
                TryToRemoveAttackableTile(testPosition, tempList);
                for (int j = 1; j < data.minRange - i; j++)
                {
                    //... and up.
                    testPosition = new Vector2Int(this.Position.x - i, this.Position.y + j);
                    TryToRemoveAttackableTile(testPosition, tempList);
                    //... and down.
                    testPosition = new Vector2Int(this.Position.x - i, this.Position.y - j);
                    TryToRemoveAttackableTile(testPosition, tempList);
                }
            }
            //Up
            testPosition = new Vector2Int(this.Position.x, this.Position.y + i);
            if (Core.Model.IsOnMap(testPosition)) TryToRemoveAttackableTile(testPosition, tempList);
            //Down
            testPosition = new Vector2Int(this.Position.x, this.Position.y - i);
            if (Core.Model.IsOnMap(testPosition)) TryToRemoveAttackableTile(testPosition, tempList);
        }
        return tempList;
    }
    void TryToAddAttackableTile(Vector2Int position, List<Tile> tiles)
    {
        if (Core.Model.IsOnMap(position))
        {
            Tile tile = Core.Model.GetTile(position);
            tiles.Add(tile);
        }
    }
    void TryToRemoveAttackableTile(Vector2Int position, List<Tile> tiles)
    {
        if (Core.Model.IsOnMap(position))
        {
            Tile tile = Core.Model.GetTile(position);
            tiles.Remove(tile);
        }
    }

    //Creates the graphics for the tiles, that can be attacked by the unit.
    void CreateAttackableTilesGfx(List< Tile> tiles)
    {
        foreach (Tile tile in tiles) _attackableTilesGfx.Add(Instantiate(Core.Model.Database.attackableTilePrefab, new Vector3(tile.Position.x, 0.1f, tile.Position.y), Quaternion.identity, this.transform));        
    }
    public List<Unit> GetReachableEnemies(Vector2Int pos)
    {
        List<Unit> tempList = new List<Unit>();
        ClearReachableTiles();
        CalcAttackableArea(pos);
        foreach (Tile item in attackableTiles) if (IsMyEnemy(item.UnitHere)) tempList.Add(item.UnitHere);
        return tempList;
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
            //TODO: also implement fuel
        }
        //If enough movement points are left and the tile is passable (we can move through our own units, but are blocked by enemies), do the recursion.
        if ((movementPoints >= 0) && (tile.data.GetMovementCost(moveType) > 0) && !IsVisibleEnemyHere(tile))
        {
            if (!_reachableTiles.Contains(tile)) _reachableTiles.Add(tile);

            //The tile was reached, so test all its neighbors for reachability. Ignore the tile you came from.
            foreach (Tile neighbor in tile.Neighbors)
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
        Core.Model.GetTile(this.Position).UnitHere = null;

    }
    public void GetUnloaded(Tile tile)
    {
        this.SetPosition(tile.Position);
        CurrentTile = tile;
        tile.UnitHere = this;
        CalcVisibleArea();
        Deactivate();
    }
    
    #endregion
    #region Unite Methods
    public void Unite(Unit unit)
    {       
        unit.AddHealth(this.health);
        unit.UpdateHealth();
        Core.Controller.KillUnit(this);
    }
    public bool CanUnite(Unit unit)
    {
        if (unit.data.type == data.type && health < 100) return true;
        else return false;
    }
    #endregion
    #region Interuption
    public void BeInterrupted()
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
    #region Conditions
    public bool IsCopterUnit()
    {
        if (data.type == UnitType.BCopter || data.type == UnitType.TCopter) return true;
        else return false;
    }
    public bool IsInfantry()
    {
        if (data.type == UnitType.Infantry || data.type == UnitType.Mech) return true;
        else return false;
    }
    public bool IsAt(Tile tile)
    {
        if (CurrentTile == tile) return true;
        else return false;        
    }
    public bool CanAttack(Unit unit)
    {
        if(data.directAttack)
        {
            foreach (Tile item in CurrentTile.Neighbors)if (item.UnitHere == unit) return true;           
        }
        if(data.rangeAttack)
        {
            CalcAttackableArea(this.Position);
            foreach (Tile item in attackableTiles)if (item.UnitHere == unit) return true;          
        }
        return false;
    }
    public bool IsInRadius(Tile tile, float radius)
    {
        if (Vector3.Distance(CurrentTile.transform.position, tile.transform.position) <= radius) return true;
        else return false;
    }
    #endregion
}
