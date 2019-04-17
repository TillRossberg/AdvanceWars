using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu (menuName ="Data Sheets/Tile")]
public class Data_Tile : ScriptableObject
{
    [Header("General")]
    public TileType type;
    public string tileName;
    public int cover;
    public bool isProperty = false;
    public int maxTakeOverPoints = 20;
    public Sprite thumbNail;
    [Header("Graphics")]
    public List<Mesh> meshes;
    public List<Material> materials;
    [Header("Movement Cost")]    
    public int footCost;
    public int mechCost;
    public int treadsCost;
    public int wheelsCost;
    public int landerCost;
    public int shipCost;
    public int airCost;

    #region Getter
    public int GetMovementCost(UnitMoveType type)
    {
        switch (type)
        {
            case UnitMoveType.Foot: return footCost;
            case UnitMoveType.Mech: return mechCost;
            case UnitMoveType.Treads: return treadsCost;
            case UnitMoveType.Wheels: return wheelsCost;
            case UnitMoveType.Lander: return landerCost;
            case UnitMoveType.Ship: return shipCost;
            case UnitMoveType.Air: return airCost;
            default: throw new System.Exception("Move type not found!");
        }
    }
    #endregion
}
