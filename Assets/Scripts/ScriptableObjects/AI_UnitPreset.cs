using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/AI Unit Preset")]
public class AI_UnitPreset : ScriptableObject
{    
    public List<UnitType> Types;        
}
