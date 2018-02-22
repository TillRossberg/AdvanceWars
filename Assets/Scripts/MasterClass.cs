//created by Till Roßberg, 2017-18
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterClass : MonoBehaviour
{
    //Datastructures
    TeamManager teamManager;
    MainFunctions mainFunctions;
    public Container container;

    public int dayCounter = 1;

	// Use this for initialization
	void Start ()
    {
        //Init
        mainFunctions = GetComponent<MainFunctions>();
        teamManager = GetComponent<TeamManager>();
        mainFunctions.activeCommander = Database.commander.Andy;
        mainFunctions.actualWeather = Database.weather.Clear;
        setupTeams();

        //Load a specified level with its predefined units. (The container exists only if we load a level from the main menu.)
        if (GameObject.FindWithTag("Container") != null)
        {
            container = GameObject.FindWithTag("Container").GetComponent<Container>();
            loadLevel(container.getNextLevel());
        }
        else
        {
            Debug.Log("No container found, loading level 01!");
            loadLevel(1);
        }
        setupSuccession();
       
        GetComponent<StatusWindow>().displayGeneralInfo();
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
        mainFunctions.activeTeam = team;
        activateUnits(team);
        //Adapt the GUI for the active team

        //Set fog of war.

        //Subtract fuel.

        //Give rations from properties and APCs.
        
        //Give funds for properties.

        //Subtract funds for repairing units.

        //Repair units.
    }

    //End turn
    public void endTurn()
    {
        deactivateUnits(mainFunctions.activeTeam);
        startTurn(teamManager.getNextTeam());
        GetComponent<StatusWindow>().displayGeneralInfo();
    }

    //Setup all the units of a team so they have a turn, can move and fire.
    public void activateUnits(Team teamToActivate)
    {
        for(int i = 0;  i < teamToActivate.myUnits.Count; i++)
        {
            if(teamToActivate.myUnits[i] != null)
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
            if(teamToDeactivate.myUnits[i] != null)
            {
                teamToDeactivate.myUnits[i].GetComponent<Unit>().hasTurn = false;
            }
        }
    }

    //Initiate the teams for this game, define wich units they can build and wich teams are enemies.
    public void setupTeams()
    {
        this.GetComponent<TeamManager>().createTeam("TeamRed", 0);
        this.GetComponent<TeamManager>().createTeam("TeamBlue", 1);
        this.GetComponent<TeamManager>().getTeam("TeamRed").addEnemyTeam(this.GetComponent<TeamManager>().getTeam("TeamBlue"));
        this.GetComponent<TeamManager>().getTeam("TeamBlue").addEnemyTeam(this.GetComponent<TeamManager>().getTeam("TeamRed"));
        this.GetComponent<TeamManager>().getTeam("TeamBlue").setAllUnitsAvailable();
        this.GetComponent<TeamManager>().getTeam("TeamRed").setAllUnitsAvailable();

    }

    //Define the succession and set the first team that has a turn.
    public void setupSuccession()
    {
        teamManager.setupRandomSuccession();
        mainFunctions.activeTeam = teamManager.teams[teamManager.succession[0]];
        activateUnits(mainFunctions.activeTeam);
    }

    //Load a specified level and spawn untis.
    public void loadLevel(int value)
    {
        switch(value)
        {
            case 0:
                this.GetComponent<Graph>().createLevel01();
                this.GetComponent<UnitCreator>().createUnitSet01();
                break;

            case 1:
                

                break;

            default:
                Debug.Log("MasterClass: No such level found!");
                break;

        }
    }
}
