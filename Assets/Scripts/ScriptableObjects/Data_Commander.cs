using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Data Sheets/Commander")]
public class Data_Commander : ScriptableObject
{
    public CommanderType type;
    public string commanderName;
    public Sprite thumbNail;

}
