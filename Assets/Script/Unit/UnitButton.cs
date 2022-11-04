using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    public Image icon;
    public Image disabled;

    public void setIcon(Sprite i)
    {
        icon.sprite = i;
    }

    public void setDisabled(bool s)
    {
        disabled.enabled = s;
    }
}
