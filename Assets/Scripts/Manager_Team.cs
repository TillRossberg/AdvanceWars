﻿//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Team : MonoBehaviour
{
    //References
    private Manager _manager;
    //General 
    public List<Team> teams; 
    
    public void init()
    {
        _manager = GetComponent<Manager>();
    }

    //Initiate the teams for this game with the info from the container. Define wich units they can build, wich teams are enemies and whos commander.
    //TODO: get the actual values from the container
    public void initTeams()
    {        
        createTeam("TeamRed", 0);
        createTeam("TeamBlue", 1);
        getTeam("TeamRed").setPlayerName("Ulf");
        getTeam("TeamRed").setPlayerPic(GetComponent<Database>().getCommanderThumb(Database.commander.Andy));
        getTeam("TeamBlue").setPlayerName("Zwulf");
        getTeam("TeamBlue").setPlayerPic(GetComponent<Database>().getCommanderThumb(Database.commander.Max));
        getTeam("TeamRed").addEnemyTeam(this.GetComponent<Manager_Team>().getTeam("TeamBlue"));
        getTeam("TeamBlue").addEnemyTeam(this.GetComponent<Manager_Team>().getTeam("TeamRed"));
        getTeam("TeamBlue").setAllUnitsAvailable();
        getTeam("TeamRed").setAllUnitsAvailable();
        getTeam("TeamBlue").setTeamCommander(Database.commander.Max);
        getTeam("TeamRed").setTeamCommander(Database.commander.Andy);
        GameObject.FindGameObjectWithTag("Container").GetComponent<Container>().setTeams(teams);
    }

    //TODO: expand this to generate a varying amount of teams...much, much later
    public void initTeamsFromContainer()
    {
        Container container = _manager.getContainer();
        teams = container.getTeams();
        Team team1 = teams[0];
        Team team2 = teams[1];
        //Team 1
        team1.addEnemyTeam(team2);
        team1.setAllUnitsAvailable();
        team1.name = team1.getTeamName();
        team1.teamMaterial = _manager.getDatabase().getTeamMaterial(2);        
        //Create empty game object in wich we will store the units for the team later.
        GameObject emptyGameObject = new GameObject();
        emptyGameObject.name = team1.name;
        emptyGameObject.transform.SetParent(this.transform);

        //Team 2
        team2.addEnemyTeam(team1);
        team2.setAllUnitsAvailable();
        team2.name = team2.getTeamName();
        team2.teamMaterial = _manager.getDatabase().getTeamMaterial(2);
        //Create empty game object in wich we will store the units for the team later.
        emptyGameObject = new GameObject();
        emptyGameObject.name = team2.name;
        emptyGameObject.transform.SetParent(this.transform);
    }

    //Create a team with a name and a color from the teamColors list and add it to the teams list.
    public void createTeam(string myTeamName, int colorNumber)
    {
        Team team = ScriptableObject.CreateInstance("Team") as Team;
        team.name = myTeamName;
        team.teamMaterial = _manager.getDatabase().getTeamMaterial(colorNumber);
        team.teamColor = _manager.getDatabase().getTeamColor(colorNumber);
        teams.Add(team);
        //Create empty game object in wich we will store the units for the team later.
        GameObject emptyGameObject = new GameObject();
        emptyGameObject.name = myTeamName;
        emptyGameObject.transform.SetParent(this.transform);
    }

    //Add an unit to a team.
    public void addUnit(Transform unit, Team myTeam)
    {        
        for (int i = 0; i < teams.Count; i++)
        {            
            if(teams[i] == myTeam)
            {
                teams[i].addUnit(unit);
            }            
        }
    }    

    //Searches for a team by a given name and returns it.
    public Team getTeam(string teamName)
    {
        for (int i = 0; i < teams.Count; i++)
        {           
            if(teamName == teams[i].name)
            {
                return teams[i];
            }            
        }
        Debug.Log("TeamManager: No such team found!");
        return null;
    }

    public Team getTeam(int index)
    {
        return teams[index];
    }
    //Returns all teams
    public List<Team> getTeams()
    {
        return teams;
    }

    //Adds a property to a team.
    public void occupyProperty(Team newOwner, Tile tile)
    {
        //Make sure the tile is a property.
        if(tile.myTileType == Tile.type.City || tile.myTileType == Tile.type.Facility || tile.myTileType == Tile.type.Airport || tile.myTileType == Tile.type.Port || tile.myTileType == Tile.type.HQ)
        {
            //If it was occupied by another team, delete it from their property list.
            if(tile.owningTeam != null)
            {
                tile.owningTeam.ownedProperties.Remove(tile);//This maybe confusing: we delete the property from the list of the team that owned it.
                tile.owningTeam = null;                     
            }
            //Introduce the new owner to the tile.
            tile.owningTeam = newOwner;
            //Set the color of the property to the occupying team color.
            tile.setMaterial(newOwner.teamMaterial);
            tile.setColor(newOwner.getTeamColor());
            //Add the tile to the new owners properties.
            newOwner.ownedProperties.Add(tile);
            //If you occupy the enemies HQ, you win the game.
            //TODO: find a better place for this
            if(tile.myTileType == Tile.type.HQ && _manager.getTurnManager().roundCounter > 1)
            {
                //TODO: decide if more than two teams are playing and then only remove the defeated team from the map.
                //TODO: winning animationstuff
                _manager.getSceneLoader().loadGameFinishedScreenWithDelay();
            }
            //If you reach the necessary amount of properties you also win the game.
            //!WORKING
            if(newOwner.ownedProperties.Count == _manager.getContainer().getPropertyCountToWin())
            {
                //TODO: decide if more than two teams are playing and then only remove the defeated team from the map.
                //TODO: winning animationstuff
                _manager.getSceneLoader().loadGameFinishedScreenWithDelay();
            }
        }
        else
        {
            Debug.Log("TeamManager: Given tile (X:" + tile.xPos + " Y:" + tile.yPos + ") is not a property!");
        }
    }    
}