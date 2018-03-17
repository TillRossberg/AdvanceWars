//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitCreator : MonoBehaviour
{
    //Required data structures   
    private MapCreator myGraph = new MapCreator();
    public Transform unitPrefab;
    Database database;

    //Graphics
    [SerializeField] List<Sprite> thumbnails = new List<Sprite>();
    [SerializeField] List<Mesh> meshes = new List<Mesh>();

    //Teamcolors
    public Material redColor;
    public Material blueColor;

    // Use this for initialization
    void Start ()
    {		
        myGraph = this.GetComponent<MapCreator>();
        database = this.GetComponent<Database>();
    }
	
	public void createUnitSet00()
    {
        Team teamRed = this.GetComponent<TeamManager>().getTeam("TeamRed");
        Team teamBlue = this.GetComponent<TeamManager>().getTeam("TeamBlue");
        createUnit(Database.commander.Andy, Unit.type.Tank, teamRed, 5, 8, 180);
        createUnit(Database.commander.Andy, Unit.type.Infantry, teamRed, 5, 7, 180);
        createUnit(Database.commander.Andy, Unit.type.Infantry, teamRed, 13, 8, 180);
        createUnit(Database.commander.Andy, Unit.type.Mech, teamRed, 5, 6, 180);
        createUnit(Database.commander.Andy, Unit.type.Recon, teamRed, 5, 5, 180);
        createUnit(Database.commander.Andy, Unit.type.APC, teamRed, 4, 5, 180);
        createUnit(Database.commander.Andy, Unit.type.Artillery, teamRed, 3, 5, 180);
        createUnit(Database.commander.Andy, Unit.type.Flak, teamRed, 2, 5, 180);
        createUnit(Database.commander.Andy, Unit.type.Rockets, teamRed, 6, 6, 180);
        createUnit(Database.commander.Andy, Unit.type.MdTank, teamRed, 6, 9, 180);

        createUnit(Database.commander.Andy, Unit.type.Tank, teamBlue, 10, 8, 0);
        createUnit(Database.commander.Andy, Unit.type.Rockets, teamBlue, 9, 6, 0);
        createUnit(Database.commander.Andy, Unit.type.Infantry, teamBlue, 9, 9, 0);
    }

    public void createUnitSet01()
    {
        Team teamRed = this.GetComponent<TeamManager>().getTeam("TeamRed");
        Team teamBlue = this.GetComponent<TeamManager>().getTeam("TeamBlue");
        createUnit(Database.commander.Andy, Unit.type.Tank, teamRed, 0, 0, 180);
        createUnit(Database.commander.Andy, Unit.type.Tank, teamBlue, 11, 11 , 180);

    }

    //Create a unit for the given team, position and rotation.
    public void createUnit(Database.commander myCommanderType, Unit.type myUnitType, Team team,  int x, int y, int rotation)
    {
        switch(myUnitType)
        {
            case Unit.type.Infantry:

                //Create the Unit
                Transform unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
                Unit unit = unitTransform.GetComponent<Unit>();
                unit.rotateUnit(rotation);
                myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
                this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.
                //Properties
                //Graphics
                unit.setThumbnail(thumbnails[0]);
                unitTransform.GetComponent<MeshFilter>().mesh = meshes[3];
                //Name, position, rotation, unittype and -movement
                unit.unitName = "Infantry";
                unitTransform.name = "Infantry" + unit.myTeam.unitCounter;
                setPosition(unit, x, y, rotation);
                unit.myUnitType = myUnitType;
                unit.myMoveType = Unit.moveType.Foot;
                //Game relevant
                unit.directAttack = true;
                setProperties(unit, myUnitType, myCommanderType);
                break;

            case Unit.type.Mech:

                //Create the Unit
                unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
                unit = unitTransform.GetComponent<Unit>();
                unit.rotateUnit(rotation);
                myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
                this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.
                //Properties
                //Graphics
                unit.setThumbnail(thumbnails[0]);
                unitTransform.GetComponent<MeshFilter>().mesh = meshes[4];
                //Name, position, rotation, unittype and -movement
                unit.unitName = "Mech";
                unitTransform.name = "Mech" + unit.myTeam.unitCounter;
                setPosition(unit, x, y, rotation);
                unit.myUnitType = myUnitType;
                unit.myMoveType = Unit.moveType.Mech;
                //Game relevant
                unit.directAttack = true;
                setProperties(unit, myUnitType, myCommanderType);
                break;

            case Unit.type.Recon:

                //Create the Unit
                unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
                unit = unitTransform.GetComponent<Unit>();
                unit.rotateUnit(rotation);
                myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
                this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.
                //Properties
                //Graphics
                unit.setThumbnail(thumbnails[0]);
                unitTransform.GetComponent<MeshFilter>().mesh = meshes[5];
                //Name, position, rotation, unittype and -movement
                unit.unitName = "Recon";
                unitTransform.name = "Recon" + unit.myTeam.unitCounter;
                setPosition(unit, x, y, rotation);
                unit.myUnitType = myUnitType;
                unit.myMoveType = Unit.moveType.Wheels;
                //Game relevant
                unit.directAttack = true;
                setProperties(unit, myUnitType, myCommanderType);
                break;

            case Unit.type.APC:

                //Create the Unit
                unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
                unit = unitTransform.GetComponent<Unit>();
                unit.rotateUnit(rotation);
                myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
                this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.
                //Properties
                //Graphics
                unit.setThumbnail(thumbnails[0]);
                unitTransform.GetComponent<MeshFilter>().mesh = meshes[6];
                //Name, position, rotation, unittype and -movement
                unit.unitName = "APC";
                unitTransform.name = "APC" + unit.myTeam.unitCounter;
                setPosition(unit, x, y, rotation);
                unit.myUnitType = myUnitType;
                unit.myMoveType = Unit.moveType.Treads;
                //Game relevant
                unit.directAttack = false;
                setProperties(unit, myUnitType, myCommanderType);
                break;

            case Unit.type.Artillery:

                //Create the Unit
                unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
                unit = unitTransform.GetComponent<Unit>();
                unit.rotateUnit(rotation);
                myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
                this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.
                //Properties
                //Graphics
                unit.setThumbnail(thumbnails[0]);
                unitTransform.GetComponent<MeshFilter>().mesh = meshes[7];
                //Name, position, rotation, unittype and -movement
                unit.unitName = "Artillery";
                unitTransform.name = "Artillery" + unit.myTeam.unitCounter;
                setPosition(unit, x, y, rotation);
                unit.myUnitType = myUnitType;
                unit.myMoveType = Unit.moveType.Treads;
                //Game relevant
                unit.rangeAttack = true;
                setProperties(unit, myUnitType, myCommanderType);
                break;

            case Unit.type.Flak:

                //Create the Unit
                unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
                unit = unitTransform.GetComponent<Unit>();
                unit.rotateUnit(rotation);
                myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
                this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.
                //Properties
                //Graphics
                unit.setThumbnail(thumbnails[0]);
                unitTransform.GetComponent<MeshFilter>().mesh = meshes[8];
                //Name, position, rotation, unittype and -movement
                unit.unitName = "Flak";
                unitTransform.name = "Flak" + unit.myTeam.unitCounter;
                setPosition(unit, x, y, rotation);
                unit.myUnitType = myUnitType;
                unit.myMoveType = Unit.moveType.Treads;
                //Game relevant
                unit.directAttack = true;
                setProperties(unit, myUnitType, myCommanderType);
                break;

            case Unit.type.Tank:
                
                //Create the Unit
                unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y),Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
                unit = unitTransform.GetComponent<Unit>();
                unit.rotateUnit(rotation);
                myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
                this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.
                //Properties
                //Graphics
                unit.setThumbnail(thumbnails[0]);
                unitTransform.GetComponent<MeshFilter>().mesh = meshes[0];
                //Name, position, rotation, unittype and -movement
                unit.unitName = "Tank";
                unitTransform.name = "Tank" + unit.myTeam.unitCounter;
                setPosition(unit, x, y, rotation);
                unit.myUnitType = myUnitType;
                unit.myMoveType = Unit.moveType.Treads;
                //Game relevant
                unit.directAttack = true;
                setProperties(unit, myUnitType, myCommanderType);
                break;

            case Unit.type.MdTank:

                //Create the Unit
                unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
                unit = unitTransform.GetComponent<Unit>();
                unit.rotateUnit(rotation);
                myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
                this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.
                //Properties
                //Graphics
                unit.setThumbnail(thumbnails[0]);
                unitTransform.GetComponent<MeshFilter>().mesh = meshes[2];
                //Name, position, rotation, unittype and -movement
                unit.unitName = "Medium Tank";
                unitTransform.name = "Medium Tank" + unit.myTeam.unitCounter;
                setPosition(unit, x, y, rotation);
                unit.myUnitType = Unit.type.MdTank;
                unit.myMoveType = Unit.moveType.Treads;
                //Game relevant
                unit.directAttack = true;
                setProperties(unit, myUnitType, myCommanderType);
                break;

            case Unit.type.Rockets:
                
                //Create the Unit
                unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
                unit = unitTransform.GetComponent<Unit>();
                unit.rotateUnit(rotation);
                myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
                this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.
                //Properties
                //Graphics
                unit.setThumbnail(thumbnails[1]);
                unitTransform.GetComponent<MeshFilter>().mesh = meshes[1];
                //Name, position, rotation, unittype and -movement
                unit.unitName = "Rockets";
                unitTransform.name = "Rockets" + unit.myTeam.unitCounter;
                setPosition(unit, x, y, rotation);
                unit.myUnitType = Unit.type.Rockets;
                unit.myMoveType = Unit.moveType.Wheels;
                //Game relevant
                unit.rangeAttack = true;
                setProperties(unit, myUnitType, myCommanderType);
                break;

            default:
            Debug.Log("Unitmanager: Unittype not found!");
            break;

        }
    }

    //Sets the rotation and position for a given unit.
    public void setPosition(Unit unit, int x, int y, int rotation)
    {
        unit.xPos = x;
        unit.yPos = y;
        unit.rotation = rotation;
    }

    //Sets the properties for a given unit depending on the commander and the unittype.
    public void setProperties(Unit unit, Unit.type myUnitType, Database.commander myCommanderType)
    {
        unit.maxAmmo = database.getAmmo(myUnitType, myCommanderType);
        unit.ammo = unit.maxAmmo;
        unit.maxFuel = database.getMaxFuel(myUnitType, myCommanderType);
        unit.fuel = unit.maxFuel;
        unit.moveDist = database.getMoveDistance(myUnitType, myCommanderType);
        unit.visionRange = database.getVision(myUnitType, myCommanderType);
        unit.minRange = database.getMinRange(myUnitType, myCommanderType);
        unit.maxRange = database.getMaxRange(myUnitType, myCommanderType);
        unit.cost = database.getCost(myUnitType, myCommanderType);
    }
}
