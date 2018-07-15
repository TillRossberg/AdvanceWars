using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : ScriptableObject
{
    public string teamName;
    public Sprite playerPic;
    public Database.commander teamCommander;
    public List<Transform> myUnits = new List<Transform>();
    public List<Tile> ownedProperties = new List<Tile>();
    public List<Unit.type> availableUnits = new List<Unit.type>();//Units this team can build.
    public  List<int> unitsBuilt = new List<int>() {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};//Units this team hast built during the game.
    //0: Flak, 1: APC, 2: Tank, 3: Artillery, 4: Rocket, 5: Missile, 6: Titantank, 7: Recon, 8: Infantry, 9: MdTank,
    //10: Mech, 11: TCopter, 12: BCopter, 13: Bomber, 14: Fighter, 15: Lander, 16: Battleship, 17: Cruiser, 18: Sub
    public Material teamMaterial;
    public Color teamColor;
    public int money = 5000;//Current money

    public List<Team> enemyTeams = new List<Team>();//Holds all the enemy teams.
    public List<Team> alliedTeams = new List<Team>();//Holds all friendos.

    //Stats
    public int totalMoney = 0;//Counts the overall earned money.
    public int unitsBuiltCounter = 0;//Counts the overall created units.
    public int unitsKilledCounter = 0;//Counts how many unit this team destroyed.

    

    //Add an unit to the team, set its color to the teamcolor and pass information about the own team and the enemy team to the unit.
    public void addUnit(Transform unitToAdd)
    {
        myUnits.Add(unitToAdd);
        unitToAdd.GetComponent<Unit>().myTeam = this;
        unitToAdd.GetComponent<Unit>().enemyTeams = enemyTeams;        
        incUnitsBuilt(unitToAdd.GetComponent<Unit>().myUnitType);
    }     
    
    public List<Transform> getUnits()
    {
        return myUnits;
    }

    public List<Tile> getOwnedProperties()
    {
        return ownedProperties;
    }

    public void incUnitsBuilt(Unit.type myType)
    {
        switch (myType)
        {
            case Unit.type.Flak: incUnitBuiltCounter(0); break;
            case Unit.type.APC: incUnitBuiltCounter(1); break;
            case Unit.type.Tank: incUnitBuiltCounter(2); break;
            case Unit.type.Artillery: incUnitBuiltCounter(3); break;    
            case Unit.type.Rockets: incUnitBuiltCounter(4); break;
            case Unit.type.Missiles: incUnitBuiltCounter(5); break;
            case Unit.type.Titantank: incUnitBuiltCounter(6); break;
            case Unit.type.Recon: incUnitBuiltCounter(7); break;
            case Unit.type.Infantry: incUnitBuiltCounter(8); break;
            case Unit.type.MdTank: incUnitBuiltCounter(9); break;
            case Unit.type.Mech: incUnitBuiltCounter(10); break;
            case Unit.type.TCopter: incUnitBuiltCounter(11); break;
            case Unit.type.BCopter: incUnitBuiltCounter(12); break;
            case Unit.type.Bomber: incUnitBuiltCounter(13); break;
            case Unit.type.Fighter: incUnitBuiltCounter(14); break;
            case Unit.type.Lander: incUnitBuiltCounter(15); break;
            case Unit.type.Battleship: incUnitBuiltCounter(16); break;
            case Unit.type.Cruiser: incUnitBuiltCounter(17); break;
            case Unit.type.Sub: incUnitBuiltCounter(18); break;                
            case Unit.type.Pipe: Debug.Log("Team: incUnitsBuilt: Pipes cannot be built!"); break;

            default:
                Debug.Log("Team: incUnitsBuilt: No such unittype found!");
                break;
        }
    }

    private void incUnitBuiltCounter(int index)
    {
        unitsBuilt[index]++;
        unitsBuiltCounter++;
    }

    public int getUnitsBuiltCounter()
    {
        return unitsBuiltCounter;
    }

    public int getUnitsBuiltCount(Unit.type myType)
    {
        switch (myType)
        {
            case Unit.type.Flak: return unitsBuilt[0];
            case Unit.type.APC: return unitsBuilt[1];
            case Unit.type.Tank: return unitsBuilt[2];
            case Unit.type.Artillery: return unitsBuilt[3];
            case Unit.type.Rockets: return unitsBuilt[4];
            case Unit.type.Missiles: return unitsBuilt[5];
            case Unit.type.Titantank: return unitsBuilt[6];
            case Unit.type.Recon: return unitsBuilt[7];
            case Unit.type.Infantry: return unitsBuilt[8];
            case Unit.type.MdTank: return unitsBuilt[9];
            case Unit.type.Mech: return unitsBuilt[10];
            case Unit.type.TCopter: return unitsBuilt[11];
            case Unit.type.BCopter: return unitsBuilt[12];
            case Unit.type.Bomber: return unitsBuilt[13];
            case Unit.type.Fighter: return unitsBuilt[14];
            case Unit.type.Lander: return unitsBuilt[15];
            case Unit.type.Battleship: return unitsBuilt[16];
            case Unit.type.Cruiser: return unitsBuilt[17];
            case Unit.type.Sub: return unitsBuilt[18];
            case Unit.type.Pipe: return -1;

            default:
                Debug.Log("Team: incUnitsBuilt: No such unittype found!");
                return -1;
        }
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
        totalMoney += amount;
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
            //TODO: if the funds are insufficient, grey out the button. This else case should never be reached.
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

    public void setPlayerName(string name)
    {
        teamName = name;
    }
    public string getPlayerName()
    {
        return teamName;
    }

    public void setMoney(int amount)
    {
        money = amount;
    }
    public int getMoney()
    {
        return money;
    }

    public void setPlayerPic(Sprite pic)
    {
        this.playerPic = pic;
    }

    public Sprite getPlayerPic()
    {
        return playerPic;
    }

    public int getUnitsKilledCount()
    {
        return unitsKilledCounter;
    }

    public void incUnitsKilledCount()
    {
        unitsKilledCounter++;
    }

    public List<int> getUnitsBuilt()
    {
        return unitsBuilt;
    }

    public void setTeamColor(Color color)
    {
        teamColor = color;
    }

    public Color getTeamColor()
    {
        return teamColor;
    }

    public void setTeamName(string name)
    {
        teamName = name;
    }

    public string getTeamName()
    {
        return teamName;
    }

    public bool isInMyTeam(Unit unitToTest)
    {
        if(myUnits.Contains(unitToTest.transform))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
