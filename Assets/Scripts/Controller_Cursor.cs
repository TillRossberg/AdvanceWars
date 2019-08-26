using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
public class Controller_Cursor : MonoBehaviour
{
    public GameObject gfx;
    public TextMeshPro estimatedDamage;
    public Vector2Int Position { get; private set; }
    public List<Mesh> meshes;

    bool inputBlocked = true;
    float inputDelay;
    bool horAxisInUse;
    bool vertAxisInUse;
    bool buttonPressed;

    float buttonHoldDelay;
    float buttonPressedTimer = 0;
    bool holdingB = false;

    public void Init(Vector2Int position)
    {
        SetPosition(position);
        inputDelay = Core.Model.Database.inputDelay;
        buttonHoldDelay = Core.Model.Database.buttonHoldDelay;
    }

    private void Update()
    {
        if (!inputBlocked)
        {
            #region Movement
            //Up
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                if (!vertAxisInUse)
                {
                    vertAxisInUse = true;
                    Core.Controller.GoTo(new Vector2Int(Position.x, Position.y + 1));
                    StartCoroutine(ResetVerticalAxisInUseDelayed(inputDelay));
                }
            }
            else
            //Down
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                if (!vertAxisInUse)
                {
                    vertAxisInUse = true;
                    Core.Controller.GoTo(new Vector2Int(Position.x, Position.y - 1));
                    StartCoroutine(ResetVerticalAxisInUseDelayed(inputDelay));
                }
            }
            //Left
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                if (!horAxisInUse)
                {
                    horAxisInUse = true;
                    Core.Controller.GoTo(new Vector2Int(Position.x - 1, Position.y));
                    StartCoroutine(ResetHorizontalAxisInUseDelayed(inputDelay));
                }
            }
            else
            //Right
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                if (!horAxisInUse)
                {
                    horAxisInUse = true;
                    Core.Controller.GoTo(new Vector2Int(Position.x + 1, Position.y));
                    StartCoroutine(ResetHorizontalAxisInUseDelayed(inputDelay));
                }
            }

            #endregion
            #region Buttons
            if (Input.GetButtonDown("Jump"))
            {
                if (!buttonPressed)
                {
                    Core.Controller.AButton();
                    buttonPressed = true;
                    StartCoroutine(ResetButtonPressedDelayed(inputDelay));
                }
            }
            if (Input.GetButtonDown("Cancel"))
            {
                Core.Controller.BButton();
                buttonPressedTimer = 0;
            }
            //Holding B button
            if (Input.GetButton("Cancel"))
            {
                buttonPressedTimer += Time.deltaTime;
                if (buttonPressedTimer >= buttonHoldDelay && !holdingB)
                {
                    Core.Controller.BButtonHold();
                    holdingB = true;
                }
            }
            if (Input.GetButtonUp("Cancel"))
            {
                if (buttonPressedTimer >= buttonHoldDelay)
                {
                    Core.Controller.BButtonReleased();
                    buttonPressedTimer = 0;
                    holdingB = false;
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
            List<Tile> tiles = Core.Model.GetTilesInRadius(Core.Model.GetTile(8, 4), 3);
            Core.Controller.IndicateTiles(tiles);

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            List<int> ints = new List<int>();
            ints.Add(5);
            ints.Add(2);
            ints.Add(17);
            ints.Add(3);
            int[] integers = ints.OrderBy(h => h).ToArray();
            ints = new List<int>(integers);
            foreach (var item in ints)
            {
                Debug.Log(item);
            }
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
        inputBlocked = value;
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
    public void HideEstimatedDamage()       
    {
        estimatedDamage.gameObject.SetActive(false);
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
        vertAxisInUse = false;
    }
    void ResetHorizontalAxisInUse()
    {
        horAxisInUse = false;
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
        buttonPressed = false;
    }
    IEnumerator ResetButtonPressedDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetButtonPressed();
    }

    #endregion
}
