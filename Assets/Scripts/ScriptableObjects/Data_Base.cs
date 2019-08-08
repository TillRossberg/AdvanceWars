using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu (menuName ="Scriptable Objects/Database")]
public class Data_Base : ScriptableObject
{
    [Header("General")]
    public Data_Sounds Sounds;
    public GameObject containerPrefab;
    [Header("Graphics")]
    public GameObject cursorPrefab;
    public GameObject reachableTilePrefab;
    public GameObject attackableTilePrefab;
    public GameObject fogOfWarTilePrefab;
    public GameObject ArrowHead;
    public GameObject ArrowTail;
    public GameObject ArrowCurve;
    public float arrowPathHeight;    
    [Header("Commander")]
    public List<Sprite> commanderThumbs;
    [Header("Teams")]
    public List<Team> premadeTeams;
    [Header("Unitprefabs")]
    public GameObject anitAirPrefab;
    public GameObject APCPrefab;
    public GameObject tankPrefab;
    public GameObject artilleryPrefab;
    public GameObject rocketPrefab;
    public GameObject missilePrefab;
    public GameObject titanTankPrefab;
    public GameObject reconPrefab;
    public GameObject infantryPrefab;
    public GameObject mdTankPrefab;
    public GameObject mechPrefab;
    public GameObject tCopterPrefab;
    public GameObject bCopterPrefab;
    public GameObject bomberPrefab;
    public GameObject fighterPrefab;
    public GameObject landerPrefab;
    public GameObject battleShipPrefab;
    public GameObject cruiserPrefab;
    public GameObject subPrefab;
    public GameObject pipePrefab;

    [Header("Tileprefabs")]
    public GameObject plainPrefab;
    public GameObject forestPrefab;
    public GameObject roadPrefab;
    public GameObject mountainPrefab;
    public GameObject riverPrefab;
    public GameObject HQPrefab;
    public GameObject cityPrefab;
    public GameObject facilityPrefab;
    public GameObject airPortPrefab;
    public GameObject portPrefab;
    public GameObject shoalPrefab;
    public GameObject seaPrefab;
    public GameObject reefPrefab;

    #region Getter
    public GameObject GetTilePrefab(TileType type)
    {
        switch (type)
        {
            case TileType.Plain: return plainPrefab;
            case TileType.Forest: return forestPrefab;
            case TileType.Road: return roadPrefab;
            case TileType.Mountain: return mountainPrefab;
            case TileType.HQ: return HQPrefab;
            case TileType.City: return cityPrefab;
            case TileType.Facility: return facilityPrefab;
            case TileType.Airport: return airPortPrefab;
            case TileType.Port: return portPrefab;
            case TileType.Sea: return seaPrefab;
            case TileType.Reef: return reefPrefab;
            case TileType.River: return riverPrefab;
            case TileType.Shoal: return shoalPrefab;
            default: throw new System.Exception("Tile type not found: " + type.ToString());
        }
    }

    public GameObject GetUnitPrefab(UnitType type)
    {
        GameObject unit = null;
        switch (type)
        {
            case UnitType.Null: unit = null; break;
            case UnitType.AntiAir: unit = anitAirPrefab; break;
            case UnitType.APC: unit = APCPrefab; break;
            case UnitType.Tank: unit = tankPrefab; break;
            case UnitType.Artillery: unit = artilleryPrefab; break;
            case UnitType.Rockets: unit = rocketPrefab; break;
            case UnitType.Missiles: unit = missilePrefab; break;
            case UnitType.Titantank: unit = titanTankPrefab; break;
            case UnitType.Recon: unit = reconPrefab; break;
            case UnitType.Infantry: unit = infantryPrefab; break;
            case UnitType.MdTank: unit = mdTankPrefab; break;
            case UnitType.Mech: unit = mechPrefab; break;
            case UnitType.TCopter: unit = tCopterPrefab; break;
            case UnitType.BCopter: unit = bCopterPrefab; break;
            case UnitType.Bomber: unit = bomberPrefab; break;
            case UnitType.Fighter: unit = fighterPrefab; break;
            case UnitType.Lander: unit = landerPrefab; break;
            case UnitType.Battleship: unit = battleShipPrefab; break;
            case UnitType.Cruiser: unit = cruiserPrefab; break;
            case UnitType.Sub: unit = subPrefab; break;
            case UnitType.Pipe: unit = pipePrefab; break;
            default: throw new System.Exception("Unittype not found: " + type.ToString());
        }
        return unit; 
    }
    public int GetUnitCost(UnitType type)
    {
        return GetUnitPrefab(type).GetComponent<Unit>().data.cost;
    }
    public Sprite GetCommanderThumb(CommanderType type)
    {
        switch (type)
        {
            case CommanderType.Andy: return commanderThumbs[0];
            case CommanderType.Kanbei:return commanderThumbs[1];
            case CommanderType.Max:return commanderThumbs[2];
            default: throw new System.Exception("Commander type not found!");
        }
    }


