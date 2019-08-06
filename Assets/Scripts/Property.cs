using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class Property : MonoBehaviour
{
    public TextMeshPro TakeOverPointsText;
    public Image TakeOverPointsBar;
    public Team OwningTeam;
    public float TextAnimationSpeed = 0.2f;
    public float BarAnimationSpeed = 3;
    
    float _targetFillAmount;
    int _currentTakeOverPoints;
    int _targetTakeOverPoints;
    int _maxTakeOverPoints;
    bool _animationRunning = false;
    Unit _occupyingUnit;
    public event Action OnAnimationFinished;

    public void Init(int maxTakeOverPoints)
    {
       _currentTakeOverPoints = _maxTakeOverPoints = maxTakeOverPoints;
    }
    public void Reset()
    {
        _currentTakeOverPoints = _maxTakeOverPoints;
        TakeOverPointsBar.fillAmount = 1.0f;
        TakeOverPointsText.text = _maxTakeOverPoints.ToString();
        _occupyingUnit = null;
        _animationRunning = false;
    }

    public void DecreaseTakeOverPoints(Unit unit)
    {
        _occupyingUnit = unit;
        ShowTakeOverGfx(true);
        _targetTakeOverPoints = _currentTakeOverPoints - unit.GetCorrectedHealth();
        if (_targetTakeOverPoints < 0) _targetTakeOverPoints = 0;
        _targetFillAmount = (float)_targetTakeOverPoints /_maxTakeOverPoints;
        _animationRunning = true;
    }
    public int GetTakeOverPoints()
    {
        return _currentTakeOverPoints;
    }
    private void Update()
    {
        if(_animationRunning)
        {
            TakeOverPointsBar.fillAmount = Mathf.Lerp(TakeOverPointsBar.fillAmount, _targetFillAmount, BarAnimationSpeed * Time.deltaTime);
            if (TakeOverPointsBar.fillAmount < _targetFillAmount + 0.001f) TakeOverPointsBar.fillAmount = _targetFillAmount;

            _currentTakeOverPoints = (int) Mathf.Lerp(_currentTakeOverPoints, _targetTakeOverPoints, TextAnimationSpeed * Time.deltaTime);
            TakeOverPointsText.text = _currentTakeOverPoints.ToString();
            if (TakeOverPointsBar.fillAmount == _targetFillAmount && _currentTakeOverPoints == _targetTakeOverPoints)
            {
                if (_currentTakeOverPoints <= 0)
                {
                    Core.Controller.Occupy(_occupyingUnit.team, this.GetComponent<Tile>());
                    Reset();
                }
                ShowTakeOverGfx(false);
                OnAnimationFinished();
                _animationRunning = false;
            }
        }
    }
    void ShowTakeOverGfx(bool value)
    {
        TakeOverPointsText.gameObject.SetActive(value);
        TakeOverPointsBar.gameObject.SetActive(value);
    }

}
