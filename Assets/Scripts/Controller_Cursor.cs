﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class Controller_Cursor : MonoBehaviour
{
    bool _inputBlocked = true;
    float _inputDelay;
    bool _horAxisInUse;
    bool _vertAxisInUse;
    bool _buttonPressed;
    public GameObject gfx;
    public TextMeshPro estimatedDamage;
    public Vector2Int Position { get; private set; }
    public List<Mesh> meshes;

    float _buttonHoldDelay;
    float _buttonPressedTimer = 0;
    bool _holdingB = false;

    public void Init(Vector2Int position)
    {
        SetPosition(position);
        _inputDelay = Core.Model.inputDelay;
        _buttonHoldDelay = Core.Model.buttonHoldDelay;
    }

    private void Update()
    {
        if (!_inputBlocked)
        {
            #region Movement
            //Up
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                if (!_vertAxisInUse)
                {
                    _vertAxisInUse = true;
                    Core.Controller.GoTo(new Vector2Int(Position.x, Position.y + 1));
                    StartCoroutine(ResetVerticalAxisInUseDelayed(_inputDelay));
                }
            }
            else
            //Down
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                if (!_vertAxisInUse)
                {
                    _vertAxisInUse = true;
                    Core.Controller.GoTo(new Vector2Int(Position.x, Position.y - 1));
                    StartCoroutine(ResetVerticalAxisInUseDelayed(_inputDelay));
                }
            }
            //Left
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                if (!_horAxisInUse)
                {
                    _horAxisInUse = true;
                    Core.Controller.GoTo(new Vector2Int(Position.x - 1, Position.y));
                    StartCoroutine(ResetHorizontalAxisInUseDelayed(_inputDelay));
                }
            }
            else
            //Right
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                if (!_horAxisInUse)
                {
                    _horAxisInUse = true;
                    Core.Controller.GoTo(new Vector2Int(Position.x + 1, Position.y));
                    StartCoroutine(ResetHorizontalAxisInUseDelayed(_inputDelay));
                }
            }

            #endregion
            #region Buttons
            if (Input.GetButtonDown("Jump"))
            {
                if (!_buttonPressed)
                {
                    Core.Controller.AButton();
                    _buttonPressed = true;
                    StartCoroutine(ResetButtonPressedDelayed(_inputDelay));
                }
            }
            if (Input.GetButtonDown("Cancel"))
            {
                Core.Controller.BButton();
                _buttonPressedTimer = 0;
            }
            //Holding B button
            if (Input.GetButton("Cancel"))
            {
                _buttonPressedTimer += Time.deltaTime;
                if (_buttonPressedTimer >= _buttonHoldDelay && !_holdingB)
                {
                    Core.Controller.BButtonHold();
                    _holdingB = true;
                }
            }
            if (Input.GetButtonUp("Cancel"))
            {
                if (_buttonPressedTimer >= _buttonHoldDelay)
                {
                    Core.Controller.BButtonReleased();
                    _buttonPressedTimer = 0;
                    _holdingB = false;
                }
            }
            if(Input.GetButtonDown("RB"))
            {
                Core.Controller.RButton();
            }
            if (Input.GetButtonDown("LB"))
            {
                Core.Controller.LButton();
            }
            #endregion
        }

        #region Debug
        if (Input.GetKeyDown(KeyCode.N))
        {
            Tile start = Core.Model.GetTile(new Vector2Int(0, 0));
            Tile end = Core.Model.GetTile(new Vector2Int(7, 0));

            Core.Controller.CalcShortestPath(UnitMoveType.Wheels, start, end);

        }
        if (Input.GetKeyDown(KeyCode.S))
        {

        }

        #endregion
    }
    public void SetPosition(Vector2Int pos)
    {
        Tile tile = Core.Model.GetTile(pos);
        this.transform.position = new Vector3(tile.transform.position.x, 0, tile.transform.position.z);
        Position = pos;
        Core.View.StatusPanel.UpdateDisplay(tile);
        Core.Controller.CameraController.SetTargetPos(pos);
    }
    public void BlockInput(bool value)
    {
        _inputBlocked = value;
    }
    public void BlockInput(float duration)
    {
        BlockInput(true);
        StartCoroutine(ActivateInput(duration));
    }
    IEnumerator ActivateInput(float time)
    {
        yield return new WaitForSeconds(time);
        BlockInput(false);
    }
    #region Text
    public void ShowEstimatedDamage(Unit attacker, Unit defender, Tile defendingTile)
    {
        estimatedDamage.gameObject.SetActive(true);
        string damage = Core.Model.BattleCalculations.CalcDamage(attacker, defender, defendingTile).ToString();
        estimatedDamage.text = damage + "% !";
    }
    public void HideEstimnatedDamage()       
    {
        estimatedDamage.gameObject.SetActive(false);
    }
    public void ShowTakeOverCounter()
    {

    }
    #endregion
    #region Cursor Gfx
    public void DisplayCursorGfx(bool value)
    {
        gfx.GetComponent<MeshRenderer>().enabled = value;
    }

    public void SetCursorGfx(int index)
    {
        gfx.GetComponent<MeshFilter>().mesh = meshes[index];   
    }

    #endregion
    #region Input Delay
    void ResetVerticalAxisInUse()
    {
        _vertAxisInUse = false;
    }
    void ResetHorizontalAxisInUse()
    {
        _horAxisInUse = false;
    }

    IEnumerator ResetVerticalAxisInUseDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetVerticalAxisInUse();
    }
    IEnumerator ResetHorizontalAxisInUseDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetHorizontalAxisInUse();
    }

    void ResetButtonPressed()
    {
        _buttonPressed = false;
    }
    IEnumerator ResetButtonPressedDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetButtonPressed();
    }

    #endregion
}
