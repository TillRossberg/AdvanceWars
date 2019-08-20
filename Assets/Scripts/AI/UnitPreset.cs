using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/AI Unit Preset")]
public class UnitPreset : ScriptableObject
{
    public UnitPresetType Type;
    public Squad.Tactic StartTactic;
    public List<UnitType> Types;
    public int Priority;
}
