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
    public Calculations_Battle BattleCalculations { get; private set; }
    public AStar AStar { get; private set; }
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
        BattleCalculations = new Calculations_Battle();
        AStar = new AStar();
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
    public List<Tile> GetProperties()
    {
        List<Tile> tempList = new List<Tile>();
        for (int x = 0; x < MapMatrix.Count; x++)
        {
            for (int y = 0; y < MapMatrix[x].Count; y++)
            {
                Tile tile = MapMatrix[x][y];
                if (tile.IsProperty()) tempList.Add(tile);
            }
        }
        return tempList;
    }
    #endregion
    #region Tile Methods
    Tile CreateTile(TileType type, Vector2Int position, int rotation)
    {
        Tile newTile = Instantiate(Database.GetTilePrefab(type), new Vector3(position.x, tileHeight, position.y), Quaternion.Euler(new Vector3(0, rotation, 0)), tileParent).GetComponent<Tile>();
        newTile.name += " at X: " + position.x + " Y: " + position.y;
        newTile.Position = position;
        GameObject fogOfWarGfx = Instantiate(Database.fogOfWarTilePrefab, new Vector3(position.x, tileHeight, position.y), Quaternion.Euler(new Vector3(0, rotation, 0)), newTile.transform);
        newTile.fogOfWarGfx = fogOfWarGfx;
        fogOfWarGfx.SetActive(false);
        newTile.Init();
        return newTile;
    }
    public Tile GetTile(Vector2Int position)
    {
        if (IsOnMap(position)) return MapMatrix[position.x][position.y];
        else return null;
    }
    
    public Tile GetTile(int x, int y)
    {
        return GetTile(new Vector2Int(x, y));
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
        tile.IsVisible = value;
        if (tile.UnitHere != null) tile.UnitHere.SetVisibility(value);
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
    
    public List<Tile> GetTilesInRadius(Tile center, int radius)
    {
        List<Tile> tempList = new List<Tile>();
        int centerX = center.Position.x;
        int centerY = center.Position.y;

        for (int i = 1; i <= radius; i++)
        {
            Tile up = Core.Model.GetTile(centerX, centerY + i);
            Tile down = Core.Model.GetTile(centerX, centerY - i);
            Tile right = Core.Model.GetTile(centerX + i, centerY);
            Tile left = Core.Model.GetTile(centerX - i, centerY);

            Tile upRight = Core.Model.GetTile(centerX + i, centerY + i);
            Tile upLeft = Core.Model.GetTile(centerX - i, centerY + i);
            Tile downRight = Core.Model.GetTile(centerX + i, centerY - i);
            Tile downLeft = Core.Model.GetTile(centerX - i, centerY - i);

            TryToAdd(up, tempList);
            TryToAdd(down, tempList);
            TryToAdd(right, tempList);
            TryToAdd(left, tempList);

            TryToAdd(upRight, tempList);
            TryToAdd(upLeft, tempList);
            TryToAdd(downRight, tempList);
            TryToAdd(downLeft, tempList);
            if (i > 1)
            {
                for (int j = 1; j < i; j++)
                {
                    if (left != null)
                    {
                        Tile aboveLeft = Core.Model.GetTile(left.Position.x, left.Position.y + j);
                        Tile belowLeft = Core.Model.GetTile(left.Position.x, left.Position.y - j);
                        TryToAdd(aboveLeft, tempList);
                        TryToAdd(belowLeft, tempList);

                    }
                    if (right != null)
                    {
                        Tile aboveRight = Core.Model.GetTile(right.Position.x, right.Position.y + j);
                        Tile belowRight = Core.Model.GetTile(right.Position.x, right.Position.y - j);
                        TryToAdd(aboveRight, tempList);
                        TryToAdd(belowRight, tempList);
                    }
                    if (up != null)
                    {
                        Tile rightUp = Core.Model.GetTile(up.Position.x + j, up.Position.y);
                        Tile leftUp = Core.Model.GetTile(up.Position.x - j, up.Position.y);
                        TryToAdd(rightUp, tempList);
                        TryToAdd(leftUp, tempList);

                    }
                    if (down != null)
                    {
                        Tile leftRight = Core.Model.GetTile(down.Position.x + j, down.Position.y);
                        Tile leftDown = Core.Model.GetTile(down.Position.x - j, down.Position.y);
                        TryToAdd(leftRight, tempList);
                        TryToAdd(leftDown, tempList);
                    }
                }
            }
        }
        return tempList;
    }
    void TryToAdd(Tile tile, List<Tile> tiles)
    {
        if (tile != null) tiles.Add(tile);
    }
    #endregion
    #region Unit Methods
    //Create a unit for the given team, position and rotation.
    public Unit CreateUnit(UnitType type, Vector2Int position, Direction facingDirection)
    {
        //Create the Unit
        Unit unit = Instantiate(Core.Model.Database.GetUnitPrefab(type), new Vector3(position.x, tileHeight, position.y), Quaternion.Euler(0, 0, 0)).GetComponent<Unit>();
        unit.Init();
        //Position and rotation
        unit.RotateUnit(facingDirection);
        unit.Position = position;
        unit.CurrentTile = GetTile(position);
        GetTile(position).UnitHere = unit;//Pass the unit to the tile it stands on
        return unit;
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
        if (Succession.IndexOf(team) == Succession.Count - 1) return true;
        else return false;
    }
    #endregion
    #region Hardcoded Maps
    public void LoadLevel01(int width, int height)
    {
        CreateEmptyMatrix(width, height, TileType.Plain);
        for (int i = 0; i < width; i++)
        {
            //ChangeTile(TileType.Sea, new Vector2Int(i, 0), 0);
            //ChangeTile(TileType.Sea, new Vector2Int(i, height - 1), 0);
            //ChangeTile(TileType.Forest, new Vector2Int(i, 2), 0);
            //ChangeTile(TileType.Mountain, new Vector2Int(i, 1), 0);
            ChangeTile(TileType.River, new Vector2Int(i, 9), 0);
            ChangeTile(TileType.Shoal, new Vector2Int(i, height - 2), 0);
        }
        ChangeTile(TileType.HQ, new Vector2Int(7, 7), 0);
        ChangeTile(TileType.Facility, new Vector2Int(7, 8), 0);
        Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(7, 8)));
        ChangeTile(TileType.Facility, new Vector2Int(8, 8), 0);
        Core.Controller.Occupy(teams[1], GetTile(new Vector2Int(8, 8)));
        ChangeTile(TileType.Road, new Vector2Int(7, 6), 0);
        SetNeighbors(MapMatrix);
        
    }
    public void LoadLevel02(int width, int height)
    {
        CreateEmptyMatrix(width, height, TileType.Plain);
        for (int i = 0; i < 5; i++) DrawX(TileType.Sea, 0, 8 + i, width);
        DrawY(TileType.Forest, 0, 0, 8);
        DrawY(TileType.Forest, 18, 0, 8);
        DrawX(TileType.Forest, 0, 0, 18);
        DrawY(TileType.River, 9, 0, 8);
        //Roads
        DrawX(TileType.Road, 6, 3, 7);
        DrawX(TileType.Road, 6, 5, 7);
        DrawX(TileType.Road, 3, 4, 3);
        DrawX(TileType.Road, 13, 4, 3);
        ChangeTile(TileType.Road, new Vector2Int(5, 3), 90);
        GetTile(new Vector2Int(5, 3)).SetMaterial(1);
        ChangeTile(TileType.Road, new Vector2Int(13, 3), 0);
        GetTile(new Vector2Int(13, 3)).SetMaterial(1);
        ChangeTile(TileType.Road, new Vector2Int(5, 5), 180);
        GetTile(new Vector2Int(5, 5)).SetMaterial(1);
        ChangeTile(TileType.Road, new Vector2Int(13, 5), 270);
        GetTile(new Vector2Int(13, 5)).SetMaterial(1);


        //Properties
        //Red
        ChangeTile(TileType.HQ, new Vector2Int(2, 4), 0);
        ChangeTile(TileType.Airport, new Vector2Int(2, 6), 0);
        ChangeTile(TileType.City, new Vector2Int(2, 2), 0);
        ChangeTile(TileType.Facility, new Vector2Int(6, 2), 0);
        ChangeTile(TileType.Facility, new Vector2Int(6, 4), 0);
        ChangeTile(TileType.Facility, new Vector2Int(6, 6), 0);
        ChangeTile(TileType.Port, new Vector2Int(4, 7), 0);
        Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(2, 4)));
        Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(2, 6)));
        //Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(2, 2)));
        Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(6, 2)));
        Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(6, 4)));
        Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(6, 6)));
        Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(4, 7)));
        //Blue
        ChangeTile(TileType.HQ, new Vector2Int(16, 4), 0);
        ChangeTile(TileType.Airport, new Vector2Int(16, 2), 0);
        ChangeTile(TileType.City, new Vector2Int(16, 6), 0);
        ChangeTile(TileType.Facility, new Vector2Int(12, 2), 0);
        ChangeTile(TileType.Facility, new Vector2Int(12, 4), 0);
        ChangeTile(TileType.Facility, new Vector2Int(12, 6), 0);
        ChangeTile(TileType.Port, new Vector2Int(14, 7), 0);
        Core.Controller.Occupy(teams[1], GetTile(new Vector2Int(16, 4)));
        Core.Controller.Occupy(teams[1], GetTile(new Vector2Int(16, 6)));
        Core.Controller.Occupy(teams[1], GetTile(new Vector2Int(16, 2)));
        Core.Controller.Occupy(teams[1], GetTile(new Vector2Int(12, 2)));
        Core.Controller.Occupy(teams[1], GetTile(new Vector2Int(12, 4)));
        Core.Controller.Occupy(teams[1], GetTile(new Vector2Int(12, 6)));
        Core.Controller.Occupy(teams[1], GetTile(new Vector2Int(14, 7)));

        SetNeighbors(MapMatrix);
    }
    public void LoadLevel03(int width, int height)
    {
        CreateEmptyMatrix(width, height, TileType.Plain);
        DrawY(TileType.River, 3, 0, 2);
        DrawY(TileType.River, 3, 3, 2);
        DrawY(TileType.Mountain, 1, 1, 3);
        DrawY(TileType.Mountain, 5, 1, 3);

        ChangeTile(TileType.HQ, new Vector2Int(0, 2), 0);
        ChangeTile(TileType.HQ, new Vector2Int(6, 2), 0);

        Core.Controller.Occupy(teams[0], GetTile(new Vector2Int(0, 2)));
        Core.Controller.Occupy(teams[1], GetTile(new Vector2Int(6, 2)));

        CreateUnit(UnitType.Tank,  new Vector2Int(2, 2), Direction.East);
        CreateUnit(UnitType.Tank,  new Vector2Int(4, 1), Direction.West);

        //ChangeTile(TileType.Mountain, new Vector2Int(5,0), 0);
        SetNeighbors(MapMatrix);
    }

    public void LoadLevel02Units()
    {
        //Red
        //ground
        //teams[0].AddUnit(CreateUnit(UnitType.Infantry, new Vector2Int(1, 1), Direction.South));
        //teams[0].AddUnit(CreateUnit(UnitType.Mech, new Vector2Int(2, 1), Direction.South));
        //teams[0].AddUnit(CreateUnit(UnitType.Recon, new Vector2Int(3, 1), Direction.South));
        //teams[0].AddUnit(CreateUnit(UnitType.Tank, new Vector2Int(4, 1), Direction.South));
        //teams[0].AddUnit(CreateUnit(UnitType.MdTank, new Vector2Int(5, 1), Direction.South));
        //teams[0].AddUnit(CreateUnit(UnitType.Titantank, new Vector2Int(7, 1), Direction.South));
        //teams[0].AddUnit(CreateUnit(UnitType.APC, new Vector2Int(6, 1), Direction.South)); 
        //teams[0].AddUnit(CreateUnit(UnitType.Artillery, new Vector2Int(3, 2), Direction.South)); 
        //teams[0].AddUnit(CreateUnit(UnitType.Rockets, new Vector2Int(4, 2), Direction.South));
        //teams[0].AddUnit(CreateUnit(UnitType.AntiAir, new Vector2Int(5, 2), Direction.South)); 
        //teams[0].AddUnit(CreateUnit(UnitType.Missiles, new Vector2Int(6, 2), Direction.South));
        //air
        //teams[0].AddUnit(CreateUnit(UnitType.TCopter, new Vector2Int(9, 1), Direction.East));
        //teams[0].AddUnit(CreateUnit(UnitType.BCopter, new Vector2Int(9, 3), Direction.East));
        //teams[0].AddUnit(CreateUnit(UnitType.Fighter, new Vector2Int(9, 5), Direction.East));
        //teams[0].AddUnit(CreateUnit(UnitType.Bomber, new Vector2Int(9, 7), Direction.East));
        //naval
        teams[0].AddUnit(CreateUnit(UnitType.Lander, new Vector2Int(3, 9), Direction.South));
        teams[0].AddUnit(CreateUnit(UnitType.Sub, new Vector2Int(5, 9), Direction.South));
        teams[0].AddUnit(CreateUnit(UnitType.Cruiser, new Vector2Int(7, 9), Direction.South));
        teams[0].AddUnit(CreateUnit(UnitType.Battleship, new Vector2Int(9, 9), Direction.South));
        //SetUnitTypeHealth(Core.Model.teams[0], UnitType.Battleship, 25);

        //Blue
        //teams[1].AddUnit(CreateUnit(UnitType.Infantry, new Vector2Int(6, 4), Direction.North));
        teams[1].AddUnit(CreateUnit(UnitType.Infantry, new Vector2Int(14, 4), Direction.North));
        SetUnitTypeHealth(Core.Model.teams[1], UnitType.Infantry, 40);
    }
    void DrawX(TileType type, int startX, int startY, int length)
    {
        for (int i = 0; i < length; i++)
        {
            ChangeTile(type, new Vector2Int(startX + i, startY), 0);
        }
    }
    void DrawY(TileType type, int startX, int startY, int length)
    {
        for (int i = 0; i < length; i++)
        {
            ChangeTile(type, new Vector2Int(startX, startY + i), 0);
        }
    }
    #endregion
    #region Debug
    void SetUnitTypeHealth(Team team, UnitType type, int amount)
    {
        List<Unit> units = team.GetAllUnitsOfType(type);
        foreach (Unit item in units)item.SetHealth(amount);       
    }
    #endregion
}
