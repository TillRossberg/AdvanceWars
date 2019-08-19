using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/AI Unit Preset")]
public class AI_UnitPreset : ScriptableObject
{
    public AI_Squad.Tactic StartTactic;
    public List<UnitType> Types;
    public int Priority;
}
