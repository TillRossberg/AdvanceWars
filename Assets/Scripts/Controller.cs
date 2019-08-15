using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Controller : MonoBehaviour
{
    #region References
    public Controller_Camera CameraController;
    public ArrowBuilder ArrowBuilder { get; private set; }
    public Controller_Cursor Cursor { get; private set; }
    #endregion

    public Team ActiveTeam { get; private set; }
    public Tile SelectedTile;
    public Unit SelectedUnit;
    public int RoundCounter { get; private set; }
    public Weather currentWeather;

    public enum Mode { Normal, Fire, Move, BuyMenu, ContextMenu, UnloadUnit, ShowTileDetails };
    public Mode CurrentMode;

    List<Tile> _tilesToCycle;
    Tile _targetTile;

    #region Debug
    public GameObject pathIndicator;
    public GameObject startIndicator;
    public GameObject endIndicator;
    public List<GameObject> indicators;
    #endregion

    #region Base Methods
    public void StartGame()
    {
        //Core.Model.InitMap();
        Core.Model.CreateTeam(Core.Model.Database.premadeTeams[0]);
        Core.Model.CreateTeam(Core.Model.Database.premadeTeams[1]);
        Core.Model.teams[0].AddEnemyTeam(Core.Model.teams[1]);
        Core.Model.teams[1].AddEnemyTeam(Core.Model.teams[0]);
        //Core.Model.LoadLevel01(15, 15);
        Core.Model.LoadLevel02(19, 13);
        Core.Model.InitTeams();
        Core.Model.LoadLevel02Units();
        //Core.Model.LoadLevel03(7, 5);
        Cursor = CreateCursor(new Vector2Int(1, 1));
        Core.Model.SetupRandomSuccession();
        ActiveTeam = Core.Model.Succession[0];
        StartTurn();
        //Core.AudioManager.PlayMusic(Core.Model.Database.sounds.music[0]);
    }

    public void Init()
    {
        ArrowBuilder = new ArrowBuilder(Core.Model.arrowPathParent);
        RoundCounter = 1;
        CurrentMode = Mode.Normal;
        currentWeather = Core.Model.MapData.startWeather;
    }
    //Define the succession and set the first team that has a turn.
    public void InitSuccession()
    {
        Core.Model.SetupRandomSuccession();
        ActiveTeam = Core.Model.Succession[0];
        ActivateUnits(ActiveTeam);
    }
    #endregion
    #region Turn
    //Start turn
    public void StartTurn()
    {
        ActivateUnits(ActiveTeam);
        Core.View.UpdateFogOfWar(ActiveTeam);//Set fog of war for this team.

        //Subtract fuel.

        //Give rations from properties and APCs.

        GiveMoneyForProperties(ActiveTeam);//Give money for properties.

        //Repair units.

        //Subtract money for repairing units.
        Core.View.CommanderPanel.UpdateDisplay();//Update the GUI for the active team.
        if(ActiveTeam.IsAI)
        {
            Core.View.StatusPanel.Hide();
            BlockInput(true);
            Cursor.DisplayCursorGfx(false);
            StartCoroutine(ActiveTeam.AI.StartTurnDelayed(.1f));
        }
        else
        {
            Core.View.StatusPanel.Show();
            BlockInput(false);
            Cursor.DisplayCursorGfx(true);
            Cursor.SetPosition(ActiveTeam.Units[0].Position);//Set cursors position to first unit
        }
    }
    IEnumerator StartTurnDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartTurn();
    }
    //End turn
    public void EndTurn()
    {
        BlockInputFor(.5f);
        Deselect();
        DeactivateUnits(ActiveTeam);
        if (Core.Model.IsLastInSuccession(ActiveTeam)) EndRound(); //Increase Round nr., change weather
        ActiveTeam = Core.Model.GetNextTeamInSuccession();
        StartTurn();
        //StartTurn();//For some reason the first unit is instantly selected!?!? So a delay is necessary -.-
    }


    //Give money for each property the team owns. 
    void GiveMoneyForProperties(Team team)
    {
        team.AddMoney(team.OwnedProperties.Count * Core.Model.MapSettings.moneyIncrement);
    }

    //Sets all the units of a team so they have a turn, can move and fire.
    void ActivateUnits(Team team)
    {
        foreach (Unit unit in team.Units)
        {
            if (unit != null) unit.Activate();
        }
    }

    //Set the properties of all units of a team so they don't have a turn.
    void DeactivateUnits(Team team)
    {
        foreach (Unit unit in team.Units)
        {
            if (unit != null) unit.Deactivate();
        }
    }

    //When all teams had their turn: change the weather (if random was selected), increase the round counter, check if the battle duration is ecxeeded
    void EndRound()
    {
        RoundCounter++;
        if (RoundCounter == Core.Model.MapSettings.battleDuration && Core.Model.MapSettings.battleDuration > 4)//Minimum for the duration of the battle is 5 rounds, if below this winning condition will never trigger.
        {
            //TODO: decide if more than two teams are playing and then only remove the defeated team from the map.

            //TODO: If the maximum amount of rounds has passed, check who won the game. (Depending on the occupied properties)
            Debug.Log("One of the teams won... I think...");
        }
        if (Core.Model.MapSettings.randomWeather) ChangeWeather(GetRandomWeather());
    }

    //Count the properties of all the teams and the team with the most wins.
    //TODO: !WORKING (try to solve this with only two teams first)
    Team GetTeamWithMostProperties()
    {
        Team winner = null;
        int highestPropertyCount = 0;
        foreach (Team team in Core.Model.teams)
        {
            int propertyCount = team.OwnedProperties.Count;
            if (propertyCount > highestPropertyCount)
            {
                highestPropertyCount = propertyCount;
                winner = team;
            }
        }
        return winner;
    }
    #endregion   
    #region A-Button Methods
    public void AButton()
    {
        Tile currentTile = Core.Model.GetTile(Cursor.Position);
        Unit unitHere = currentTile.UnitHere;
        switch (CurrentMode)
        {
            case Mode.Normal:
                //Select Unit
                if (unitHere != null)
                {
                    //Select own unit...
                    if (ActiveTeam == unitHere.team && unitHere.CanMove)
                    {
                        CurrentMode = Mode.Move;
                        Select(unitHere);
                        SelectedUnit.ShowReachableArea();
                        ArrowBuilder.StartPath(SelectedUnit);
                    }
                    //...or select already used unit or unit from enemy team.
                    else Core.AudioManager.PlaySFX(Core.Model.Database.Sounds.NopeSound);
                }
                //Select tile, that can produce units...
                else if (currentTile.CanProduceUnits() && currentTile.Property.OwningTeam == ActiveTeam) Core.View.BuyMenu.Show(currentTile);
                //...or select empty tile.
                else Core.View.TileDetails.Show(currentTile);
                break;
            case Mode.Fire:
                SelectedUnit.RotateAndAttack(_targetTile.UnitHere);
                Cursor.HideEstimatedDamage();
                ResetTilesToCycle();
                Deselect();
                break;
            case Mode.Move:
                //Select another unit you want the to interact with...
                if (unitHere != null)
                {
                    //Click on self
                    if (unitHere == SelectedUnit)
                    {
                        SelectedUnit.GetAttackableEnemies(SelectedUnit.Position);
                        Core.View.ContextMenu.Show(SelectedUnit);
                    }
                    //Click on transporter
                    else if (SelectedUnit.IsInMyTeam(unitHere) && unitHere.CanLoadtUnits())
                    {
                        if ((unitHere.data.type == UnitType.APC || unitHere.data.type == UnitType.TCopter) && SelectedUnit.IsInfantry()) SelectedUnit.MoveUnitToLoad(Cursor.Position);
                        if (unitHere.data.type == UnitType.Lander && SelectedUnit.IsGroundUnit()) SelectedUnit.MoveUnitToLoad(Cursor.Position);
                        if (unitHere.data.type == UnitType.Cruiser && SelectedUnit.IsCopterUnit()) SelectedUnit.MoveUnitToLoad(Cursor.Position);
                    }
                    //Click on unit of the same type to unite.
                    else if (unitHere.CanUnite(SelectedUnit) || SelectedUnit.CanUnite(unitHere)) SelectedUnit.MoveUnitToUnite(Cursor.Position);
                    //If you click on an enemy unit or an unit that is in an alliance with you.
                    else Core.AudioManager.PlaySFX(Core.Model.Database.Sounds.NopeSound);
                }
                //...or select an empty tile you want to go to.
                else
                {
                    if(SelectedUnit.CurrentTile.IsProperty()) StopOccupation(SelectedUnit.CurrentTile);
                    Select(currentTile);
                    SelectedUnit.MoveTo(currentTile.Position);
                }

                break;
            case Mode.BuyMenu:

                break;
            case Mode.ContextMenu:
                break;
            case Mode.UnloadUnit:
                UnloadUnit(_targetTile);
                break;
        }
        BlockInputFor(0.2f);
    }
    #endregion
    #region B-Button Methods
    public void BButton()
    {
        switch (CurrentMode)
        {
            case Mode.Normal:
                if (Core.Model.GetTile(Cursor.Position).UnitHere == null)
                {
                    Core.View.ContextMenu.Show(Core.Model.GetTile(Cursor.Position));
                }
                break;
            case Mode.Fire:
                ResetTilesToCycle();
                Cursor.SetPosition(GetSelectedPosition());
                Cursor.SetCursorGfx(0);
                Cursor.HideEstimatedDamage();
                Core.View.ContextMenu.Show(SelectedUnit);
                break;
            case Mode.Move:
                if (!SelectedUnit.AnimationController.IsMovingToTarget)
                {
                    if (SelectedTile != null)
                    {
                        SelectedUnit.ResetPosition();
                        SelectedUnit.ResetRotation();
                        Cursor.SetPosition(SelectedUnit.Position);
                    }
                    Deselect();
                }
                break;
            case Mode.BuyMenu:
                Core.View.BuyMenu.Hide();
                Core.View.ShowStandardPanels();
                break;
            case Mode.ContextMenu:
                if (SelectedUnit != null)
                {
                    SelectedUnit.ResetPosition();
                    SelectedUnit.ResetRotation();
                    Cursor.SetPosition(SelectedUnit.Position);
                    Deselect();
                }
                else
                {
                    Core.View.ContextMenu.Hide();
                }
                break;
            case Mode.UnloadUnit:
                ResetTilesToCycle();
                Cursor.SetPosition(GetSelectedPosition());
                Core.View.ContextMenu.Show(SelectedUnit);
                break;
            case Mode.ShowTileDetails:
                Core.View.TileDetails.Hide();
                break;
            default:
                break;
        }
    }
    public void BButtonHold()
    {
        Unit unit = Core.Model.GetTile(Cursor.Position).UnitHere;
        if (unit != null)
        {
            SelectedUnit = unit;
            unit.ShowAttackableTiles(unit.Position);
        }
    }
    public void BButtonReleased()
    {
        Deselect();
    }
    #endregion
    #region RB & LB
    public void RButton()
    {
        if (CurrentMode == Mode.Normal && ActiveTeam.HasActiveUnits())
        {
            GoTo(ActiveTeam.GetNextActiveUnit().Position);
        }
        else Core.AudioManager.PlayNopeSound();
    }
    public void LButton()
    {
        if (CurrentMode == Mode.Normal && ActiveTeam.HasActiveUnits())
        {
            GoTo(ActiveTeam.GetPreviousActiveUnit().Position);
        }
        else Core.AudioManager.PlayNopeSound();
    }
    #endregion
    #region Cursor Methods
    public Controller_Cursor CreateCursor(Vector2Int pos)
    {
        Controller_Cursor cursor = Instantiate(Core.Model.Database.cursorPrefab, this.transform).GetComponent<Controller_Cursor>();
        cursor.Init(pos);
        return cursor;
    }
    public void GoTo(Vector2Int pos)
    {
        if (Core.Model.IsOnMap(pos))
        {
            switch (CurrentMode)
            {
                case Mode.Normal:
                    Cursor.SetPosition(pos);
                    break;
                case Mode.Fire:
                    CyclePositions(_tilesToCycle, pos);
                    break;
                case Mode.Move:
                    Tile tile = Core.Model.GetTile(pos);             
                    //TODO: Adapt movement arrow, so you can chose your own path. (avoid interuption)
                    //Draws an Arrow on the tile, if it is reachable.
                    //if (SelectedUnit.CanReachTile(tile) && ArrowBuilder.EnoughMovePointsRemaining(tile, SelectedUnit) && !ArrowBuilder.IsPartOfArrowPath(tile))
                    //{
                    //    ArrowBuilder.Add(tile);
                    //    Cursor.SetPosition(pos);
                    //}
                    //else if(ArrowBuilder.IsPartOfArrowPath(tile))
                    //{
                    //    ArrowBuilder.Remove(tile);
                    //    Cursor.SetPosition(pos);
                    //}
                    //else 
                    if(SelectedUnit.CanReachTile(tile))
                    {
                        Cursor.SetPosition(pos);
                        Tile start = SelectedUnit.CurrentTile;
                        Tile end = tile;
                        //ClearIndicators();
                        //IndicatePath(Core.Model.AStar.finalPath);
                        ArrowBuilder.SetPath(Core.Model.AStar.GetPath(SelectedUnit, start, end, true));                        
                    }                    
                    break;
                case Mode.BuyMenu:

                    break;
                case Mode.ContextMenu:
                    break;
                case Mode.UnloadUnit:
                    CyclePositions(_tilesToCycle, pos);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
    #region Cycle Positions
    void CyclePositions(List<Tile> tiles, Vector2Int newPos)
    {
        Vector2Int currentPos = Cursor.Position;
        if (newPos.x > currentPos.x)
        {
            if (CanCycleRight(currentPos))
            {
                Tile nextTile = GetClosestTileRight(currentPos);
                Cursor.SetPosition(nextTile.Position);
                Cursor.ShowEstimatedDamage(SelectedUnit, nextTile.UnitHere, nextTile);
                _targetTile = nextTile;
            }
        }
        else if (newPos.x < currentPos.x)
        {
            if (CanCycleLeft(currentPos))
            {
                Tile nextTile = GetClosestTileLeft(currentPos);
                Cursor.SetPosition(nextTile.Position);
                Cursor.ShowEstimatedDamage(SelectedUnit, nextTile.UnitHere, nextTile);
                _targetTile = nextTile;
            }
        }
        else if (newPos.y > currentPos.y)
        {
            if (CanCycleUp(currentPos))
            {
                Tile nextTile = GetClosestTileUp(currentPos);
                Cursor.SetPosition(nextTile.Position);
                Cursor.ShowEstimatedDamage(SelectedUnit, nextTile.UnitHere, nextTile);
                _targetTile = nextTile;
            }
        }
        else if (newPos.y < currentPos.y)
        {
            if (CanCycleDown(currentPos))
            {
                Tile nextTile = GetClosestTileDown(currentPos);
                Cursor.SetPosition(nextTile.Position);
                Cursor.ShowEstimatedDamage(SelectedUnit, nextTile.UnitHere, nextTile);
                _targetTile = nextTile;
            }
        }
    }
    bool CanCycleRight(Vector2Int currentPos)
    {
        foreach (Tile tile in _tilesToCycle) if (tile.Position.x > currentPos.x) return true;
        return false;
    }
    bool CanCycleLeft(Vector2Int currentPos)
    {
        foreach (Tile tile in _tilesToCycle) if (tile.Position.x < currentPos.x) return true;
        return false;
    }
    bool CanCycleUp(Vector2Int currentPos)
    {
        foreach (Tile tile in _tilesToCycle) if (tile.Position.y > currentPos.y) return true;
        return false;
    }
    bool CanCycleDown(Vector2Int currentPos)
    {
        foreach (Tile tile in _tilesToCycle) if (tile.Position.y < currentPos.y) return true;
        return false;
    }

    Tile GetClosestTileRight(Vector2Int currentPos)
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile tile in _tilesToCycle) if (tile.Position.x > currentPos.x) tempList.Add(tile);
        return GetClosestTile(tempList, currentPos);
    }
    Tile GetClosestTileLeft(Vector2Int currentPos)
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile tile in _tilesToCycle) if (tile.Position.x < currentPos.x) tempList.Add(tile);
        return GetClosestTile(tempList, currentPos);
    }
    Tile GetClosestTileUp(Vector2Int currentPos)
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile tile in _tilesToCycle) if (tile.Position.y > currentPos.y) tempList.Add(tile);
        return GetClosestTile(tempList, currentPos);
    }
    Tile GetClosestTileDown(Vector2Int currentPos)
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile tile in _tilesToCycle) if (tile.Position.y < currentPos.y) tempList.Add(tile);
        return GetClosestTile(tempList, currentPos);
    }

    Tile GetClosestTile(List<Tile> tiles, Vector2Int currentPos)
    {
        float shortestDistance = 99999;
        Tile closestTile = null;
        foreach (Tile tile in tiles)
        {
            float distance = Vector2Int.Distance(tile.Position, currentPos);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestTile = tile;
            }
        }
        return closestTile;
    }
    #endregion
    #region Selection Methods
    //Select an unit.
    void Select(Unit unit)
    {
        SelectedUnit = unit;
    }
    //Select a tile.
    void Select(Tile tile)
    {
        SelectedTile = tile;
    }
    public Vector2Int GetSelectedPosition()
    {
        if (SelectedTile != null) return SelectedTile.Position;
        else return SelectedUnit.Position;
    }

    //If you click on a new object, drop the old one, return to normal mode, delete the marking cursor and reset all data referring to this object.
    public void Deselect()
    {
        Core.View.ContextMenu.Hide();
        if (SelectedUnit != null) DeselectUnit();
        if (SelectedTile != null) DeselectTile();
    }
    //Deselect an Unit.
    public void DeselectUnit()
    {
        SelectedUnit.ClearAttackableTiles();
        SelectedUnit.ClearReachableTiles();
        SelectedUnit.ClearAttackableTiles();
        ArrowBuilder.ResetAll();
        Cursor.SetCursorGfx(0);
        SelectedUnit = null;
    }
    //Deselect a Tile.
    public void DeselectTile()
    {
        SelectedTile = null;
    }
    #endregion
    #region Buttons    
    public void FireButton()
    {
        CurrentMode = Mode.Fire;
        _tilesToCycle = SelectedUnit.GetAttackableEnemyTiles();
        Cursor.SetCursorGfx(1);
        Cursor.SetPosition(_tilesToCycle[0].Position);
        Cursor.ShowEstimatedDamage(SelectedUnit, _tilesToCycle[0].UnitHere, _tilesToCycle[0]);
        _targetTile = _tilesToCycle[0];
        Core.View.ContextMenu.Hide(Mode.Fire);
    }
    IEnumerator FireButtonDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
    public void WaitButton()
    {
        SelectedUnit.Wait();
        Deselect();
    }    
    public void OccupyButton()
    {
        OccupyAction(SelectedUnit, Core.Model.GetTile(Cursor.Position));        
        Deselect();
    }
    public void RangeButton()
    {

    }
    //Pause Menu
    public void EndTurnButton()
    {
        StartCoroutine(EndTurnButtonDelayed(0.01f));
    }
    IEnumerator EndTurnButtonDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndTurn();
    }
    public void InfoButton()
    {
        Debug.Log("Interesting info, you never knew you would want to know!");
    }
    public void UniteButton()
    {
        Unit clickedUnit = Core.Model.GetTile(Cursor.Position).UnitHere;
        clickedUnit.Unite(SelectedUnit);
        SelectedUnit.ConfirmPosition(Cursor.Position);
        SelectedUnit.Deactivate();
        Deselect();
        Core.View.ContextMenu.Hide();
    }
    #endregion   
    #region Occupy Property
    //Adds a property to a team.
    public void OccupyAction(Unit unit, Tile tile)
    {
        tile.Property.DecreaseTakeOverPoints(unit);
        unit.Wait();
    }
    public void StopOccupation(Tile tile)
    {
        tile.GetComponent<Property>().Reset();
    }
    public void Occupy(Team newOwner, Tile tile)
    {
        //If it was occupied by another team, delete it from their property list.
        if (tile.Property.OwningTeam != null) tile.Property.OwningTeam.OwnedProperties.Remove(tile);
        //Introduce the new owner to the tile.
        tile.Property.OwningTeam = newOwner;
        //Set the color of the property to the occupying team color.
        tile.SetColor(newOwner.Data.color);
        //Add the tile to the new owners properties.
        newOwner.OwnedProperties.Add(tile);
        //If you occupy the enemies HQ, you win the game.
        //TODO: find a better place for this
        if (tile.data.type == TileType.HQ && Core.Controller.RoundCounter > 1)
        {
            //TODO: decide if more than two teams are playing and then only remove the defeated team from the map.
            //TODO: winning animationstuff
            Core.View.VictoryScreen.Show(ActiveTeam);
        }
        //If you reach the necessary amount of properties you also win the game.
        //!WORKING
        if (Core.Model.MapSettings.propertiesToWin > 11 && newOwner.OwnedProperties.Count == Core.Model.MapSettings.propertiesToWin)
        {
            //TODO: decide if more than two teams are playing and then only remove the defeated team from the map.
            //TODO: winning animationstuff
            Core.View.VictoryScreen.Show(ActiveTeam);

        }
    }
    #endregion
    #region Load/Save

    #endregion
    #region Weather
    void ChangeWeather(Weather newWeather)
    {
        if (currentWeather != newWeather)
        {
            currentWeather = newWeather;
            //TODO: Change graphics            
        }
    }

    Weather GetRandomWeather()
    {
        int randomInt = UnityEngine.Random.Range(0, Enum.GetValues(typeof(Weather)).Length);
        return (Weather)randomInt;
    }
    #endregion
    #region Transport Unit
    public void LoadUnit()
    {
        Unit_Transporter transporter = Core.Model.GetTile(Cursor.Position).UnitHere.GetComponent<Unit_Transporter>();
        transporter.LoadUnit(SelectedUnit);
        Core.View.ContextMenu.Hide();
        CurrentMode = Mode.Normal;
    }
    public void UnloadUnit(Tile tile)
    {
        SelectedUnit.GetComponent<Unit_Transporter>().UnloadUnit(tile);
        SelectedUnit.Wait();
        ResetTilesToCycle();
        Deselect();
        CurrentMode = Mode.Normal;
    }
    public void ChoseUnloadPosition()
    {
        SelectedUnit.ClearReachableTiles();
        Core.View.ContextMenu.Hide(Mode.UnloadUnit);
        _tilesToCycle = SelectedUnit.GetComponent<Unit_Transporter>().GetPossibleDropOffPositions(GetSelectedPosition());
        _targetTile = _tilesToCycle[0];
        Cursor.SetPosition(_tilesToCycle[0].Position);

        CurrentMode = Mode.UnloadUnit;
    }
    void ResetTilesToCycle()
    {
        _targetTile = null;
        _tilesToCycle.Clear();
    }
    #endregion
    #region A*    
    public void IndicatePath(List<Tile> path)
    {
        float offset = 1;
        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0)
            {
                Vector3 position = new Vector3(path[i].transform.position.x, offset, path[i].transform.position.z);
                indicators.Add(Instantiate(startIndicator, position, Quaternion.identity));
            }
            indicators.Add(Instantiate(pathIndicator, path[i].transform.position, Quaternion.identity));
            if (i == path.Count - 1)
            {
                Vector3 position = new Vector3(path[i].transform.position.x, offset, path[i].transform.position.z);
                indicators.Add(Instantiate(endIndicator, position, Quaternion.identity));
            }
        }
    }
    
    #endregion
    #region Utility
    public void BlockInputFor(float duration)
    {
        Cursor.BlockInput(duration);
    }
    public void BlockInput(bool value)
    {
        Cursor.BlockInput(value);
    }
    #endregion
    #region Kill Unit
    //Destroys the unit
    public void KillUnit(Unit unit)
    {
        Tile tile = Core.Model.GetTile(unit.Position);
        if (tile.IsProperty()) tile.Property.Reset();
        //Set the unit standing on this tile as null.
        tile.UnitHere = null;
        //Remove unit from team list
        unit.team.Units.Remove(unit);
        //If this was the last unit of the player the game is lost.
        if (unit.team.Units.Count <= 0)
        {
            Core.View.VictoryScreen.Show(unit.team.EnemyTeams[0]);
        }
        //If the unit is AI controlled, clear the AI logic for this unit.
        if (unit.team.IsAI)
        {
            unit.team.AI.RemoveAIUnit(unit);
        }
        //Finally delete the unit.
        Destroy(unit.gameObject);
    }
    public IEnumerator KillUnitDelayed(Unit unit, float delay)
    {
        yield return new WaitForSeconds(delay);
        KillUnit(unit);
    }
    #endregion
    #region Debug
    public void KillAllEnemies()
    {
        List<Team> enemyTeams = ActiveTeam.EnemyTeams;
        for (int i = 0; i < enemyTeams.Count; i++)
        {
            for (int j = 0; j < enemyTeams[i].Units.Count; j++)
            {
                Unit unit = enemyTeams[i].Units[j];
                StartCoroutine(Core.Controller.KillUnitDelayed(unit, UnityEngine.Random.Range(1.1f, 2.5f)));
            }
        }
    }
    public void IndicateTiles(List<Tile> tiles)
    {
        ClearIndicators();
        for (int i = 0; i < tiles.Count; i++)
        {            
            indicators.Add(Instantiate(pathIndicator, tiles[i].transform.position, Quaternion.identity));           
        }
    }
    public void ClearIndicators()
    {
        foreach (GameObject gameObject in indicators)Destroy(gameObject);    
        indicators.Clear();
    }
    #endregion
}
