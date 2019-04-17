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

    public enum Mode { normal, fire, move, menu };
    public Mode CurrentMode { get; private set; }
   
    int _enemyIndex = 0;

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
        CurrentMode = Mode.normal;
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
    #region A-Button
    public void AButton()
    {
        Tile tile = Core.Model.GetTile(Cursor.Position);
        Unit unit = tile.GetUnitHere();
        switch (Core.Controller.CurrentMode)
        {
            case Mode.normal:
                if (unit != null)
                {
                    //Select own unit
                    if (ActiveTeam == unit.team && unit.CanMove)
                    {
                        Select(unit.gameObject);
                    }
                    else
                    {
                        //Play dörp sound, unit has aleady moved or is not in our team
                    }
                }
                break;
            case Mode.fire:
                Unit attacker = SelectedUnit;
                Unit defender = SelectedUnit.attackableUnits[_enemyIndex];
                //Align the units to face each other.
                if(attacker.targetTile != null)
                {
                    attacker.AlignUnit(attacker.targetTile.position, defender.position);
                    defender.AlignUnit(defender.position, attacker.targetTile.position);
                }
                else
                {
                    attacker.AlignUnit(attacker.position, defender.position);
                    defender.AlignUnit(defender.position, attacker.position);
                }
                //Battle
                Core.Model.BattleCalculations.Fight(attacker, defender);
                //Reset Cursor
                Cursor.SetPosition(attacker.position);
                Cursor.SetCursorGfx(0);
                //End turn for attacking unit
                _enemyIndex = 0;
                attacker.Wait();
                DeselectObject();
                break;
            case Mode.move:
                if (unit != null)
                {
                    if (unit.isSelected)//If you click on yourself.
                    {
                        OpenContextMenu();
                    }
                    else if (SelectedUnit.team.units.Contains(unit))//If you click on a friendly unit
                    {
                        //Try to unite units
                    }
                    else//If you click on an enemy unit or an unit that is in an alliance with you.
                    {
                        //Play dörp sound, you cant go there
                    }
                }
                else
                {
                    SelectedUnit.MoveUnitTo(Cursor.Position);
                }
                break;
            case Mode.menu:
                //apply choice
                break;
        }
    }
    #endregion
    #region B-Button
    public void BButton()
    {
        //displayCursorGfx(true); reactivate later
        if (SelectedUnit != null && !SelectedUnit.AnimationController.IsMovingToTarget)
        {
            SelectedUnit.ResetPosition();
            Cursor.SetPosition(SelectedUnit.position);
            DeselectObject();
        }
        else if(SelectedUnit == null)
        {
            if(CurrentMode == Mode.menu)
            {
                Core.View.DisplayContextMenu(false);
                CurrentMode = Mode.normal;
            }
            else
            {
                Core.View.DisplayContextMenu(true);
                Core.View.contextMenu.SetButtons(0);
                CurrentMode = Mode.menu;

            }
        }
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
                case Mode.normal:
                    Cursor.SetPosition(pos);
                    break;
                case Mode.fire:
                    //TODO: Change cursor
                    //cycle through the attackable enemies
                    CycleAttackableEnemies(SelectedUnit);
                    break;
                case Mode.move:
                    Tile tile = Core.Model.GetTile(pos);

                    //If you go back, make the arrow smaller.
                    if (tile.isPartOfArrowPath)
                    {
                        //We wont move outside the bounds of the reachable area.
                        if (ArrowBuilder.CanGoBack(tile)) Cursor.SetPosition(pos);
                        ArrowBuilder.TryToGoBack(tile);
                    }
                    //Draws an Arrow on the tile, if it is reachable
                    else if (!tile.isPartOfArrowPath && SelectedUnit.reachableTiles.Contains(tile) && ArrowBuilder.EnoughMovePointsRemaining(tile, SelectedUnit))
                    {
                        //We wont move outside the bounds of the reachable area.
                        ArrowBuilder.CreateNextPart(tile);
                        Cursor.SetPosition(pos);
                    }
                    break;
                case Mode.menu:

                    //navigate menu
                    break;
                default:
                    Debug.Log("Controller_MarkingCursor: goTo: mode not implemented yet!");
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
                Vector2Int enemyPos = unit.attackableUnits[_enemyIndex].position;
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
                Vector2Int enemyPos = unit.attackableUnits[_enemyIndex].position;
                Cursor.SetPosition(enemyPos);
            }
        }
    }

    #endregion
    #region Selection
    public void Select(GameObject gameObject)
    {
        DeselectObject(); //Previous selected object out!
        if (gameObject.GetComponent<Unit>()) SelectUnit(gameObject.GetComponent<Unit>());
        if (gameObject.GetComponent<Tile>()) SelectTile(gameObject.GetComponent<Tile>());
    }
    //Select an unit.
    void SelectUnit(Unit unitToSelect)
    {
        SelectedUnit = unitToSelect;//Handover the object.
        SelectedUnit.isSelected = true;
        Core.View.statusPanel.UpdateDisplay(unitToSelect);
        DisplayReachableArea(SelectedUnit);
        CurrentMode = Mode.move;
        //Init the logic that draws an arrow, that shows where the unit can go.
        Tile tileTheUnitStandsOn = Core.Model.GetTile(SelectedUnit.position);
        ArrowBuilder.StartArrowPath(tileTheUnitStandsOn, SelectedUnit.data.moveDist);             
    }   
    //Select a tile.
    void SelectTile(Tile tile)
    {
        //Decide wich menu to open.        
        if(tile.data.isProperty && tile.owningTeam == ActiveTeam)
        {
            if (tile.data.type == TileType.Facility || tile.data.type == TileType.Airport || tile.data.type == TileType.Port)
            {
                OpenBuyMenu(tile);              
            }
        }
    }
    //If you click on a new object, drop the old one, return to normal mode, delete the marking cursor and reset all data referring to this object.
    public void DeselectObject()
    {
        Core.View.DisplayContextMenu(false);
        if (SelectedUnit != null) DeselectUnit();
        if (SelectedTile != null) DeselectTile();
        CurrentMode = Mode.normal;
    }
    //Deselect a Unit.
    public void DeselectUnit()
    {
        SelectedUnit.isSelected = false;//Deselect Unit
        ClearAttackableArea(SelectedUnit);
        ClearReachableArea(SelectedUnit);
        ClearAttackableUnits(SelectedUnit);
        ArrowBuilder.ResetAll();//Resets the movement arrow.    
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
        unit.CalcReachableArea(unit.position, unit.data.moveDist, unit.data.moveType, null);
        Core.View.CreateReachableTilesGfx(unit);
    }
    public void ClearReachableArea(Unit unit)
    {
        unit.reachableTiles.Clear();
        Core.View.ResetReachableTiles(unit);
    }
    public void DisplayAttackableArea(Unit unit)
    {
        unit.CalcAttackableArea(SelectedUnit.position);
        Core.View.CreateAttackableTilesGfx(unit);
    }
    public void ClearAttackableArea(Unit unit)
    {
        unit.attackableTiles.Clear();
        Core.View.ResetAttackableTiles(unit);
    }
    public void DisplayAttackableUnits(Unit unit)
    {
        //View: indicate attackable units
        //view: create cursor for selected attackable unit
        Cursor.SetCursorGfx(1);
        Cursor.SetPosition(unit.attackableUnits[0].position);
    }
    public void ClearAttackableUnits(Unit unit)
    {
        unit.attackableUnits.Clear();
        //view: delete graphics for indication
    }
    #endregion
    #region Context Menu
    //Context Menu
    public void OpenContextMenu()
    {
        Core.View.DisplayContextMenu(true);
        if(SelectedUnit.targetTile != null) SelectedUnit.CalcAttackableArea(SelectedUnit.targetTile.position);
        if(SelectedUnit.targetTile == null) SelectedUnit.CalcAttackableArea(SelectedUnit.position);
        SelectedUnit.FindAttackableEnemies();
        Core.View.contextMenu.SetButtons(SelectedUnit);
        CurrentMode = Mode.menu;
        //eventSystem.SetSelectedGameObject(null);
        //Invoke("highlightFirstMenuButton", 0.01f);
    }
    public void FireButtonDelayed()
    {
        StartCoroutine(FireButton(0.01f));
    }
    IEnumerator FireButton(float delay)
    {
        yield return new WaitForSeconds(delay);
        CurrentMode = Mode.fire;       
        DisplayAttackableUnits(SelectedUnit);
        //Show indicator over the first enemy     
        ////Set cursor gfx to attack 
        //_manager.getCursor().setCursorGfx(1);
        ////Set cursor position to the first attackable unit
        //int x = _manager.getGameFunctions().getSelectedUnit().attackableUnits[0].position.x;
        //int y = _manager.getGameFunctions().getSelectedUnit().attackableUnits[0].position.y;
        //_manager.getCursor().setCursorPosition(x, y);
        Core.View.DisplayContextMenu(false);
    }

    public void WaitButton()
    {
        SelectedUnit.Wait();
        DeselectObject();
    }
    public void OccupyButton()
    {
        OccupyAction(SelectedUnit, Core.Model.GetTile(SelectedUnit.position));
        SelectedUnit.Wait();
        DeselectObject();
    }
    public void RangeButton()
    {
        Core.View.ToggleAttackableTilesGfx();
    }
    //Pause Menu
    public void EndTurnButtonDelayed()
    {
       StartCoroutine(EndTurnButton(0.01f));
    }
    IEnumerator EndTurnButton(float delay)
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
        Core.View.DisplayBuyMenu(true);
        Core.View.buyMenu.DisplayMenu(tile);
    }
    public void BuyUnit()
    {
        Unit unit = Core.View.buyMenu.SelectedUnit;
        Vector2Int position = Core.View.buyMenu.ProductionPosition;
        //TODO: store the direction of the enemy hq somewhere.
        Direction direction = Direction.East;
        Core.Model.CreateUnit(unit.data.type, ActiveTeam, position, direction);
        Core.View.DisplayBuyMenu(false);
    }
    #endregion
    #region Turn
    //Start turn
    public void StartTurn()
    {
        CurrentMode = Mode.normal;
        ActivateUnits(ActiveTeam);
        Core.View.UpdateFogOfWar(ActiveTeam);//Set fog of war for this team.

        //Subtract fuel.

        //Give rations from properties and APCs.

        GiveMoneyForProperties(ActiveTeam);//Give money for properties.

        //Repair units.

        //Subtract money for repairing units.
        
        Core.View.commanderPanel.UpdateDisplay();//Update the GUI for the active team.
        Cursor.SetPosition(ActiveTeam.units[0].position);//Set cursors position to first unit
    }
    IEnumerator StartTurnDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartTurn();
    }
    //End turn
    public void EndTurn()
    {
        DeselectObject();
        DeactivateUnits(ActiveTeam);
        if (Core.Model.IsLastInSuccession(ActiveTeam)) EndRound(); //Increase Round nr., change weather
        ActiveTeam = Core.Model.GetNextTeamInSuccession();
        StartTurn();//For some reason the first unit is instantly selected!?!? So a delay is necessary -.-
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
    void Occupy(Team newOwner, Tile tile)
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
            Debug.Log(ActiveTeam + " wins the game by getting properties! Wuhuuu!!");

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
