using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : ScriptableObject
{
    public Database.commander teamCommander;
    public List<Transform> myUnits = new List<Transform>();
    public List<Tile> ownedProperties = new List<Tile>();
    public List<Unit.type> availableUnits = new List<Unit.type>();//Units this team can build.
    public Material teamMaterial;
    public Color teamColor;
    public int unitCounter = 0;//Counts the overall created units.
    public int money = 5000;

    public List<Team> enemyTeams = new List<Team>();//Holds all the enemy teams.
    public List<Team> alliedTeams = new List<Team>();//Holds all friendos.
    //Add an unit to the team, set its color to the teamcolor and pass information about the own team and the enemy team to the unit.
    public void addUnit(Transform unitToAdd)
    {
        myUnits.Add(unitToAdd);
        unitToAdd.GetComponent<Unit>().myTeam = this;
        unitToAdd.GetComponent<Unit>().enemyTeams = enemyTeams;
        unitToAdd.GetComponentInChildren<MeshRenderer>().material = teamMaterial;
        unitCounter++;
    }

    //Deletes a unit completely with all references. (Sure?)
    public void deleteUnit(Transform unit)
    {
        Destroy(unit);        
    }

   

    //Checks if enough money is on the account to do the deposit.
    public bool enoughMoney(int amount)
    {
        return money - amount >= 0? true : false;        
    }
    
    //Adds funds to the team account.
    public void addMoney(int amount)
    {
        money += amount;
    }

    //Subtracts funds from the teams account.
    public void subtractMoney(int amount)
    {
        if(enoughMoney(amount))
        {
            money -= amount;
        }
        else
        {
            Debug.Log("Team: You have insufficient funds!");
        }
    }

    //Add an unit type to the available units list.
    public void addAvailableUnit(Unit.type myUnittype)
    {
        if(!availableUnits.Contains(myUnittype))
        {
            availableUnits.Add(myUnittype);
        }
        else
        {
            Debug.Log("Team:" + myUnittype + ". Unit is already available!");
        }
    }

    //Remove available unit.
    public void removeAvailableUnit(Unit.type myUnittype)
    {
        if (availableUnits.Contains(myUnittype))
        {
            availableUnits.Remove(myUnittype);
        }
        else
        {
            Debug.Log("Team:" + myUnittype + ". Unit was not available!");
        }
    }

    //Set the list of available units for a team.
    public void setAvailableUnits(Team team, List<Unit.type> newUnitList)
    {
        team.availableUnits.Clear();
        team.availableUnits = newUnitList;
    }

    //Makes all possible units available.
    public void setAllUnitsAvailable()
    {
        availableUnits.Add(Unit.type.Flak);
        availableUnits.Add(Unit.type.APC);
        availableUnits.Add(Unit.type.Artillery);
        availableUnits.Add(Unit.type.Battleship);
        availableUnits.Add(Unit.type.BCopter);
        availableUnits.Add(Unit.type.Bomber);
        availableUnits.Add(Unit.type.Cruiser);
        availableUnits.Add(Unit.type.Fighter);
        availableUnits.Add(Unit.type.Infantry);
        availableUnits.Add(Unit.type.Lander);
        availableUnits.Add(Unit.type.MdTank);
        availableUnits.Add(Unit.type.Mech);
        availableUnits.Add(Unit.type.Missiles);
        availableUnits.Add(Unit.type.Titantank);
        availableUnits.Add(Unit.type.Recon);
        availableUnits.Add(Unit.type.Rockets);
        availableUnits.Add(Unit.type.Sub);
        availableUnits.Add(Unit.type.Tank);
        availableUnits.Add(Unit.type.TCopter);       
    }

    //Sets the commander for this team.
    public void setTeamCommander(Database.commander myCommander)
    {
        teamCommander = myCommander;
    }
    public Database.commander getTeamCommander()
    {
        return teamCommander;
    }

    //Add enemy team, but only if it is not already in the list and it is not THIS team.
    public void addEnemyTeam(Team possibleEnemy)
    {
        if (possibleEnemy != this && !enemyTeams.Contains(possibleEnemy) && !alliedTeams.Contains(possibleEnemy))
        {
            enemyTeams.Add(possibleEnemy);
        }
        else
        {
            Debug.Log("Team: You are either trying to add your own team to the enemy teams list, this enemy team is already in the list or you try to add one of your allies to the enemy list!");
        }
    }

    public List<Team> getEnemyTeams()
    {
        return enemyTeams;
    }

    //Get/Set allied Teams
    public void addAlliedTeam(Team possibleAlly)
    {
        if (possibleAlly != this && !alliedTeams.Contains(possibleAlly) && !enemyTeams.Contains(possibleAlly) )
        {
            alliedTeams.Add(possibleAlly);
        }
        else
        {
            Debug.Log("Team: You are either trying to add your own team to the allied teams list, this possible allied team is already in the list or you try to add one of your enemies to the allied list!");
        }
    }

    public List<Team> getAlliedTeams()
    {
        return alliedTeams;
    }
}
