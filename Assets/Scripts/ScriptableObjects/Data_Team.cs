using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Data Sheets/Team")]
public class Data_Team : ScriptableObject
{
    #region Basic Fields
    public string teamName;
    public CommanderType commander;
    public Color teamColor;
    public List<UnitType> availableUnits = new List<UnitType>();

    #endregion
    #region Statistic Fields
    List<int> _unitsBuilt = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//Units this team hast built during the game.
    int _totalMoney = 0;//Counts the overall earned money.
    int _unitsBuiltCounter = 0;//Counts the overall created units.
    int _unitsKilledCounter = 0;//Counts how many unit this team destroyed.  
    #endregion

    #region Available Units
    //Add an unit type to the available units list.
    public void AddAvailableUnit(UnitType myUnittype)
    {
        if (!availableUnits.Contains(myUnittype))
        {
            availableUnits.Add(myUnittype);
        }
        else
        {
            throw new System.Exception(myUnittype + ": Unit is already available!");
        }
    }

    //Remove available unit.
    public void RemoveAvailableUnit(UnitType myUnittype)
    {
        if (availableUnits.Contains(myUnittype))
        {
            availableUnits.Remove(myUnittype);
        }
        else
        {
            throw new System.Exception(myUnittype + ": Unit was not available!");
        }
    }

    //Set the list of available units for a team.
    public void SetAvailableUnits(List<UnitType> newUnitList)
    {
        availableUnits.Clear();
        availableUnits = newUnitList;
    }

    //Makes all possible units available.
    public void SetAllUnitsAvailable()
    {
        availableUnits.Add(UnitType.Flak);
        availableUnits.Add(UnitType.APC);
        availableUnits.Add(UnitType.Artillery);
        availableUnits.Add(UnitType.Battleship);
        availableUnits.Add(UnitType.BCopter);
        availableUnits.Add(UnitType.Bomber);
        availableUnits.Add(UnitType.Cruiser);
        availableUnits.Add(UnitType.Fighter);
        availableUnits.Add(UnitType.Infantry);
        availableUnits.Add(UnitType.Lander);
        availableUnits.Add(UnitType.MdTank);
        availableUnits.Add(UnitType.Mech);
        availableUnits.Add(UnitType.Missiles);
        availableUnits.Add(UnitType.Titantank);
        availableUnits.Add(UnitType.Recon);
        availableUnits.Add(UnitType.Rockets);
        availableUnits.Add(UnitType.Sub);
        availableUnits.Add(UnitType.Tank);
        availableUnits.Add(UnitType.TCopter);
    }

    public List<UnitType> GetAvailableGroundUnits()
    {
        List<UnitType> tempList = new List<UnitType>();
        if (availableUnits.Contains(UnitType.Flak)) tempList.Add(UnitType.Flak);
        if (availableUnits.Contains(UnitType.APC)) tempList.Add(UnitType.APC);
        if (availableUnits.Contains(UnitType.Artillery)) tempList.Add(UnitType.Artillery);
        if (availableUnits.Contains(UnitType.Infantry)) tempList.Add(UnitType.Infantry);
        if (availableUnits.Contains(UnitType.MdTank)) tempList.Add(UnitType.MdTank);
        if (availableUnits.Contains(UnitType.Mech)) tempList.Add(UnitType.Mech);
        if (availableUnits.Contains(UnitType.Missiles)) tempList.Add(UnitType.Missiles);
        if (availableUnits.Contains(UnitType.Titantank)) tempList.Add(UnitType.Titantank);
        if (availableUnits.Contains(UnitType.Recon)) tempList.Add(UnitType.Recon);
        if (availableUnits.Contains(UnitType.Rockets)) tempList.Add(UnitType.Rockets);
        if (availableUnits.Contains(UnitType.Tank)) tempList.Add(UnitType.Tank);
        return tempList;
    }
    public List<UnitType> GetAvailableAirUnits()
    {
        List<UnitType> tempList = new List<UnitType>();
        if (availableUnits.Contains(UnitType.BCopter)) tempList.Add(UnitType.BCopter);
        if (availableUnits.Contains(UnitType.TCopter)) tempList.Add(UnitType.TCopter);
        if (availableUnits.Contains(UnitType.Bomber)) tempList.Add(UnitType.Bomber);
        if (availableUnits.Contains(UnitType.Fighter)) tempList.Add(UnitType.Fighter);
        return tempList;
    }
    public List<UnitType> GetAvailableNavalUnits()
    {
        List<UnitType> tempList = new List<UnitType>();
        if (availableUnits.Contains(UnitType.Lander)) tempList.Add(UnitType.Lander);
        if (availableUnits.Contains(UnitType.Cruiser)) tempList.Add(UnitType.Cruiser);
        if (availableUnits.Contains(UnitType.Battleship)) tempList.Add(UnitType.Battleship);
        if (availableUnits.Contains(UnitType.Sub)) tempList.Add(UnitType.Sub);
        return tempList;
    }
    #endregion
    #region Statistics
    public void IncTotalMoney(int amount){_totalMoney += amount;}
    public int GetTotalMoney() { return _totalMoney; }

