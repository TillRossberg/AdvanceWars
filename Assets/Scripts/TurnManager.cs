using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    MainFunctions mainFunctions;
    TeamManager teamManager;

    public Team activeTeam;
    public Database.weather actualWeather;
    public int roundCounter = 1;//A round has passed, when all teams had their turn.

    //Succession
    public List<Team> succession = new List<Team>();
    int successionCounter = 0;
    
    public void init()
    {
        mainFunctions = GetComponent<MainFunctions>();
        teamManager = GetComponent<TeamManager>();
    }   

    //Manage who has turn.
    public void manageTurns()
    {

    }

    //Start turn
    public void startTurn(Team team)
    {
        mainFunctions.deselectObject();//Make sure nothing is selected when the next turn starts.
        activeTeam = team;
        GetComponent<StatusWindow>().displayGeneralInfo();//Adapt the GUI for the active team       
        activateUnits(team);       
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
        startTurn(getNextTeam());
    }

    //Give money for each property the team owns. 
    public void giveMoney(Team team)
    {
        team.money += team.ownedProperties.Count * GetComponent<MasterClass>().container.getMoneyIncrement();
        GetComponent<StatusWindow>().displayGeneralInfo();  
    }

    //Setup all the units of a team so they have a turn, can move and fire.
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
            //TODO: If the maximum amount of rounds has passed, check who won the game. (Depending on the occupied properties)

            //TODO: decide if more than two teams are playing and then only remove the defeated team from the map.
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
        int counter = 0;
        List<Team> allTeams = new List<Team>();
        //Fill all teams in a list...
        for(int i = 0; i < teamManager.getSuperTeamList().Count; i++)
        {
            for(int j = 0; j < teamManager.getSuperTeamList()[i].Count; j++)
            {
                allTeams.Add(teamManager.getSuperTeamList()[i][j]);
            }
        }
        //...and randomly pick teams from this list       
        while (succession.Count < allTeams.Count)
        {
            int randomPick = Random.Range(0, allTeams.Count);
            if (!succession.Contains(allTeams[randomPick]))
            {
                succession.Add(allTeams[randomPick]);
            }
            counter++;
            if (counter > 1000)
            {
                Debug.Log("TeamManager: Too many attempts to create a random succession!");
                break;
            }
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
            actualWeather = GetComponent<MasterClass>().container.getWeather();
        }
    }
    public Database.weather getWeather()
    {
        return actualWeather;
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
    public void findTeamWithMostProperties()
    {
        //Create a list to hold the super teams overall held properties.
        List<int> countingList = new List<int>();
        List<Team> teamList = new List<Team>();//To keep track of wich team has 
        int maxProperties = 0;
        int winnerTeamIndex = 0;
        for(int i = 0; i < teamManager.getSuperTeamList().Count; i++)
        {            
            for(int j = 0; j < teamManager.getSuperTeamList()[i].Count; j++)
            {

                countingList[i] += teamManager.getSuperTeamList()[i][j].ownedProperties.Count;//Add the amount of a teams properties to the super team amount.
                teamList.Add(teamManager.getSuperTeamList()[i][j]);
            }
        }
        //Find the team with the most properties
        for(int i = 0; i < countingList.Count; i++)
        {
            if(countingList[i] > maxProperties)
            {
                maxProperties = countingList[i];
                winnerTeamIndex = i;
            }
        }
        //Finally set the winning teams in the container to the list of teams that made it


    }
}
