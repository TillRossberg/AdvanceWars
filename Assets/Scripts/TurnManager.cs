using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    MainFunctions mainFunctions;
    TeamManager teamManager;

    public Team activeTeam;
    public Database.weather currentWeather;
    public int roundCounter = 1;//A round has passed, when all teams had their turn.

    //Succession
    public List<Team> succession = new List<Team>();
    int successionCounter = 0;
    
    public void init()
    {
        mainFunctions = GetComponent<MainFunctions>();
        teamManager = GetComponent<TeamManager>();
        currentWeather = GetComponent<MasterClass>().getContainer().getWeather();
        initSuccession();
        setFogOfWar(activeTeam);
    }      

    //Start turn
    public void startTurn()
    {
        mainFunctions.deselectObject();//Make sure nothing is selected when the next turn starts.
        activeTeam = getNextTeam();
        GetComponent<StatusWindow>().displayGeneralInfo();//Update the GUI for the active team       
        activateUnits(activeTeam);       
        setFogOfWar(activeTeam);//Set fog of war for this team.

        //Subtract fuel.

        //Give rations from properties and APCs.

        giveMoney(activeTeam);//Give money for properties.

        //Repair units.

        //Subtract money for repairing units.
    }

    //End turn
    public void endTurn()
    {
        deactivateUnits(activeTeam);
        GetComponent<MapCreator>().resetFogOfWar();
        startTurn();
    }

    //Give money for each property the team owns. 
    public void giveMoney(Team team)
    {
        team.money += team.ownedProperties.Count * GetComponent<MasterClass>().container.getMoneyIncrement();
        GetComponent<StatusWindow>().displayGeneralInfo();  
    }

    //Sets all the units of a team so they have a turn, can move and fire.
    public void activateUnits(Team teamToActivate)
    {
        for (int i = 0; i < teamToActivate.myUnits.Count; i++)
        {
            if (teamToActivate.myUnits[i] != null)
            {
                teamToActivate.myUnits[i].GetComponent<Unit>().hasTurn = true;
                teamToActivate.myUnits[i].GetComponent<Unit>().hasMoved = false;
                teamToActivate.myUnits[i].GetComponent<Unit>().canFire = true;
                teamToActivate.myUnits[i].GetComponent<Unit>().setIsInterrupted(false);
            }
        }
    }

    //Set the properties of all units of a team so they don't have a turn.
    public void deactivateUnits(Team teamToDeactivate)
    {
        for (int i = 0; i < teamToDeactivate.myUnits.Count; i++)
        {
            if (teamToDeactivate.myUnits[i] != null)
            {
                teamToDeactivate.myUnits[i].GetComponent<Unit>().hasTurn = false;
            }
        }
    }

    //Sets the team that now has its turn.
    public void setActiveTeam(Team team)
    {
        activeTeam = team;
    }
    public Team getActiveTeam()
    {
        return activeTeam;
    }

    //Gets the next team in the succesion.
    public Team getNextTeam()
    {
        successionCounter++;
        //If you get past the last entry of the succession list all teams had their turn, so end the round and start again from the beginning. 
        if (successionCounter == succession.Count)
        {
            successionCounter = 0;
            endRound();
        }
        return succession[successionCounter];//Look up the index of the team that has the next turn.
    }

    //When all teams had their turn: change the weather (if random was selected), increase the round counter, check if the battle duration is ecxeeded
    public void endRound()
    {
        roundCounter++;
        if (roundCounter == GetComponent<MasterClass>().container.battleDuration && GetComponent<MasterClass>().container.battleDuration > 4)//Minimum for the duration of the battle is 5 rounds, if below this winning condition will never trigger.
        {
            //TODO: decide if more than two teams are playing and then only remove the defeated team from the map.

            //TODO: If the maximum amount of rounds has passed, check who won the game. (Depending on the occupied properties)
            GetComponent<LevelLoader>().loadGameFinishedScreenWithDelay();
        }
        setWeather();
    }

    //Define the succession and set the first team that has a turn.
    public void initSuccession()
    {
        setupRandomSuccession();
        activeTeam = succession[0];
        activateUnits(activeTeam);
    }

    //Defines the order in wich the teams have their turns. (TODO: find a better way to solve this...)
    public void setupRandomSuccession()
    {
        List<Team> tempList = new List<Team>(teamManager.getTeams());         
        while(tempList.Count > 0)
        {
            int randomPick = Random.Range(0, tempList.Count);
            succession.Add(tempList[randomPick]);
            tempList.Remove(tempList[randomPick]);
        }
    }

    //Sets the weather for the next day, if random weather was selcted. Otherwise simply take the preset weather.
    public void setWeather()
    {
        if (GetComponent<MasterClass>().container.getWeather() == Database.weather.Random)
        {
            //TODO: randomly chose a weather
        }
        else
        {
            currentWeather = GetComponent<MasterClass>().container.getWeather();
        }
    }
    public Database.weather getWeather()
    {
        return currentWeather;
    }

    //Each unit of the given team calculates its visiblity and marks the tiles it can see. (Only if fog of war was activated in the options)
    //Then the graph sets the visibility of what the team can see and what not. Owned properties always have vision.
    public void setFogOfWar(Team team)
    {
        if(GetComponent<MasterClass>().container.fogOfWar)
        {
            GetComponent<MapCreator>().resetFogOfWar();//Reset all tiles to invisible.            
            for(int i = 0; i < team.myUnits.Count; i++)
            {
                team.myUnits[i].GetComponent<Unit>().calcVisibleArea();
            }
            for(int i = 0; i  < team.ownedProperties.Count; i++)
            {
                team.ownedProperties[i].setVisible(true);
            }

            GetComponent<MapCreator>().setVisibility();
        }
    }

    //Count the properties of all the teams and the team with the most wins.
    //TODO: !WORKING
    public Team getTeamWithMostProperties()
    {
        Team winner = new Team();
        int highestPropertyCount = 0;
        for (int i = 0; i < teamManager.getTeams().Count; i++)
        {
            int propertyCount = teamManager.getTeams()[i].getOwnedProperties().Count;
            if(propertyCount > highestPropertyCount)
            {
                highestPropertyCount = propertyCount;
                winner = teamManager.getTeams()[i];
            }
        }
        return winner;
    }
}
