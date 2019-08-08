using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Team: MonoBehaviour
{
    #region Basic Fields
    public Data_Team Data;
    public List<Team> EnemyTeams = new List<Team>();
    public List<Team> AlliedTeams = new List<Team>();

    public int Money { get; private set; }
    public List<Unit> Units = new List<Unit>();
    int _unitIndex = 0;
    public List<Tile> OwnedProperties = new List<Tile>();
    #endregion
    #region AI
    public bool IsAI;
    public AI AI { get; private set; }
    #endregion
    #region Basic
    public void Init()
    {
        Money = Core.Model.MapSettings.startMoney;
        if(Units.Count > 0)AssignTeamColor(Units);
        if (IsAI)
        {
            AI = GetComponent<AI>();
            AI.Init(this);
        }
    }  
   
    public void ActivateUnits()
    {
        foreach (Unit unit in Units)
        {
            if(unit != null) unit.Activate();
        }
    }
    
    #endregion   
      
    #region Units
    //Add an unit to the team, set its color to the teamcolor and pass information about the own team and the enemy team to the unit.
    public void AddUnit(Unit unitToAdd)
    {
        unitToAdd.transform.parent = this.transform;
        Units.Add(unitToAdd);
        unitToAdd.team = this;
        unitToAdd.enemyTeams = EnemyTeams;
        unitToAdd.SetTeamColor(Data.color);
        Data.IncUnitsBuilt(unitToAdd.data.type);
        if(IsAI) AI.AddAIUnit(unitToAdd);          
    }     
    //Deletes a unit completely with all references. (Sure?)
    public void DestroyUnit(Unit unit)
    {
        GameObject.Destroy(unit.gameObject);        
    }
    public bool IsInMyTeam(Unit unitToTest)
    {
        if(Units.Contains(unitToTest)) return true;      
        else return false;        
    }
    void AssignTeamColor(List<Unit> units)
    {
        foreach (Unit unit in units)
        {
            if(unit != null) unit.SetTeamColor(Data.color);
        }
    }
    public List<Unit> GetAllUnitsOfType(UnitType type)
    {
        List<Unit> tempList = new List<Unit>();
        foreach (Unit item in Units)if (item.data.type == type) tempList.Add(item);        
        return tempList;
    }
    public bool HasActiveUnits()
    {
        foreach (Unit item in Units) if (item.HasTurn) return true;       
        return false;
    }
    public Unit GetNextActiveUnit()
    {
        Unit unit = null;
        _unitIndex++;
        if (_unitIndex >= Units.Count) _unitIndex = 0;
        for (int i = _unitIndex; i < Units.Count; i++)
        {
            if(Units[i].HasTurn)
            {
                unit = Units[i];
                _unitIndex = i;
                break;
            }
        }        
        return unit;
    }
    public Unit GetPreviousActiveUnit()
    {
        Unit unit = null;
        _unitIndex--;
        if (_unitIndex < 0) _unitIndex = Units.Count - 1;
        for (int i = _unitIndex; i >= 0; i--)
        {
            if (Units[i].HasTurn)
            {
                unit = Units[i];
                _unitIndex = i;
                break;
            }
        }
        return unit;
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
        Data.IncTotalMoney(amount);
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
        if (possibleEnemy != this && !EnemyTeams.Contains(possibleEnemy) && !AlliedTeams.Contains(possibleEnemy))EnemyTeams.Add(possibleEnemy);     
        else throw new System.Exception("Team: You are either trying to add your own team to the enemy teams list, this enemy team is already in the list or you try to add one of your allies to the enemy list!");
    }

    #endregion
    #region Allied Teams
    //Get/Set allied Teams
    public void AddAlliedTeam(Team possibleAlly)
    {
        if (possibleAlly != this && !AlliedTeams.Contains(possibleAlly) && !EnemyTeams.Contains(possibleAlly) ) AlliedTeams.Add(possibleAlly);        
        else throw new System.Exception("Team: You are either trying to add your own team to the allied teams list, this possible allied team is already in the list or you try to add one of your enemies to the allied list!");         
    }   

    #endregion    
    
}
