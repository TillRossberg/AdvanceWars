using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    MainFunctions mainFunctions;
    TeamManager teamManager;

    public Team activeTeam;
    public Database.weather actualWeather;
    public int roundCounter = 1;

    //Succession
    public List<int> succession = new List<int>();
    int successionCounter = 0;

    // Use this for initialization
    void Start ()
    {
        mainFunctions = GetComponent<MainFunctions>();
        teamManager = GetComponent<TeamManager>();
    }

    // Update is called once per frame
    void Update ()
    {
		
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
        activateUnits(team);
        GetComponent<StatusWindow>().displayGeneralInfo();

        //Adapt the GUI for the active team

        //Set fog of war.

        //Subtract fuel.

        //Give rations from properties and APCs.

        //Give funds for properties.

        //Subtract funds for repairing units.

        //Repair units and subtract money for repairing.
    }

    //End turn
    public void endTurn()
    {
        deactivateUnits(activeTeam);
        startTurn(getNextTeam());
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
        //If you get past the last entry of the succession list all teams had their turn, so increase the number of rounds and start again from the beginning. 
        if (successionCounter == teamManager.teams.Count)
        {
            successionCounter = 0;
            roundCounter++;
        }
        return teamManager.teams[succession[successionCounter]];//Look up the index of the team that has the next turn.
    }

    //When all teams had their turn change the weather (if random was selected), increase the round counter

    //Define the succession and set the first team that has a turn.
    public void initSuccession()
    {
        setupRandomSuccession();
        activeTeam = teamManager.teams[succession[0]];
        activateUnits(activeTeam);
    }

    //Defines the order in wich the teams have their turns. (TODO: find a better way to solve this...)
    public void setupRandomSuccession()
    {
        int counter = 0;
        while (succession.Count < teamManager.teams.Count)
        {
            int randomPick = Random.Range(0, teamManager.teams.Count);

            if (!succession.Contains(randomPick))
            {
                succession.Add(randomPick);
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
}
