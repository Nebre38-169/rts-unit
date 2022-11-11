using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>Class building button</c>, herits from MonoBehaviour,
/// Hold Image components for the button that allow building.
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public class BuildingButton : MonoBehaviour
{
    public Image icon;
    public Image selected;
    public Image disabled;

    /// <summary>
    /// <c>Function set Icon</c>
    /// Set the icon (building image)
    /// </summary>
    /// <param name="i"></param>
    public void setIcon(Sprite i)
    {
        icon.sprite = i;
    }

    /// <summary>
    /// <c>Function set Selected</c>
    /// Activates border to indicates this building is the selected one
    /// </summary>
    /// <param name="s"></param>
    public void setSelected(bool s)
    {
        selected.enabled = s;
    }

    /// <summary>
    /// <c>Function set Disabled</c>
    /// Indicates whether or not a building can be built, and add a gray layer on the button
    /// </summary>
    /// <param name="s"></param>
    public void setDisabled(bool s)
    {
        disabled.enabled = s;
    }
}
