//created by Till Roßberg, 2017-18
//The container is supposed to be the link between the main menu and battleground. The settings you make in the main menu are stored here to be retrieved when you start the game.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public int nextLevelToLoad;
    public List<Team> teams;
    private Team winnerTeam;
    public bool fogOfWar;
    public bool abilityPower;//Are the abilities for the players activated?
    public Database.weather myWeather;
    public int moneyIncrement;//The money you get per round per building.
    public float battleDuration;//Duration of the battle in days, 4 means no limited duration.
    public float propertiesToWin = 11;//Amount of properties one player needs to win the game, 11 turns this winning condition off.


    // Use this for initialization
    void Start()
    {
        //Initialize fields
        nextLevelToLoad = 0;
        teams = new List<Team>();
        abilityPower = true;
        moneyIncrement = 1000;
        battleDuration = 4;
        propertiesToWin = 11;
        myWeather = Database.weather.Clear;
        initTeams(2);

        DontDestroyOnLoad(this);
    }

//Teams
    public void setTeamParameters(int teamIndex, Database.commander teamCommander, Color teamColor)
    {

    }

    //Init the list of the teams.
    public void initTeams(int count)
    {
        teams.Clear();
        for (int i = 0; i < count; i++)
        {
            Team team = ScriptableObject.CreateInstance("Team") as Team;            
            teams.Add(team);
        }
        teams[1].setTeamCommander(Database.commander.Max);
    }

    //Get/Set the general for the chosen team. (Currently only two opposing teams are possible.)
    public void setCommanderPlayerOne(int index)
    {
        teams[0].setTeamCommander((Database.commander)index);
    }
    public void setCommanderPlayerTwo(int index)
    {
        teams[1].setTeamCommander((Database.commander)index);
    }
    public Database.commander getTeamCommander(int teamIndex)
    {
        return teams[teamIndex].getTeamCommander();
    }

    //Teamcolor
    public void setTeamColor(int teamIndex, Color color)
    {
        teams[teamIndex].setTeamColor(color);
    }

    //Teamname
    public void setTeamName(int teamIndex, string name)
    {
        teams[teamIndex].setTeamName(name);
    }

    //Set/Get Winnerteam
    public void setWinnerTeam(Team winner)
    {
        winnerTeam = winner;
    }

    public Team getWinnerTeam()
    {
        return winnerTeam;
    }

    public void setTeams(List<Team> teams)
    {
        this.teams = teams;
    }

    public List<Team> getTeams()
    {
        return teams;
    }

//Multiplayer Options
    //Ability
    public void setAbility(bool value)
    {
        abilityPower = value;
    }

    //Amount of properties one player needs to win the game
    public void setPropertiesToWin(float value)
    {
        propertiesToWin = value;
    }

    public int getPropertyCountToWin()
    {
        if (propertiesToWin > 11)
        {
            return (int)(propertiesToWin);
        }
        else
        {
            return -1;
        }
    }

    //Duration of the battle
    public void setBattleDuration(float value)
    {
        battleDuration = value;
    }

    public int getBattleDuration()
    {
        return (int)(battleDuration);
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

    public int getNextLevelIndex()
    {
        return nextLevelToLoad;
    }

    public void initTestContainer01()
    {
        setNextLevel(2);
        myWeather = Database.weather.Clear;
        initTeams(2);
        setCommanderPlayerOne(0);
        setCommanderPlayerTwo(2);
        setTeamColor(0, Color.red);
        setTeamColor(1, Color.blue);
        setTeamName(0, "Team Andy");
        setTeamName(1, "Team Max");
        setFogOfWar(true);
    }
}
