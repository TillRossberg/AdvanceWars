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
    public Color color;
    public List<UnitType> availableGroundUnits;
    public List<UnitType> availableAirUnits;
    public List<UnitType> availableNavalUnits;


    #endregion
    #region Statistic Fields
    List<int> _unitsBuilt = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//Units this team hast built during the game.
    int _totalMoney = 0;//Counts the overall earned money.
    int _unitsBuiltCounter = 0;//Counts the overall created units.
    int _unitsKilledCounter = 0;//Counts how many unit this team destroyed.  
    #endregion
    #region Basic Methods
    private void OnEnable()
    {
        _unitsBuilt = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }
    #endregion
    #region Available Units
    //Add an unit type to the available units list.
    public void AddAvailableUnit(UnitType myUnittype, UnitCategory category)
    {
        switch (category)
        {
            case UnitCategory.ground: AddAvailableUnit(myUnittype, availableGroundUnits);break;
            case UnitCategory.air: AddAvailableUnit(myUnittype, availableAirUnits);break;
            case UnitCategory.naval: AddAvailableUnit(myUnittype, availableNavalUnits);break;
            default:
                break;
        }
        
    }
    void AddAvailableUnit(UnitType type, List<UnitType> typeList)
    {
        if (!typeList.Contains(type)) typeList.Add(type);
        else throw new System.Exception(type + ": Unit is already available!");       
    }

    //Remove available unit.
    public void RemoveAvailableUnit(UnitType myUnittype, UnitCategory category)
    {
        switch (category)
        {
            case UnitCategory.ground: RemoveAvailableUnit(myUnittype, availableGroundUnits);break;
            case UnitCategory.air:RemoveAvailableUnit(myUnittype, availableAirUnits);break;
            case UnitCategory.naval:RemoveAvailableUnit(myUnittype, availableNavalUnits);break;
            default:
                break;
        }
    }
    void RemoveAvailableUnit(UnitType myUnittype, List<UnitType> typeList)
    {
        if (typeList.Contains(myUnittype))typeList.Remove(myUnittype);
        else throw new System.Exception(myUnittype + ": Unit was not available!");
    }

    //Makes all possible units available.
    public void SetAllUnitsAvailable()
    {
        //ground
        availableGroundUnits.Add(UnitType.AntiAir);
        availableGroundUnits.Add(UnitType.APC);
        availableGroundUnits.Add(UnitType.Artillery);
        availableGroundUnits.Add(UnitType.Infantry);
        availableGroundUnits.Add(UnitType.MdTank);
        availableGroundUnits.Add(UnitType.Mech);
        availableGroundUnits.Add(UnitType.Missiles);
        availableGroundUnits.Add(UnitType.Titantank);
        availableGroundUnits.Add(UnitType.Recon);
        availableGroundUnits.Add(UnitType.Rockets);
        availableGroundUnits.Add(UnitType.Tank);
        //air
        availableAirUnits.Add(UnitType.Fighter);
        availableAirUnits.Add(UnitType.TCopter);
        availableAirUnits.Add(UnitType.BCopter);
        availableAirUnits.Add(UnitType.Bomber);
        //naval
        availableNavalUnits.Add(UnitType.Battleship);
        availableNavalUnits.Add(UnitType.Cruiser);
        availableNavalUnits.Add(UnitType.Lander);
        availableNavalUnits.Add(UnitType.Sub);
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
            case UnitType.AntiAir: IncUnitBuiltCounter(0); break;
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
            case UnitType.AntiAir: return _unitsBuilt[0];
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
