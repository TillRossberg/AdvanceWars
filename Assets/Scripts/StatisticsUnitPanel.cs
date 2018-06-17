using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsUnitPanel : MonoBehaviour
{
    public Image unitThumb;
    public Text unitsBuiltText;
    public Text unitName;
    
    public void setValues(Sprite myNewImage, int builtCount, string myUnitName)
    {
        this.unitThumb.sprite = myNewImage;
        this.unitsBuiltText.text = builtCount.ToString();
        this.unitName.text = myUnitName;
    }
}