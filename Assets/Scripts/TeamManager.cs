//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    //General 
    public List<Team> teams = new List<Team>();   
    
    //Initiate the teams for this game with the info from the container. Define wich units they can build, wich teams are enemies and whos commander.
    public void setupTeams()
    {
        createTeam("TeamRed", 0);
        createTeam("TeamBlue", 1);
        getTeam("TeamRed").addEnemyTeam(this.GetComponent<TeamManager>().getTeam("TeamBlue"));
        getTeam("TeamBlue").addEnemyTeam(this.GetComponent<TeamManager>().getTeam("TeamRed"));
        getTeam("TeamBlue").setAllUnitsAvailable();
        getTeam("TeamRed").setAllUnitsAvailable();
        getTeam("TeamBlue").setTeamCommander(Database.commander.Max);
        getTeam("TeamRed").setTeamCommander(Database.commander.Andy);
    }

    //Create a team with a name and a color from the teamColors list and add it to the teams list.
    public void createTeam(string myTeamName, int colorNumber)
    {
        Team team = ScriptableObject.CreateInstance("Team") as Team;
        team.name = myTeamName;
        team.teamMaterial = GetComponent<Database>().getTeamMaterial(colorNumber);
        team.teamColor = GetComponent<Database>().getTeamColor(colorNumber);
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

    //Returns the list of all the teams.
    public List<Team> getTeamList()
    {
        return teams;
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

    //Adds a property to a team.
    public void occupyProperty(Team newOwner, Tile tile)
    {
        //Make sure the tile is a property.
        if(tile.myTileType == Tile.type.City || tile.myTileType == Tile.type.Facility || tile.myTileType == Tile.type.Airport || tile.myTileType == Tile.type.Port)
        {
            //If it was occupied by another team, delete it from their property list.
            if(tile.owningTeam != null)
            {
                tile.owningTeam.ownedProperties.Remove(tile);//Maybe confusing: delete the property from the list of the team that owned it.
                tile.owningTeam = null;                     
            }
            //Introduce the new owner to the tile.
            tile.owningTeam = newOwner;
            //Set the color of the property to the occupying team color.
            tile.setMaterial(newOwner.teamMaterial);
            //Add the tile to the new owners properties.
            newOwner.ownedProperties.Add(tile);
        }
        else
        {
            Debug.Log("TeamManager: Given tile (X:" + tile.xPos + " Y:" + tile.yPos + ") is not a property!");
        }
    }

    
}
