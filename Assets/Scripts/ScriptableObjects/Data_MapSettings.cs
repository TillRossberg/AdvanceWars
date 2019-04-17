using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/Settings")]
public class Data_MapSettings : ScriptableObject
{
    public bool randomWeather;
    public bool fogOfWar;
    public bool abilityPower;//Are the abilities for the players activated?
    public int moneyIncrement = 1000;//The money you get per round per building.
    public int startMoney = 10000;
    public float battleDuration = 4;//Duration of the battle in days, 4 means no limited duration.
    public float propertiesToWin = 11;//Amount of properties one player needs to win the game, 11 turns this winning condition off.
}
