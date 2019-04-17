using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu (menuName = "Scriptable Objects/Map")]
public class Data_Map : ScriptableObject
{
    public int gridHeight;
    public int gridWidth;
    public TileType baseType;
    public Weather startWeather;
    public Team winner;
    public List<Team> teams;


    

    

}
