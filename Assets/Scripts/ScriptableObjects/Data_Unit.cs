using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu (menuName = "Data Sheets/Unit")]
public class Data_Unit : ScriptableObject
{
    #region Prefabs    
    public Sprite thumbNail;
    public Mesh mesh;
    public Material material;

    #endregion
    #region Fields
    [Header("General")]
    public UnitType type;
    public UnitMoveType moveType;
    public string description;
    public bool directAttack;
    public bool rangeAttack;
    [Header("Properties")]
    public int maxAmmo;
    public int maxFuel;
    public int moveDist;
    public int visionRange;
    public int minRange;
    public int maxRange;
    public int cost;
    #endregion
    #region Damage
    [Header("Damage")]
    public float Infantry;
    public float Mech;
    public float Flak;
    public float APC;
    public float Tank;
    public float Artillery;
    public float Rockets;
    public float Missiles;
    public float Titantank;
    public float Recon;
    public float MdTank;
    public float TCopter;
    public float BCopter;
    public float Bomber;
    public float Fighter;
    public float Lander;
    public float Battleship;
    public float Cruiser;
    public float Sub;
    public float Pipe;

    public float GetDamageAgainst(UnitType type)
    {
        switch (type)
        {
            case UnitType.Flak: return Flak;
            case UnitType.APC: return APC;               
            case UnitType.Tank: return Tank;
            case UnitType.Artillery: return Artillery;
            case UnitType.Rockets: return Rockets;
            case UnitType.Missiles: return Missiles;
            case UnitType.Titantank: return Titantank;
            case UnitType.Recon: return Recon;
            case UnitType.Infantry: return Infantry;
            case UnitType.MdTank: return MdTank;
            case UnitType.Mech: return Mech;
            case UnitType.TCopter: return TCopter;
            case UnitType.BCopter: return BCopter;
            case UnitType.Bomber: return Bomber;
            case UnitType.Fighter: return Fighter;
            case UnitType.Lander: return Lander;
            case UnitType.Battleship: return Battleship;
            case UnitType.Cruiser: return Cruiser;
            case UnitType.Sub: return Sub;
            case UnitType.Pipe: return Pipe;
            default: throw new System.Exception("Unittype not found!");
        }
    }
    #endregion
}
