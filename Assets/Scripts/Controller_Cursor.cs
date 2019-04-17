using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller_Cursor : MonoBehaviour
{    
    float _inputDelay;
    bool _horAxisInUse;
    bool _vertAxisInUse;
    bool _buttonPressed;
    public GameObject gfx;
    public Vector2Int Position { get; private set; }
    public List<Mesh> meshes;

    public void Init(Vector2Int position)
    {
        SetPosition(position);
        _inputDelay = Core.Model.inputDelay;
    }

    private void Update()
    {
        #region Movement
        //Up
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            if (!_vertAxisInUse)
            {
                _vertAxisInUse = true;
                Core.Controller.GoTo(new Vector2Int(Position.x, Position.y + 1));
                StartCoroutine(ResetAxisInUseDelayed(_inputDelay));
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
                StartCoroutine(ResetAxisInUseDelayed(_inputDelay));
            }
        }
        //Left
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            if (!_horAxisInUse)
            {
                _horAxisInUse = true;
                Core.Controller.GoTo(new Vector2Int(Position.x - 1, Position.y));
                StartCoroutine(ResetAxisInUseDelayed(_inputDelay));
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
                StartCoroutine(ResetAxisInUseDelayed(_inputDelay));
            }
        }

        #endregion
        
        if (Input.GetButtonDown("Jump"))
        {
            if(!_buttonPressed)
            {
                Core.Controller.AButton();
                _buttonPressed = true;
                StartCoroutine(ResetButtonPressedDelayed(_inputDelay));
            }
        }      
        if (Input.GetButtonDown("Cancel") )
        {
            Core.Controller.BButton();
        }
       
        #region Debug
        if (Input.GetKeyDown(KeyCode.A))
        {
           
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            
        }

        #endregion
    }

    

    public void SetPosition(Vector2Int pos)
    {
        Tile tile = Core.Model.GetTile(pos);
        this.transform.position = new Vector3(tile.transform.position.x, 0, tile.transform.position.z);
        Position = pos;
        Core.View.statusPanel.UpdateDisplay(tile);
    }

    #region Cursor Gfx
    private void DisplayCursorGfx(bool value)
    {
        gfx.GetComponent<MeshRenderer>().enabled = value;
    }

    public void SetCursorGfx(int index)
    {
        gfx.GetComponent<MeshFilter>().mesh = meshes[index];   
    }

    #endregion
    #region Input Delay
    void ResetAxisInUse()
    {
        _horAxisInUse = false;
        _vertAxisInUse = false;
    }

    IEnumerator ResetAxisInUseDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetAxisInUse();
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
