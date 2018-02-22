//created by Till Roßberg, 2017-18
//The container is supposed to be the link between the main menu and battleground. The settings you make in the main menu are stored here to be retrieved when you start the game.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public int nextLevelToLoad;
    List<List<Team>> teams = new List<List<Team>>();
    public bool fogOfWar = true;
    public Database.weather myWeather;
    public int moneyIncrement = 1000;//The money you get per round per building.
    public float battleDuration = 4;//Duration of the battle in days, 4 means no limited duration.
    public float propertiesToWin = 11;//Amount of properties one player needs to win the game, 11 turns this winning condition off.


    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(this);	
	}

    //Amount of properties one player needs to win the game
    public void setPropertiesToWin(float value)
    {
        propertiesToWin = value;
    }

    public float getPropertiesToWin()
    {
        return propertiesToWin;
    }
	
	//Duration of the battle
    public void setBattleDuration(float value)
    {
        battleDuration = value;
    }

    public float getBattleDuration()
    {
        return battleDuration;
    }

    //Moneyz
    public void setMoneyIncrement(int value)
    {
        moneyIncrement = value;
    }

    public int getMoneyIncrement()
    {
        return moneyIncrement;
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
