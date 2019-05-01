using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    #region References
    [SerializeField] Data_Base _database;
    public Data_Base Database { get { return _database; } }
    [SerializeField] Data_MapSettings _mapSettings;
    public Data_MapSettings MapSettings { get { return _mapSettings; } }
    [SerializeField] Data_Map _mapData;
    public Data_Map MapData { get { return _mapData; } }
    public Container Container { get; private set; }
    public Manager_Team TeamManager { get; private set; }
    public Calculations_Battle BattleCalculations { get; private set; }
    #endregion
    #region Fields
    public List<List<Tile>> MapMatrix { get; private set; }
    public float tileHeight = 0;
    public float inputDelay = 0.145f;
    public float buttonHoldDelay = 0.1f;
    #endregion
    #region Team fields
    public List<Team> teams = new List<Team>();
    #endregion
    #region Succession Fields    
    public List<Team> Succession { get; private set; }
    int _successionCounter = 0;
    #endregion
    #region Parent Objects    
    public Transform tileParent;
    public Transform fogOfWarParent;
    public Transform arrowPathParent;
    #endregion

    #region Init Methods
    public void Init()
    {
        TeamManager = new Manager_Team();
        BattleCalculations = new Calculations_Battle();
    }
    //Check if the data container with a game setup created in the main menu is existing and return it. If not, we create a default container.
    private void InitContainer()
    {
        if (GameObject.FindWithTag("Container") != null)
        {
            Container = GameObject.FindWithTag("Container").GetComponent<Container>();
        }
        else
        {
            Debug.Log("MasterClass: No container found, loading default container!");
            Container container = Instantiate(Database.containerPrefab).GetComponent<Container>();
            this.Container = container;
        }
    }
    public void InitMap()
    {
        CreateEmptyMatrix(_mapData.gridWidth, _mapData.gridHeight, _mapData.baseType);
    }
    public void InitTeams()
    {
        foreach (Team team in teams) team.Init();
    }
    #endregion
    #region Map Matrix Methods    
    private void CreateEmptyMatrix(int dimX, int dimY, TileType myTileType)
    {
        MapMatrix = new List<List<Tile>>();
        for (int colIndex = 0; colIndex < dimX; colIndex++)
        {
            MapMatrix.Add(new List<Tile>());
            for (int rowIndex = 0; rowIndex < dimY; rowIndex++)
            {
                Tile newTile = CreateTile(myTileType, new Vector2Int(colIndex, rowIndex), 0);
                MapMatrix[colIndex].Add(newTile);
            }
        }
    }      
    public bool IsOnMap(Vector2Int position)
    {
        if (position.x >= 0 && position.x < MapMatrix.Count && position.y >= 0 && position.y < MapMatrix[0].Count) return true;
        else return false;      
    }
    #endregion
    #region Tile Methods
    Tile CreateTile(TileType type, Vector2Int position, int rotation)
    {
        Tile newTile = Instantiate(Database.GetTilePrefab(type), new Vector3(position.x, tileHeight, position.y), Quaternion.Euler(new Vector3(0, rotation, 0)), tileParent).GetComponent<Tile>();
        newTile.name += " at X: " + position.x + " Y: " + position.y;
        newTile.position = position;
        GameObject fogOfWarGfx = Instantiate(Database.fogOfWarTilePrefab, new Vector3(position.x, tileHeight, position.y), Quaternion.Euler(new Vector3(0, rotation, 0)), newTile.transform);
        newTile.fogOfWarGfx = fogOfWarGfx;
        fogOfWarGfx.SetActive(false);
        return newTile;
    }
    public Tile GetTile(Vector2Int position)
    {
        return MapMatrix[position.x][position.y];
    }
    public void ChangeTile(TileType type, Vector2Int position, int rotation)
    {
        Destroy(MapMatrix[position.x][position.y].gameObject);
        MapMatrix[position.x][position.y] = CreateTile(type, position, rotation);
    }
    public void SetVisibility(Vector2Int position, bool value)
    {
        Tile tile = GetTile(position);
        tile.fogOfWarGfx.SetActive(!value);
        tile.isVisible = value;
        if(tile.unitStandingHere != null) tile.unitStandingHere.SetVisibility(value);      
    }
    public void SetNeighbors(List<List<Tile>> matrix)
    {
        for (int i = 0; i < matrix.Count; i++)
        {
            for (int j = 0; j < matrix[i].Count; j++)
            {
                matrix[i][j].SetNeighbors();
            }
        }
    }
    #endregion
    #region Unit Methods
    //Create a unit for the given team, position and rotation.
    public void CreateUnit(UnitType type, Team team, Vector2Int position, Direction facingDirection)
    {        
        //Create the Unit
        Unit unit = Instantiate(Core.Model.Database.GetUnitPrefab(type), new Vector3(position.x, tileHeight, position.y), Quaternion.Euler(0, 0, 0), team.transform).GetComponent<Unit>();
        unit.Init();
        //Position and rotation
        unit.RotateUnit(facingDirection);
        unit.position = position;       
        team.AddUnit(unit);
        GetTile(position).SetUnitHere(unit);//Pass the unit to the tile it stands on
    }
    #endregion
    #region Team Methods
   
    //Create a team with a name and a color from the teamColors list and add it to the teams list.
    public void CreateTeam(string myTeamName)
    {
        Team team = new Team();
        team.name = myTeamName;
        teams.Add(team);
        //Create empty game object in wich we will store the units for the team later.
        team.transform.parent = this.transform;
    }
    //Used for creating premade teams    
    public void CreateTeam(Team team)
    {
        Team newTeam = Instantiate(team.gameObject, this.transform).GetComponent<Team>();
        teams.Add(newTeam);
    }

    //Searches for a team by a given name and returns it.
    public Team GetTeam(string teamName)
    {
        for (int i = 0; i < teams.Count; i++)
        {
            if (teamName == teams[i].name)
            {
                return teams[i];
            }
        }
        throw new System.Exception("No such team found!");
    } 

    public Team GetTeam(int index)
    {
        return teams[index];
    }
  
    #endregion
    #region Succession
    //Defines the order in wich the teams have their turns. (TODO: find a better way to solve this...)
    public void SetupRandomSuccession()
    {
        Succession = new List<Team>();
        List<Team> tempList = new List<Team>(Core.Model.teams);
        while (tempList.Count > 0)
        {
            int randomPick = Random.Range(0, tempList.Count);
            Succession.Add(tempList[randomPick]);
            tempList.Remove(tempList[randomPick]);
        }
    }
    //Gets the next team in the succesion.
    public Team GetNextTeamInSuccession() { return Succession[((Succession.IndexOf(Core.Controller.ActiveTeam) + 1) % Succession.Count)]; }
    public bool IsLastInSuccession(Team team)
    {
        if(Succession.IndexOf(team) == Succession.Count - 1) return true;
        else return false;
    }
    #endregion
    #region Hardcoded Maps
    public void LoadLevel01(int width, int height)
    {
        CreateEmptyMatrix(width, height, TileType.Plain);
        for (int i = 0; i < width; i++)
        {
            ChangeTile(TileType.Sea, new Vector2Int(i, 0), 0);
            ChangeTile(TileType.Sea, new Vector2Int(i, height - 1), 0);
            ChangeTile(TileType.Forest, new Vector2Int(i, 2), 0);
            ChangeTile(TileType.Mountain, new Vector2Int(i, 1), 0);
            ChangeTile(TileType.River, new Vector2Int(i, 9), 0);
            ChangeTile(TileType.Shoal, new Vector2Int(i, height - 2), 0);
        }
        ChangeTile(TileType.HQ, new Vector2Int(7, 7), 0);
        ChangeTile(TileType.Facility, new Vector2Int(7, 8), 0);
        Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(7, 8)));
        ChangeTile(TileType.Road, new Vector2Int(7, 6), 0);
        SetNeighbors(MapMatrix);
        CreateUnit(UnitType.Tank, Core.Model.teams[0], new Vector2Int(5, 5), Direction.North);
        CreateUnit(UnitType.Tank, Core.Model.teams[1], new Vector2Int(6, 5), Direction.West);
    }

    #endregion
}
