//created by Till Roßberg, 2017-18
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterClass : MonoBehaviour
{
    //Datastructures    
    public Container container;
    public GameObject containerPrefab;

	// Use this for initialization
	void Start ()
    {
        container = getContainer();
        GetComponent<MapCreator>().init();
        //GetComponent<TeamManager>().initTeams();
        GetComponent<TeamManager>().initTeamsFromContainer();
        GetComponent<MainFunctions>().loadLevel(container.getNextLevelIndex());
        //GetComponent<TurnManager>().currentWeather = container.getWeather();
        GetComponent<TurnManager>().init();
        //GetComponent<TurnManager>().initSuccession();
        //GetComponent<TurnManager>().setFogOfWar(GetComponent<TurnManager>().activeTeam);
        GetComponent<StatusWindow>().displayGeneralInfo();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //Check if the data container with a game setup created in the main menu is existing and return it. If not, we create a default container.
    public Container getContainer()
    {
        if (GameObject.FindWithTag("Container") != null)
        {
            return  GameObject.FindWithTag("Container").GetComponent<Container>();
        }
        else
        {
            Debug.Log("MasterClass: No container found, loading default container!");
            Container container = Instantiate(containerPrefab).GetComponent<Container>();
            container.initTestContainer01();
            return container;
        }
    }
}
