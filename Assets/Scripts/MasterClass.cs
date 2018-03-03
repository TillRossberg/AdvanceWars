//created by Till Roßberg, 2017-18
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterClass : MonoBehaviour
{
    //Datastructures
    public TeamManager teamManager;
    MainFunctions mainFunctions;
    public Container container;
    public GameObject containerPrefab;

	// Use this for initialization
	void Start ()
    {        
        //Init
        mainFunctions = GetComponent<MainFunctions>();
        teamManager = GetComponent<TeamManager>();
        GetComponent<TurnManager>().init();
        GetComponent<Graph>().init();

        teamManager.setupTeams();

        //Load a specified level with its predefined units. (The container exists only if we load a level from the main menu.)
        if (GameObject.FindWithTag("Container") != null)
        {
            container = GameObject.FindWithTag("Container").GetComponent<Container>();
            GetComponent<TurnManager>().actualWeather = container.getWeather();
            mainFunctions.loadLevel(container.getNextLevel());
        }
        else
        {
            Debug.Log("MasterClass: No container found, loading default container!");
            container = Instantiate(containerPrefab).GetComponent<Container>();
            GetComponent<TurnManager>().actualWeather = Database.weather.Clear;
            mainFunctions.loadLevel(0);
        }
        GetComponent<TurnManager>().initSuccession();
        GetComponent<StatusWindow>().displayGeneralInfo();
        GetComponent<TurnManager>().setFogOfWar(GetComponent<TurnManager>().activeTeam);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}    
}