    public void IncUnitsKilledCount() { _unitsKilledCounter++; }
    public int GetUnitsKilledCount() { return _unitsKilledCounter; }

    public int GetUnitsBuiltCounter() { return _unitsBuiltCounter; }
    public List<int> GetUnitsBuilt() { return _unitsBuilt; }
    public void IncUnitsBuilt(UnitType myType)
    {
        switch (myType)
        {
            case UnitType.Flak: IncUnitBuiltCounter(0); break;
            case UnitType.APC: IncUnitBuiltCounter(1); break;
            case UnitType.Tank: IncUnitBuiltCounter(2); break;
            case UnitType.Artillery: IncUnitBuiltCounter(3); break;
            case UnitType.Rockets: IncUnitBuiltCounter(4); break;
            case UnitType.Missiles: IncUnitBuiltCounter(5); break;
            case UnitType.Titantank: IncUnitBuiltCounter(6); break;
            case UnitType.Recon: IncUnitBuiltCounter(7); break;
            case UnitType.Infantry: IncUnitBuiltCounter(8); break;
            case UnitType.MdTank: IncUnitBuiltCounter(9); break;
            case UnitType.Mech: IncUnitBuiltCounter(10); break;
            case UnitType.TCopter: IncUnitBuiltCounter(11); break;
            case UnitType.BCopter: IncUnitBuiltCounter(12); break;
            case UnitType.Bomber: IncUnitBuiltCounter(13); break;
            case UnitType.Fighter: IncUnitBuiltCounter(14); break;
            case UnitType.Lander: IncUnitBuiltCounter(15); break;
            case UnitType.Battleship: IncUnitBuiltCounter(16); break;
            case UnitType.Cruiser: IncUnitBuiltCounter(17); break;
            case UnitType.Sub: IncUnitBuiltCounter(18); break;
            case UnitType.Pipe: throw new System.Exception("Pipes cannot be build!");
            default: throw new System.Exception("No such unittype found!");
        }
    }
    void IncUnitBuiltCounter(int index)
    {
        _unitsBuilt[index]++;
        _unitsBuiltCounter++;
    }
    public int GetUnitsBuiltCount(UnitType myType)
    {
        switch (myType)
        {
            case UnitType.Flak: return _unitsBuilt[0];
            case UnitType.APC: return _unitsBuilt[1];
            case UnitType.Tank: return _unitsBuilt[2];
            case UnitType.Artillery: return _unitsBuilt[3];
            case UnitType.Rockets: return _unitsBuilt[4];
            case UnitType.Missiles: return _unitsBuilt[5];
            case UnitType.Titantank: return _unitsBuilt[6];
            case UnitType.Recon: return _unitsBuilt[7];
            case UnitType.Infantry: return _unitsBuilt[8];
            case UnitType.MdTank: return _unitsBuilt[9];
            case UnitType.Mech: return _unitsBuilt[10];
            case UnitType.TCopter: return _unitsBuilt[11];
            case UnitType.BCopter: return _unitsBuilt[12];
            case UnitType.Bomber: return _unitsBuilt[13];
            case UnitType.Fighter: return _unitsBuilt[14];
            case UnitType.Lander: return _unitsBuilt[15];
            case UnitType.Battleship: return _unitsBuilt[16];
            case UnitType.Cruiser: return _unitsBuilt[17];
            case UnitType.Sub: return _unitsBuilt[18];
            case UnitType.Pipe: throw new System.Exception("Pipes cannot be build!");
            default: throw new System.Exception("No such unittype found!");
        }
    }
    #endregion
}
