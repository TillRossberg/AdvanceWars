using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Team: MonoBehaviour
{
    #region Basic Fields
    public Data_Team data;
    public List<Team> enemyTeams = new List<Team>();
    public List<Team> alliedTeams = new List<Team>();

    public int Money { get; private set; }
    public List<Unit> units = new List<Unit>();
    public List<Tile> ownedProperties = new List<Tile>();    

    #endregion
    #region Basic
    public void Init()
    {
        Money = Core.Model.MapSettings.startMoney;
        if(units.Count > 0)
        {
            AssignTeamColor(units);
        }
    }  
   
    public void ActivateUnits()
    {
        foreach (Unit unit in units)
        {
            if(unit != null) unit.Activate();
        }
    }
    
    #endregion   
      
    #region Units
    //Add an unit to the team, set its color to the teamcolor and pass information about the own team and the enemy team to the unit.
    public void AddUnit(Unit unitToAdd)
    {
        units.Add(unitToAdd);
        unitToAdd.team = this;
        unitToAdd.enemyTeams = enemyTeams;
        unitToAdd.SetTeamColor(data.teamColor);
        data.IncUnitsBuilt(unitToAdd.data.type);
    }     
    //Deletes a unit completely with all references. (Sure?)
    public void DestroyUnit(Unit unit)
    {
        GameObject.Destroy(unit.gameObject);        
    }
    public bool IsInMyTeam(Unit unitToTest)
    {
        if(units.Contains(unitToTest)) return true;      
        else return false;        
    }
    void AssignTeamColor(List<Unit> units)
    {
        foreach (Unit unit in units)
        {
            if(unit != null) unit.SetTeamColor(data.teamColor);
        }
    }
    public List<Unit> GetAllUnitsOfType(UnitType type)
    {
        List<Unit> tempList = new List<Unit>();
        foreach (Unit item in units)if (item.data.type == type) tempList.Add(item);        
        return tempList;
    }
    #endregion
    #region Money
    public void SetStartMoney(int amount) { Money = amount; }

    //Checks if enough money is on the account to do the deposit.
    public bool EnoughMoney(int amount){return Money - amount >= 0? true : false;}
    
    //Adds funds to the team account.
    public void AddMoney(int amount)
    {
        Money += amount;
        data.IncTotalMoney(amount);
    }
    

    //Subtracts funds from the teams account.
    public void SubtractMoney(int amount)
    {
        if(EnoughMoney(amount))
        {
            Money -= amount;
        }
        else
        {
            //TODO: if the funds are insufficient, grey out the button. This else case should never be reached.
            Debug.Log("Team: You have insufficient funds!");
        }        
    }

    #endregion
    
    #region Enemy Teams
    //Add enemy team, but only if it is not already in the list and it is not THIS team.
    public void AddEnemyTeam(Team possibleEnemy)
    {
        if (possibleEnemy != this && !enemyTeams.Contains(possibleEnemy) && !alliedTeams.Contains(possibleEnemy))enemyTeams.Add(possibleEnemy);     
        else throw new System.Exception("Team: You are either trying to add your own team to the enemy teams list, this enemy team is already in the list or you try to add one of your allies to the enemy list!");
    }

    #endregion
    #region Allied Teams
    //Get/Set allied Teams
    public void AddAlliedTeam(Team possibleAlly)
    {
        if (possibleAlly != this && !alliedTeams.Contains(possibleAlly) && !enemyTeams.Contains(possibleAlly) ) alliedTeams.Add(possibleAlly);        
        else throw new System.Exception("Team: You are either trying to add your own team to the allied teams list, this possible allied team is already in the list or you try to add one of your enemies to the allied list!");         
    }   

    #endregion    
    
}
