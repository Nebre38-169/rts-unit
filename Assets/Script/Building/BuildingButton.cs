using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public Image icon;
    public Image selected;
    public Image disabled;

    public void setIcon(Sprite i)
    {
        icon.sprite = i;
    }

    public void setSelected(bool s)
    {
        selected.enabled = s;
    }

    public void setDisabled(bool s)
    {
        disabled.enabled = s;
    }
}