    public string GetDescription(UnitType myUnitType)
    {
        switch (myUnitType)
        {
            case UnitType.AntiAir: return "This vehicle can take down helicopters and infantry units in one shot using the vulcan cannon, but it lacks the armor tanks have.";
            case UnitType.APC: return "The APC unit is the troop carrier of the game - it cannot attack, as it has no weapons, but can transport one unit of either Infantry or Mech, supply any units it is next to and also soak up a lot of fire before being destroyed.";
            case UnitType.Artillery: return "This unit attacks from a distance, cannot counter-attack, and in addition it cannot move and attack on the same turn. Cheaper than a regular tank, it is by far more powerful.";
            case UnitType.Battleship: return "This ship is an indirect combat unit of the sea but the pride of most navys.";
            case UnitType.BCopter: return "Use this thing to take down most enemies but watch out for fighters. This is the most versatile unit, it is able to attack everything except planes.";
            case UnitType.Bomber: return "Use this plane to support ground troops. Bombers can take down most ground units";
            case UnitType.Cruiser: return "Use this ship to bring down enemy jets flying over the sea or attack subs. Transports two copters.";
            case UnitType.Fighter: return "Use this jet to shoot down flying attackers when Anti Air Vehicles are not able to stop them alone.";
            case UnitType.Infantry: return "The Infantry unit is the basic ground unit. Although the weakest of all units, it serves the purpose of capturing cities.";
            case UnitType.Lander: return "Transports two ground units.";
            case UnitType.MdTank: return "The strength of the medium tank lies not with its firepower, but its armor instead. A regular tank gets shredded once shelled by an artillery piece or attacked by a mech, but a medium tank can survive more than 2 hits from both instances.";
            case UnitType.Mech: return "The Mech unit is a MECHanized form of the Infantry, but notably slower than it's counterpart. At the cost of 3000 it packs an anti-vehicle rocket launcher that can be used to great effect. Like the Infantry, it can also capture cities. Surprisingly the Mech's movement is not taxed by mountain travel. Both infantry units can increase their line of sight by 3 if they move to a mountain terrain.";
            case UnitType.Missiles: return "This unit can attack air units from a distance. It cannot attack ground or naval units.";
            case UnitType.Titantank: return "Essentially a medium tank with more firepower, more armor, and 1 more unit of mobility. Downside is the price.";
            case UnitType.Recon: return "The Recon unit is the lowest priced vehicle that is very mobile - especially on the roads. As a RECONnaissance unit they provide a large line of site during the Fog of War. Otherwise, this is an unarmored vehicle that has the same machine gun the small tank has.";
            case UnitType.Rockets: return "This unit can attack units from a distance. Weak against Direct combat units, stronger than Artillery with a longer range.";
            case UnitType.Sub: return "Good against most naval foes but hates the cruisers anti sub missiles.";
            case UnitType.Tank: return "The Tank is the basic ground vehicle, useful in its mobility due to its tracks and its versatility of being able to attack a large number of units.";
            case UnitType.TCopter: return "Transports one infantry.";

            default:
                Debug.Log("Database(getDescription()): Unittype " + myUnitType + "not found!");
                return "nonono";
        }

    }
    #endregion
}
