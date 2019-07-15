using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Property : MonoBehaviour
{
    public TextMeshPro TakeOverPoints;
    public Team OwningTeam;

    public int TakeOverCounter;
    int _maxTakeOverPoints;

    public void Init(int maxTakeOverPoints)
    {
        _maxTakeOverPoints = maxTakeOverPoints;
    }
    public void ResetTakeOverCounter()
    {
        TakeOverCounter = _maxTakeOverPoints;
    }
}
