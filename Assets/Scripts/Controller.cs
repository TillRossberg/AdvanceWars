using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Controller : MonoBehaviour
{
    #region References
    public ArrowBuilder ArrowBuilder { get; private set; }
    public Controller_Cursor Cursor { get; private set; }
    #endregion
    public Team ActiveTeam { get; private set; }
    public Tile SelectedTile { get; private set; }
    public Unit SelectedUnit { get; private set; }
    public int RoundCounter { get; private set; }
    public Weather currentWeather;

    public enum Mode { Normal, Fire, Move, BuyMenu, ContextMenu, UnloadUnit, ShowTileDetails };
    public Mode CurrentMode;
   
    int _enemyIndex = 0;
    int _dropOffIndex = 0;

    #region Base Methods
    public void StartGame()
    {
        //Core.Model.InitMap();
        Core.Model.CreateTeam(Core.Model.Database.premadeTeams[0]);
        Core.Model.CreateTeam(Core.Model.Database.premadeTeams[1]);
        Core.Model.teams[0].AddEnemyTeam(Core.Model.teams[1]);
        Core.Model.teams[1].AddEnemyTeam(Core.Model.teams[0]);
        Core.Model.InitTeams();
        Core.Model.LoadLevel01(15, 15);
        Cursor = CreateCursor(new Vector2Int(5, 5));
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
    #region A-Button Methods
    public void AButton()
    {
        Tile currentTile = Core.Model.GetTile(Cursor.Position);
        Unit unitHere = currentTile.GetUnitHere();
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
                        DisplayReachableArea(unitHere);
                        ArrowBuilder.StartArrowPath(unitHere);
                    }
                    //...or select already used unit or unit from enemy team.
                    else Core.AudioManager.PlaySFX(Core.Model.Database.Sounds.NopeSound);                    
                }
                //Select tile, that can produce units...
                else if (currentTile.CanProduceUnits() && currentTile.owningTeam == ActiveTeam) OpenBuyMenu(currentTile);  
                //...or select empty tile.
                else Core.View.TileDetails.Show(currentTile);               
                break;
            case Mode.Fire:
                Unit attacker = SelectedUnit;
                Unit defender = SelectedUnit.attackableUnits[_enemyIndex];
                //Align the units to face each other.
               
                //Battle
                Core.Model.BattleCalculations.Fight(attacker, defender);
                //Reset Cursor
                Cursor.SetPosition(attacker.Position);
                Cursor.SetCursorGfx(0);
                //End turn for attacking unit
                _enemyIndex = 0;
                attacker.Wait();
                Deselect();
                break;
            case Mode.Move:
                //Select another unit you want the to interact with...
                if (unitHere != null)
                {
                    if (unitHere == SelectedUnit)
                    {
                        Core.View.ContextMenu.Show(SelectedUnit);
                    }
                    else if (SelectedUnit.team.units.Contains(unitHere))//If you click on a friendly unit
                    {
                        //Try to unite units
                        //If the unit on this tile can load other units, check if it can load the selected unit.
                        if (unitHere.GetComponent<Unit_Transporter>() != null)
                        {
                            if ((unitHere.data.type == UnitType.APC || unitHere.data.type == UnitType.TCopter) && SelectedUnit.IsInfantryUnit()) SelectedUnit.MoveUnitToLoad(Cursor.Position);
                            if (unitHere.data.type == UnitType.Lander && SelectedUnit.IsGroundUnit()) SelectedUnit.MoveUnitToLoad(Cursor.Position);
                            if (unitHere.data.type == UnitType.Cruiser && SelectedUnit.IsCopterUnit()) SelectedUnit.MoveUnitToLoad(Cursor.Position);
                        }
                    }
                    else//If you click on an enemy unit or an unit that is in an alliance with you.
                    {
                        //Play dörp sound, you cant go there
                    }
                }
                //...or select an empty tile you want to go to.
                else
                {
                    Select(currentTile);
                    SelectedUnit.SetPosition(SelectedTile.Position);
                    SelectedUnit.FindAttackableEnemies();
                    ClearReachableArea(SelectedUnit);
                    ArrowBuilder.ResetAll();
                    Core.View.ContextMenu.Show(SelectedUnit);
                }
               
                break;
            case Mode.BuyMenu:

                break;
            case Mode.ContextMenu:
                break;
            case Mode.UnloadUnit:
                UnloadUnit(SelectedUnit.GetComponent<Unit_Transporter>().dropOffPositions[_dropOffIndex]);
                break;
        }
        Cursor.BlockInput(0.2f);
    }
    #endregion
    #region B-Button Methods
    public void BButton()
    {
        switch (CurrentMode)
        {
            case Mode.Normal:
                Core.View.ContextMenu.Show(Core.Model.GetTile(Cursor.Position));
                break;
            case Mode.Fire:
                break;
            case Mode.Move:
                if (SelectedUnit.PreviousTile != null)
                {
                    SelectedUnit.ResetPosition();
                    Cursor.SetPosition(SelectedUnit.Position);
                    Deselect();
                }
                else Deselect();              
                break;
            case Mode.BuyMenu:
                Core.View.DisplayBuyMenu(false);
                CurrentMode = Mode.Normal;
                break;
            case Mode.ContextMenu:
                if (SelectedUnit != null)
                {
                    SelectedUnit.ResetPosition();
                    Cursor.SetPosition(SelectedUnit.Position);
                    Deselect();
                }
                else
                {
                    Core.View.ContextMenu.Hide();
                }
                break;
            case Mode.UnloadUnit:
                ResetUnloadStuff();
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
        Unit unit = Core.Model.GetTile(Cursor.Position).GetUnitHere();
        if (unit != null)
        {            
            unit.CalcAttackableArea(unit.Position);
            Core.View.CreateAttackableTilesGfx(unit);
        }
    }
    public void BButtonReleased()
    {
        Core.View.ResetAttackableTiles();                    
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
                    //TODO: Change cursor
                    //Cycle through the attackable enemies.
                    CycleAttackableEnemies(SelectedUnit);
                    break;
                case Mode.Move:
                    Tile tile = Core.Model.GetTile(pos);

                    //If you go back, make the arrow smaller.
                    if (tile.isPartOfArrowPath)
                    {
                        //We wont move outside the bounds of the reachable area.
                        if (ArrowBuilder.CanGoBack(tile))
                        {
                            Cursor.SetPosition(pos);
                            ArrowBuilder.TryToGoBack(tile);
                        }
                    }
                    //Draws an Arrow on the tile, if it is reachable
                    else if (!tile.isPartOfArrowPath && SelectedUnit.reachableTiles.Contains(tile) && ArrowBuilder.EnoughMovePointsRemaining(tile, SelectedUnit))
                    {
                        //We wont move outside the bounds of the reachable area.
                        ArrowBuilder.CreateNextPart(tile);
                        Cursor.SetPosition(pos);
                    }
                    break;
                case Mode.BuyMenu:

                    break;
                case Mode.ContextMenu:
                    break;
                case Mode.UnloadUnit:                    
                    CycleDropOffPositions(SelectedUnit.GetComponent<Unit_Transporter>(), pos);
                    break;
                default:
                    break;
            }
        }
    }
    //Goes through the list of enemies and positions the attack cursor over them.
    //TODO: find a way to move through the enemies depending on their position (above, below and so on)
    public void CycleAttackableEnemies(Unit unit)
    {
        if(unit.attackableUnits.Count > 1)
        {
            if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Vertical") > 0)
            {
                _enemyIndex--;
                if (_enemyIndex < 0)
                {
                    _enemyIndex = unit.attackableUnits.Count - 1;
                }
                Vector2Int enemyPos = unit.attackableUnits[_enemyIndex].Position;
                Cursor.SetPosition(enemyPos);
            }
            else
            if (Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("Vertical") < 0)
            {
                _enemyIndex++;
                if (_enemyIndex > unit.attackableUnits.Count - 1)
                {
                    _enemyIndex = 0;
                }
                Vector2Int enemyPos = unit.attackableUnits[_enemyIndex].Position;
                Cursor.SetPosition(enemyPos);
            }
        }
    }
    void CycleDropOffPositions(Unit_Transporter transporter, Vector2Int newPos)
    {
        Vector2Int currentPos = Cursor.Position;
        if (transporter.dropOffPositions.Count > 1)
        {            
            if(newPos.x > currentPos.x)
            {
                Debug.Log("right");
                if (transporter.GetTile(newPos) != null) Cursor.SetPosition(newPos);
            }
            if(newPos.x < currentPos.x)
            {
                if (transporter.GetTile(newPos) != null) Cursor.SetPosition(newPos);
                Debug.Log("left");
            }
            if(newPos.y > currentPos.y)
            {
                if (transporter.GetTile(newPos) != null) Cursor.SetPosition(newPos);
                Debug.Log("up");
            }
            if(newPos.y < currentPos.y)
            {
                if (transporter.GetTile(newPos) != null) Cursor.SetPosition(newPos);
                Debug.Log("down");

            }
            //if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Vertical") > 0)
            //{
            //    _dropOffIndex--;
            //    if (_dropOffIndex < 0)
            //    {
            //        _dropOffIndex = transportert.dropOffPositions.Count - 1;
            //    }
            //    Vector2Int dropOffPosition = transportert.dropOffPositions[_dropOffIndex].position;
            //    Cursor.SetPosition(dropOffPosition);
            //}
            //else
            //if (Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("Vertical") < 0)
            //{
            //    _dropOffIndex++;
            //    if (_dropOffIndex > transportert.dropOffPositions.Count - 1)
            //    {
            //        _dropOffIndex = 0;
            //    }
            //    Vector2Int dropOffPosition = transportert.dropOffPositions[_dropOffIndex].position;
            //    Cursor.SetPosition(dropOffPosition);
            //}
        }
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
        ClearAttackableArea(SelectedUnit);
        ClearReachableArea(SelectedUnit);
        ClearAttackableUnits(SelectedUnit);
        ArrowBuilder.ResetAll();    
        Cursor.SetCursorGfx(0);
        SelectedUnit = null;
    }
    //Deselect a Tile.
    public void DeselectTile()
    {
        SelectedTile = null;
    }

    public void DisplayReachableArea(Unit unit)
    {
        unit.CalcReachableArea(unit.Position, unit.data.moveDist, unit.data.moveType, null);
        Core.View.CreateReachableTilesGfx(unit);
    }
    public void ClearReachableArea(Unit unit)
    {
        unit.reachableTiles.Clear();
        Core.View.ResetReachableTiles(unit);
    }
    public void DisplayAttackableArea(Unit unit)
    {
        unit.CalcAttackableArea(SelectedUnit.Position);
        Core.View.CreateAttackableTilesGfx(unit);
    }
    public void ClearAttackableArea(Unit unit)
    {
        unit.attackableTiles.Clear();
        Core.View.ResetAttackableTiles();
    }
    public void DisplayAttackableUnits(Unit unit)
    {
        //View: indicate attackable units
        //view: create cursor for selected attackable unit
        Cursor.SetCursorGfx(1);
        Cursor.SetPosition(unit.attackableUnits[0].Position);
    }
    public void ClearAttackableUnits(Unit unit)
    {
        unit.attackableUnits.Clear();
        //view: delete graphics for indication
    }
    #endregion
    #region Context Menu    
    public void FireButton()
    {
        StartCoroutine(FireButtonDelayed(0.01f));
    }
    IEnumerator FireButtonDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        CurrentMode = Mode.Fire;       
        DisplayAttackableUnits(SelectedUnit);
        //Show indicator over the first enemy     
        ////Set cursor gfx to attack 
        //_manager.getCursor().setCursorGfx(1);
        ////Set cursor position to the first attackable unit
        //int x = _manager.getGameFunctions().getSelectedUnit().attackableUnits[0].position.x;
        //int y = _manager.getGameFunctions().getSelectedUnit().attackableUnits[0].position.y;
        //_manager.getCursor().setCursorPosition(x, y);
        Core.View.ContextMenu.Hide();
    }

    public void WaitButton()
    {
        SelectedUnit.Wait();
        Deselect();
    }
    public void ChoseUnloadPosition()
    {
        ClearReachableArea(SelectedUnit);
        Core.View.ContextMenu.Hide(Mode.UnloadUnit);
        SelectedUnit.GetComponent<Unit_Transporter>().SetPossibleDropPositions(SelectedUnit.CurrentTile);
        Cursor.SetPosition(SelectedUnit.GetComponent<Unit_Transporter>().dropOffPositions[0].Position);
        CurrentMode = Mode.UnloadUnit;
        Cursor.BlockInput(0.5f);
    }
   
    public void OccupyButton()
    {
        OccupyAction(SelectedUnit, Core.Model.GetTile(SelectedUnit.Position));
        SelectedUnit.Wait();
        Deselect();
    }
    public void RangeButton()
    {
        Core.View.ToggleAttackableTilesGfx();
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
    #endregion
    #region Buy Menu
    public void OpenBuyMenu(Tile tile)
    {
        CurrentMode = Mode.BuyMenu;
        Core.View.DisplayBuyMenu(true);
        Core.View.buyMenu.DisplayMenu(tile);
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
        
        Core.View.commanderPanel.UpdateDisplay();//Update the GUI for the active team.
        Cursor.SetPosition(ActiveTeam.units[0].Position);//Set cursors position to first unit
    }
    IEnumerator StartTurnDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartTurn();
    }
    //End turn
    public void EndTurn()
    {
        Cursor.BlockInput(.5f);
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
        team.AddMoney(team.ownedProperties.Count * Core.Model.MapSettings.moneyIncrement);
    }

    //Sets all the units of a team so they have a turn, can move and fire.
    void ActivateUnits(Team team)
    {
        foreach (Unit unit in team.units)
        {
            if(unit != null) unit.Activate();
        }        
    }

    //Set the properties of all units of a team so they don't have a turn.
    void DeactivateUnits(Team team)
    {
        foreach (Unit unit in team.units)
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
            int propertyCount = team.ownedProperties.Count;
            if (propertyCount > highestPropertyCount)
            {
                highestPropertyCount = propertyCount;
                winner = team;
            }
        }        
        return winner;
    }
    #endregion   
    #region Occupy Property
    //Adds a property to a team.
    public void OccupyAction(Unit unit, Tile tile)
    {
        tile.takeOverCounter -= unit.GetCorrectedHealth();
        if(tile.takeOverCounter <= 0)
        {
            tile.takeOverCounter = 0;
            Occupy(unit.team, tile);
        }
    }
    public void StopOccupation(Tile tile)
    {
        tile.ResetTakeOverCounter();
    }
    public void Occupy(Team newOwner, Tile tile)
    {       
        //If it was occupied by another team, delete it from their property list.
        if (tile.owningTeam != null)
        {
            tile.owningTeam.ownedProperties.Remove(tile);//This maybe confusing: we delete the property from the list of the team that owned it.
            tile.owningTeam = null;
        }
        //Introduce the new owner to the tile.
        tile.owningTeam = newOwner;
        //Set the color of the property to the occupying team color.
        tile.SetColor(newOwner.data.teamColor);
        //Add the tile to the new owners properties.
        newOwner.ownedProperties.Add(tile);
        //If you occupy the enemies HQ, you win the game.
        //TODO: find a better place for this
        if (tile.data.type == TileType.HQ && Core.Controller.RoundCounter > 1)
        {
            //TODO: decide if more than two teams are playing and then only remove the defeated team from the map.
            //TODO: winning animationstuff
            Debug.Log(ActiveTeam + " wins the game by capturing the enmy HQ! Wuhuuu!!"); 
        }
        //If you reach the necessary amount of properties you also win the game.
        //!WORKING
        if (Core.Model.MapSettings.propertiesToWin > 11 && newOwner.ownedProperties.Count == Core.Model.MapSettings.propertiesToWin)
        {
            //TODO: decide if more than two teams are playing and then only remove the defeated team from the map.
            //TODO: winning animationstuff
            Debug.Log(ActiveTeam + " wins the game by getting " + Core.Model.MapSettings.propertiesToWin +" properties! Wuhuuu!!");

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
        Unit_Transporter transporter = Core.Model.GetTile(Cursor.Position).GetUnitHere().GetComponent<Unit_Transporter>();
        transporter.LoadUnit(SelectedUnit);
        Core.View.ContextMenu.Hide();
        CurrentMode = Mode.Normal;
        Cursor.BlockInput(0.5f);
    }
    public void UnloadUnit(Tile tile)
    {
        SelectedUnit.GetComponent<Unit_Transporter>().UnloadUnit(tile);
        SelectedUnit.Wait();
        ResetUnloadStuff();
        Deselect();
        CurrentMode = Mode.Normal;        
    }
    void ResetUnloadStuff()
    {
        _dropOffIndex = 0;
        SelectedUnit.GetComponent<Unit_Transporter>().dropOffPositions.Clear();
    }
    #endregion
    #region Debug
    public void killAllEnemies()
    {
        List<Team> enemyTeams = ActiveTeam.enemyTeams;
        for (int i = 0; i < enemyTeams.Count; i++)
        {
            for (int j = 0; j < enemyTeams[i].units.Count; j++)
            {
                Unit unit = enemyTeams[i].units[j];
                unit.KillUnitDelayed(UnityEngine.Random.Range(1.1f, 2.5f));
            }
        }
    }
    #endregion
}
