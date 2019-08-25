using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class AI : MonoBehaviour
{
    #region References
    Team team;
    #endregion
    #region Fields
    public Tile enemyHQ;
    public List<POI> POIs = new List<POI>();
    public List<Squad> Squads = new List<Squad>();
    public List<UnitPreset> UnitPresets;
    public int enemyHQRadius = 4;//In this radius we consider units as near the HQ.
    public enum Strategy { FrontalAttack, HoldPOIs, Siege, Guerilla, AttackFromBehind}
    public Strategy CurrentStrategy;
    bool decisionPhase = true;
    bool squadPhase = true;
    bool buyPhase = true;
    #endregion    

    #region Basic Methods
    public void Init(Team team)
    {
        this.team = team;
        enemyHQ = GetEnemyHQ(team);
        InitUnits(team.Units);
        InitSquads();
    }   
    void InitSquads()
    {       
        //Create squads for the presets given.
        foreach (UnitPreset preset in UnitPresets)
        {
            Squad newSquad = new Squad(this, preset);
            newSquad.Init();
            Squads.Add(newSquad);
        }
    }    
    //If units are already on the map, put them into an empty squad. For now...
    void InitUnits(List<Unit> units)
    {
        if(units.Count > 0)
        {
            Squad emptySquad = new Squad(this, 0, null, Squad.Tactic.HoldPosition);
            Squads.Add(emptySquad);
            foreach (Unit unit in units)emptySquad.Add(unit);          
        }
    }
    void ResetPhases()
    {
        decisionPhase = true;
        squadPhase = true;
        buyPhase = true;
    }
    void ResetSquads()        
    {        
        foreach (Squad item in Squads) item.Reset();        
    }
    #endregion
    #region Main Methods
    public IEnumerator StartTurnDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartTurn();
    }
    public void StartTurn()
    {
        Debug.Log("========================");
        Debug.Log("AI: " + team + " starts turn!");
        //Analyze situation
        //Change strategy
        ContinueTurn();
    }
    public void ContinueTurn()
    {
        //Scout before make decisions?
        //Decide what to do
        if (decisionPhase) Decide();
        //Make moves for units
        else if (squadPhase) ActivateNextSquad();
        //Buy new units
        else if (buyPhase) BuyUnits();
        //Nothing more to do?
        else EndTurn();
    }
    public void EndTurn()
    {
        Debug.Log("AI ends turn!");
        ResetPhases();
        ResetSquads();
        Core.Controller.EndTurnButton();
    }
    #endregion
    #region Decision Phase
    void Decide()
    {
        foreach (Squad squad in Squads)
        {
            //evaluate and adapt tactics
            squad.TargetTile = enemyHQ;
            switch (squad.Preset.Type)
            {
                case UnitPresetType.InfantrySquad:
                    //There are free properties
                    if(GetNeutralProperties().Count > 0) squad.CurrentTactic = Squad.Tactic.OccupyNeutralProperties;                 
                    //No free properties left, start capturing enemy properties
                    else squad.CurrentTactic = Squad.Tactic.OccupyEnemyProperties;                   
                    break;
                case UnitPresetType.InfantryAndMechSquad:
                    break;
                case UnitPresetType.TankSquad:
                    break;
                case UnitPresetType.HeavyTankSquad:
                    break;
                case UnitPresetType.ArtillerySquad:
                    break;
                case UnitPresetType.HeavyArtillerySquad:
                    squad.CurrentTactic = Squad.Tactic.HoldPOI;

                    Debug.Log("Poi center" + Core.Model.POIs[0].Center);
                    squad.POI = Core.Model.POIs[0];
                    break;
                default:
                    break;
            }
        }        
        decisionPhase = false;
        ContinueTurn();
    }
    

    #endregion
    #region Squad Phase
    void ActivateNextSquad()        
    {
        Squad nextSquad = GetNextUnusedSquad(Squads);
        if(nextSquad != null)
        {            
            nextSquad.Start();
        }
        else
        {
            squadPhase = false;
            ContinueTurn();
        }
    }
    Squad GetNextUnusedSquad(List<Squad> squads)
    {
        foreach (Squad squad in squads)if (!squad.Finished) return squad;        
        return null;
    }    
    #endregion   
    #region Buy Phase
    void BuyUnits()
    {
        Debug.Log("----> AI wants to buy units for...");
        //Ground Units
        foreach (Squad squad in Squads)
        {
            if (squad.Preset == null) continue;
            Debug.Log("... squad: " + squad.Preset.Type);
            List<Tile> productionBuildings = GetFreeProductionBuildings();
            foreach (Tile facility in productionBuildings)
            {
                UnitType newType = squad.GetNextAffordableInPreset(team);
                if (newType != UnitType.Null && facility.CanProduce(newType)) 
                {
                    Buy(newType, facility, team, squad);
                }
            }
        }                
        buyPhase = false;
        ContinueTurn();
    }
    void Buy(UnitType unitType, Tile tile, Team team, Squad squad)
    {
        Core.Controller.Cursor.SetPosition(tile.Position);
        Unit newUnit = Core.View.BuyMenu.Buy(unitType, tile.Position, team);
        Debug.Log("AI buys : " + newUnit);
        team.Add(newUnit);
        squad.Add(newUnit);
    }
    
    List<Tile> GetFreeProductionBuildings()
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile tile in team.OwnedProperties)
        {
            if (tile.UnitHere == null && (tile.data.type == TileType.Facility || tile.data.type == TileType.Airport || tile.data.type == TileType.Port)) tempList.Add(tile);
        }
        return tempList;
    }

    #endregion
    #region Unit Methods   
    public void Remove(Unit unit)
    {
        AI_Unit unitToRemove = GetAIUnit(unit);
        Squad squad = GetSquad(unit);
        squad.Remove(unitToRemove);
    }
    #endregion
    #region Property Methods
    List<Tile> GetNeutralProperties()
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile item in Core.Model.GetAllProperties())
        {
            if (item.Property.OwningTeam == null) tempList.Add(item);
        }
        return tempList;
    }


    #endregion
    #region Utility
    public List<Tile> SortTilesByDistance(Tile tile, List<Tile> tiles)
    {
        Dictionary<Tile, float> TileDistance = new Dictionary<Tile, float>();
        foreach (Tile item in tiles) TileDistance.Add(item, Vector3.Distance(tile.transform.position, item.transform.position));
        var tempList = TileDistance.ToList();
        tempList = TileDistance.OrderBy(h => h.Value).ToList();
        List<Tile> tempList2 = new List<Tile>();
        foreach (var item in tempList) tempList2.Add(item.Key);
        return tempList2;
    }
    #endregion
    #region Getter    
    
    public List<AI_Unit> GetAttackingUnits(Unit unit)
    {
        List<AI_Unit> tempList = new List<AI_Unit>();
        foreach (AI_Unit aiUnit in GetAllUnits())
        {
            foreach (Order order in aiUnit.Orders)
            {
                if (order.GetType() == typeof(Attack) && order.TargetUnit == unit) tempList.Add(aiUnit);
            }
        }
        return tempList;
    }
    Squad GetSquad(Unit unit)
    {
        foreach (Squad squad in Squads)
        {
            AI_Unit aiUnit = squad.GetAIUnit(unit);
            if (aiUnit != null) return squad;
        }
        throw new System.Exception("AI unit not found in on of the squads!");
    }
    AI_Unit GetAIUnit(Unit unit)
    {
        foreach (Squad squad in Squads)
        {
            AI_Unit aiUnit = squad.GetAIUnit(unit);
            if (aiUnit != null) return aiUnit;
        }
        throw new System.Exception("AI unit for" + unit + " not found!");
    }
    Tile GetEnemyHQ(Team ownTeam)
    {
        foreach (Tile tile in ownTeam.EnemyTeams[0].OwnedProperties) if (tile.data.type == TileType.HQ) return tile;        
        throw new System.Exception(ownTeam + " :No enemy HQ found!");
    }
    public List<AI_Unit> GetAllUnits()
    {
        List<AI_Unit> tempList = new List<AI_Unit>();
        foreach (Squad squad in Squads)
            foreach (AI_Unit unit in squad.Units)
                if(unit != null) tempList.Add(unit);   
        return tempList;
    }
    #endregion
}
