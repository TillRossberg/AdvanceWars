using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    //General 
    public List<Team> teams = new List<Team>();
    public List<Material> teamColors = new List<Material>();

    public List<int> succession = new List<int>();
    int successionCounter = 0;

    //Create a team with a name and a color from the teamColors list and add it to the teams list.
    public void createTeam(string myTeamName, int colorNumber)
    {
        Team team = ScriptableObject.CreateInstance("Team") as Team;
        team.name = myTeamName;
        team.teamColor = teamColors[colorNumber];
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
            tile.setMaterial(newOwner.teamColor);
            //Add the tile to the new owners properties.
            newOwner.ownedProperties.Add(tile);
        }
        else
        {
            Debug.Log("TeamManager: Given tile (X:" + tile.xPos + " Y:" + tile.yPos + ") is not a property!");
        }
    }

    //Gets the next team in the succesion.
    public Team getNextTeam()
    {
        successionCounter++;
        //If you get past the last entry of the succession list all teams had their turn, so increase the number of rounds and start again from the beginning. 
        if(successionCounter == teams.Count)
        {
            successionCounter = 0;
            GetComponent<MasterClass>().dayCounter++;
        }
        return teams[succession[successionCounter]];//Look up the index of the team that has the next turn.
    }

    //Defines the order in wich the teams have their turns. (TODO: find a better way to solve this...)
    public void setupRandomSuccession()
    {
        int counter = 0;
        while(succession.Count < this.teams.Count)
        {
            int randomPick = Random.Range(0, this.teams.Count );

            if(!succession.Contains(randomPick))
            {
                succession.Add(randomPick);
            }
            counter++;
            if(counter > 1000)
            {
                Debug.Log("TeamManager: Too many attempts to create a random succession!");
                break;
            }
        }
    }
}
