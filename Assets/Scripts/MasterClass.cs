﻿//created by Till Roßberg, 2017-18
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterClass : MonoBehaviour
{
    //Datastructures
    TeamManager teamManager;
    MainFunctions mainFunctions;
    Container container;

   

	// Use this for initialization
	void Start ()
    {
        //Init
        mainFunctions = GetComponent<MainFunctions>();
        teamManager = GetComponent<TeamManager>();

        mainFunctions.activeCommander = Database.commander.Andy;
        mainFunctions.setupTeams();

        //Load a specified level with its predefined units. (The container exists only if we load a level from the main menu.)
        if (GameObject.FindWithTag("Container") != null)
        {
            container = GameObject.FindWithTag("Container").GetComponent<Container>();
            mainFunctions.actualWeather = container.getWeather();
            mainFunctions.loadLevel(container.getNextLevel());
        }
        else
        {
            Debug.Log("MasterClass: No container found, loading default map!");
            mainFunctions.activeCommander = Database.commander.Andy;
            mainFunctions.actualWeather = Database.weather.Clear;
            mainFunctions.loadLevel(0);
        }
        mainFunctions.setupSuccession();
        mainFunctions.activateUnits(mainFunctions.activeTeam);
        
        GetComponent<StatusWindow>().displayGeneralInfo();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    
}