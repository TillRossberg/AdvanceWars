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
        GetComponent<TurnManager>().init();
        GetComponent<MapCreator>().init();
        GetComponent<TeamManager>().setupTeams();
        container = getContainer();
        GetComponent<TurnManager>().actualWeather = container.getWeather();
        GetComponent<MainFunctions>().loadLevel(0);
        GetComponent<TurnManager>().initSuccession();
        GetComponent<StatusWindow>().displayGeneralInfo();
        GetComponent<TurnManager>().setFogOfWar(GetComponent<TurnManager>().activeTeam);
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
            return  Instantiate(containerPrefab).GetComponent<Container>();           
        }
    }
}
