using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public int nextLevelToLoad;
    List<List<Team>> teams = new List<List<Team>>();
    public bool fogOfWar = true;
    public Database.weather myWeather;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this);	
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    //Weather
    public void setWeather(int value)
    {
        myWeather = (Database.weather)value;       
    }

    public Database.weather getWeather()
    {
        return myWeather;
    }
        
    //Fog of war
    public void setFogOfWar(bool value)
    {
        fogOfWar = value;
    }

    public bool getFogOfWar()
    {
        return fogOfWar;
    }

    //Level
    public void setNextLevel(int value)
    {
        nextLevelToLoad = value;
    }

    public int getNextLevel()
    {
        return nextLevelToLoad;
    }
}
