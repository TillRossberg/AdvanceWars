using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public enum commander { Andy, Kanbei, Max}
    public enum weather { Clear, Rain, Snow }
    private List<string> levelNames = new List<string>() { "Level01", "Level02", "Level03" };

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    //Returns a list with the names of available levels.
    public List<string> getLevels()
    {
        return levelNames;
    }

    //Returns the cost for moving over this tile depending on the weather and the move type of the unit.
    public int getMovementCost(Tile.type myTileType, Unit.moveType myMoveType, weather myWeather)
    {
        switch(myWeather)
        {
            case weather.Clear:
                switch(myTileType)
                {
                    case Tile.type.Airport:
                        {
                            switch(myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;                                
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.City:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }                        
                    case Tile.type.Facility:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Forest:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 2;
                                case Unit.moveType.Wheels: return 3;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Mountain:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 2;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Plain:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 2;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Port:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return 1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return 1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Reef:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return -1;
                                case Unit.moveType.Lander: return 2;
                                case Unit.moveType.Mech: return -1;
                                case Unit.moveType.Ship: return 2;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.RoadBridge:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.RoadStraight:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.RoadCurve:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Sea:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return -1;
                                case Unit.moveType.Lander: return 1;
                                case Unit.moveType.Mech: return -1;
                                case Unit.moveType.Ship: return 1;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Shoal:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return 1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.River:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 2;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    default:
                        Debug.Log("Database(getMaxMoveDistance): No such tile type" + myTileType + "found!");
                        return -1;
                }

            case weather.Rain:
                switch (myTileType)
                {
                    case Tile.type.Airport:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.City:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Facility:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Forest:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 3;
                                case Unit.moveType.Wheels: return 4;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Mountain:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 2;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Plain:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 2;
                                case Unit.moveType.Wheels: return 3;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Port:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return 1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return 1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Reef:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return -1;
                                case Unit.moveType.Lander: return 2;
                                case Unit.moveType.Mech: return -1;
                                case Unit.moveType.Ship: return 2;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.RoadBridge:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.RoadStraight:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.RoadCurve:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Sea:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return -1;
                                case Unit.moveType.Lander: return 1;
                                case Unit.moveType.Mech: return -1;
                                case Unit.moveType.Ship: return 1;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Shoal:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return 1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.River:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 1;
                                case Unit.moveType.Foot: return 2;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    default:
                        Debug.Log("Database(getMaxMoveDistance): No such tile type" + myTileType + "found!");
                        return -1;
                }

            case weather.Snow:
                switch (myTileType)
                {
                    case Tile.type.Airport:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.City:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Facility:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Forest:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 3;
                                case Unit.moveType.Wheels: return 4;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Mountain:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 4;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 2;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Plain:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 2;
                                case Unit.moveType.Wheels: return 3;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Port:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return 2;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return 2;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Reef:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return -1;
                                case Unit.moveType.Lander: return 2;
                                case Unit.moveType.Mech: return -1;
                                case Unit.moveType.Ship: return 2;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.RoadBridge:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.RoadStraight:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.RoadCurve:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Sea:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return -1;
                                case Unit.moveType.Lander: return 2;
                                case Unit.moveType.Mech: return -1;
                                case Unit.moveType.Ship: return 2;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.Shoal:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 1;
                                case Unit.moveType.Lander: return 1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return 1;
                                case Unit.moveType.Wheels: return 1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    case Tile.type.River:
                        {
                            switch (myMoveType)
                            {
                                case Unit.moveType.Air: return 2;
                                case Unit.moveType.Foot: return 2;
                                case Unit.moveType.Lander: return -1;
                                case Unit.moveType.Mech: return 1;
                                case Unit.moveType.Ship: return -1;
                                case Unit.moveType.Treads: return -1;
                                case Unit.moveType.Wheels: return -1;
                                default:
                                    Debug.Log("Database(getMaxMoveDistance): No such move type found on " + myTileType + "!");
                                    return -1;
                            }
                        }
                    default:
                        Debug.Log("Database(getMaxMoveDistance): No such tile type" + myTileType + "found!");
                        return -1;
                }

            default:
                Debug.Log("Database: No such weathertype found!");
                return -1;
        }
    }

    //Returns the maximum ammunition for the primary weapon for a specified commander- and unittype. (All secondary weapons have infinite ammo!)
    public int getAmmo(Unit.type myUnitType, commander myCommandType)
    {
        switch(myCommandType)
        {            
            case commander.Andy:
                switch(myUnitType)
                {
                    case Unit.type.Flak: return 9;
                    case Unit.type.APC: return 0;
                    case Unit.type.Artillery: return 9;
                    case Unit.type.Battleship: return 9;
                    case Unit.type.BCopter: return 6;
                    case Unit.type.Bomber: return 9;
                    case Unit.type.Cruiser: return 9;
                    case Unit.type.Fighter: return 9;
                    case Unit.type.Infantry: return 0;
                    case Unit.type.Lander: return 0;
                    case Unit.type.MdTank: return 8;
                    case Unit.type.Mech: return 3;
                    case Unit.type.Missiles: return 6;
                    case Unit.type.Titantank: return 9;
                    case Unit.type.Recon: return 0;
                    case Unit.type.Rockets: return 6;
                    case Unit.type.Sub: return 6;
                    case Unit.type.Tank: return 9;
                    case Unit.type.TCopter: return 0;

                    default:
                        Debug.Log("Database(getAmmo()): Unittype " + myUnitType + " not found for " + myCommandType + "!");
                        return -1;
                }

            case commander.Max:

                return -1;

            case commander.Kanbei:

                return -1;

            default:
                Debug.Log("Database(getAmmo()): No such commander found!");
                return -1;
        }
    }

    //Returns the maximum move distance for a specified commander- and unittype.
    public int getMoveDistance(Unit.type myUnitType, commander myCommandType)
    {
        switch (myCommandType)
        {
            case commander.Andy:
                switch (myUnitType)
                {
                    case Unit.type.Flak: return 6;
                    case Unit.type.APC: return 6;
                    case Unit.type.Artillery: return 5;
                    case Unit.type.Battleship: return 5;
                    case Unit.type.BCopter: return 6;
                    case Unit.type.Bomber: return 7;
                    case Unit.type.Cruiser: return 6;
                    case Unit.type.Fighter: return 9;
                    case Unit.type.Infantry: return 3;
                    case Unit.type.Lander: return 6;
                    case Unit.type.MdTank: return 5;
                    case Unit.type.Mech: return 2;
                    case Unit.type.Missiles: return 5;
                    case Unit.type.Titantank: return 6;
                    case Unit.type.Recon: return 8;
                    case Unit.type.Rockets: return 5;
                    case Unit.type.Sub: return 5;
                    case Unit.type.Tank: return 6;
                    case Unit.type.TCopter: return 6;

                    default:
                        Debug.Log("Database(getMoveDistance()): Unittype " + myUnitType + " not found for " + myCommandType + "!");
                        return -1;
                }

            case commander.Max:

                return -1;

            case commander.Kanbei:

                return -1;

            default:
                Debug.Log("Database(getMoveDistance()): No such commander found!");
                return -1;
        }
    }

    //Returns the vision range for a specified commander- and unittype.
    public int getVision(Unit.type myUnitType, commander myCommandType)
    {
        switch (myCommandType)
        {
            case commander.Andy:
                switch (myUnitType)
                {
                    case Unit.type.Flak: return 2;
                    case Unit.type.APC: return 1;
                    case Unit.type.Artillery: return 1;
                    case Unit.type.Battleship: return 2;
                    case Unit.type.BCopter: return 3;
                    case Unit.type.Bomber: return 2;
                    case Unit.type.Cruiser: return 3;
                    case Unit.type.Fighter: return 2;
                    case Unit.type.Infantry: return 2;
                    case Unit.type.Lander: return 1;
                    case Unit.type.MdTank: return 1;
                    case Unit.type.Mech: return 2;
                    case Unit.type.Missiles: return 5;
                    case Unit.type.Titantank: return 1;
                    case Unit.type.Recon: return 5;
                    case Unit.type.Rockets: return 1;
                    case Unit.type.Sub: return 5;
                    case Unit.type.Tank: return 3;
                    case Unit.type.TCopter: return 2;

                    default:
                        Debug.Log("Database(getVision()): Unittype " + myUnitType + " not found for " + myCommandType + "!");
                        return -1;
                }

            case commander.Max:

                return -1;

            case commander.Kanbei:

                return -1;

            default:
                Debug.Log("Database(getVision()): No such commander found!");
                return -1;
        }
    }

    //Returns the minimum range for a specified commander- and unittype.
    public int getMinRange(Unit.type myUnitType, commander myCommandType)
    {
        switch (myCommandType)
        {
            case commander.Andy:
                switch (myUnitType)
                {
                    case Unit.type.Flak: return 1;
                    case Unit.type.APC: return 1;
                    case Unit.type.Artillery: return 2;
                    case Unit.type.Battleship: return 2;
                    case Unit.type.BCopter: return 1;
                    case Unit.type.Bomber: return 1;
                    case Unit.type.Cruiser: return 1;
                    case Unit.type.Fighter: return 1;
                    case Unit.type.Infantry: return 1;
                    case Unit.type.Lander: return 1;
                    case Unit.type.MdTank: return 1;
                    case Unit.type.Mech: return 1;
                    case Unit.type.Missiles: return 3;
                    case Unit.type.Titantank: return 1;
                    case Unit.type.Recon: return 1;
                    case Unit.type.Rockets: return 3;
                    case Unit.type.Sub: return 1;
                    case Unit.type.Tank: return 1;
                    case Unit.type.TCopter: return 1;

                    default:
                        Debug.Log("Database(getMinRange()): Unittype " + myUnitType + " not found for " + myCommandType + "!");
                        return -1;
                }

            case commander.Max:

                return -1;

            case commander.Kanbei:

                return -1;

            default:
                Debug.Log("Database(getMinRange()): No such commander found!");
                return -1;
        }
    }

    //Returns the maximum range for a specified commander- and unittype.
    public int getMaxRange(Unit.type myUnitType, commander myCommandType)
    {
        switch (myCommandType)
        {
            case commander.Andy:
                switch (myUnitType)
                {
                    case Unit.type.Flak: return 1;
                    case Unit.type.APC: return 1;
                    case Unit.type.Artillery: return 3;
                    case Unit.type.Battleship: return 6;
                    case Unit.type.BCopter: return 1;
                    case Unit.type.Bomber: return 1;
                    case Unit.type.Cruiser: return 1;
                    case Unit.type.Fighter: return 1;
                    case Unit.type.Infantry: return 1;
                    case Unit.type.Lander: return 1;
                    case Unit.type.MdTank: return 1;
                    case Unit.type.Mech: return 1;
                    case Unit.type.Missiles: return 5;
                    case Unit.type.Titantank: return 1;
                    case Unit.type.Recon: return 1;
                    case Unit.type.Rockets: return 5;
                    case Unit.type.Sub: return 1;
                    case Unit.type.Tank: return 1;
                    case Unit.type.TCopter: return 1;

                    default:
                        Debug.Log("Database(getMaxRange()): Unittype " + myUnitType + " not found for " + myCommandType + "!");
                        return -1;
                }

            case commander.Max:

                return -1;

            case commander.Kanbei:

                return -1;

            default:
                Debug.Log("Database(getMaxRange()): No such commander found!");
                return -1;
        }
    }

    //Returns the maximum fuel for a specified commander- and unittype.
    public int getMaxFuel(Unit.type myUnitType, commander myCommandType)
    {
        switch (myCommandType)
        {
            case commander.Andy:
                switch (myUnitType)
                {
                    case Unit.type.Flak: return 60;
                    case Unit.type.APC: return 70;
                    case Unit.type.Artillery: return 50;
                    case Unit.type.Battleship: return 99;
                    case Unit.type.BCopter: return 99;
                    case Unit.type.Bomber: return 99;
                    case Unit.type.Cruiser: return 99;
                    case Unit.type.Fighter: return 99;
                    case Unit.type.Infantry: return 99;
                    case Unit.type.Lander: return 99;
                    case Unit.type.MdTank: return 50;
                    case Unit.type.Mech: return 70;
                    case Unit.type.Missiles: return 50;
                    case Unit.type.Titantank: return 99;
                    case Unit.type.Recon: return 80;
                    case Unit.type.Rockets: return 50;
                    case Unit.type.Sub: return 60;
                    case Unit.type.Tank: return 70;
                    case Unit.type.TCopter: return 99;

                    default:
                        Debug.Log("Database(getMaxRange()): Unittype " + myUnitType + " not found for " + myCommandType + "!");
                        return -1;
                }

            case commander.Max:

                return -1;

            case commander.Kanbei:

                return -1;

            default:
                Debug.Log("Database(getMaxRange()): No such commander found!");
                return -1;
        }
    }

    //Returns the cost for a specified commander- and unittype.
    public int getCost(Unit.type myUnitType, commander myCommandType)
    {
        switch (myCommandType)
        {
            case commander.Andy:
                switch (myUnitType)
                {
                    case Unit.type.Flak: return 8000;
                    case Unit.type.APC: return 5000;
                    case Unit.type.Artillery: return 6000;
                    case Unit.type.Battleship: return 28000;
                    case Unit.type.BCopter: return 9000;
                    case Unit.type.Bomber: return 22000;
                    case Unit.type.Cruiser: return 18000;
                    case Unit.type.Fighter: return 20000;
                    case Unit.type.Infantry: return 1000;
                    case Unit.type.Lander: return 12000;
                    case Unit.type.MdTank: return 16000;
                    case Unit.type.Mech: return 3000;
                    case Unit.type.Missiles: return 12000;
                    case Unit.type.Titantank: return 22000;
                    case Unit.type.Recon: return 4000;
                    case Unit.type.Rockets: return 15000;
                    case Unit.type.Sub: return 20000;
                    case Unit.type.Tank: return 7000;
                    case Unit.type.TCopter: return 5000;

                    default:
                        Debug.Log("Database(getMaxRange()): Unittype " + myUnitType + " not found for " + myCommandType + "!");
                        return -1;
                }

            case commander.Max:

                return -1;

            case commander.Kanbei:

                return -1;

            default:
                Debug.Log("Database(getMaxRange()): No such commander found!");
                return -1;
        }
    }

    //Returns the description for a specified unittype.
    public string getDescription(Unit.type myUnitType)
    {
        switch (myUnitType)
        {
            case Unit.type.Flak: return "This vehicle can take down helicopters and infantry units in one shot using the vulcan cannon, but it lacks the armor tanks have.";
            case Unit.type.APC: return "The APC unit is the troop carrier of the game - it cannot attack, as it has no weapons, but can transport one unit of either Infantry or Mech, supply any units it is next to and also soak up a lot of fire before being destroyed.";
            case Unit.type.Artillery: return "This unit attacks from a distance, cannot counter-attack, and in addition it cannot move and attack on the same turn. Cheaper than a regular tank, it is by far more powerful.";
            case Unit.type.Battleship: return "This ship is an indirect combat unit of the sea but the pride of most navys.";
            case Unit.type.BCopter: return "Use this thing to take down most enemies but watch out for fighters. This is the most versatile unit, it is able to attack everything except planes.";
            case Unit.type.Bomber: return "Use this plane to support ground troops. Bombers can take down most ground units";
            case Unit.type.Cruiser: return "Use this ship to bring down enemy jets flying over the sea or attack subs. Transports two copters.";
            case Unit.type.Fighter: return "Use this jet to shoot down flying attackers when Anti Air Vehicles are not able to stop them alone.";
            case Unit.type.Infantry: return "The Infantry unit is the basic ground unit. Although the weakest of all units, it serves the purpose of capturing cities.";
            case Unit.type.Lander: return "Transports two ground units.";
            case Unit.type.MdTank: return "The strength of the medium tank lies not with its firepower, but its armor instead. A regular tank gets shredded once shelled by an artillery piece or attacked by a mech, but a medium tank can survive more than 2 hits from both instances.";
            case Unit.type.Mech: return "The Mech unit is a MECHanized form of the Infantry, but notably slower than it's counterpart. At the cost of 3000 it packs an anti-vehicle rocket launcher that can be used to great effect. Like the Infantry, it can also capture cities. Surprisingly the Mech's movement is not taxed by mountain travel. Both infantry units can increase their line of sight by 3 if they move to a mountain terrain.";
            case Unit.type.Missiles: return "This unit can attack air units from a distance. Cannot attack ground or naval units.";
            case Unit.type.Titantank: return "Essentially a medium tank with more firepower, more armor, and 1 more unit of mobility. Downside is the price.";
            case Unit.type.Recon: return "The Recon unit is the lowest priced vehicle that is very mobile - especially on the roads. As a RECONnaissance unit they provide a large line of site during the Fog of War. Otherwise, this is an unarmored vehicle that has the same machine gun the small tank has.";
            case Unit.type.Rockets: return "This unit can attack units from a distance. Weak against Direct combat units, stronger than Artillery with a longer range.";
            case Unit.type.Sub: return "Good against most naval foes but hates the cruisers anti sub missiles.";
            case Unit.type.Tank: return "The Tank is the basic ground vehicle, useful in its mobility due to its tracks and its versatility of being able to attack a large number of units.";
            case Unit.type.TCopter: return "Transports one infantry.";

            default:
                Debug.Log("Database(getDescription()): Unittype " + myUnitType + "not found!");
                return "nonono";
        }

    }
}
