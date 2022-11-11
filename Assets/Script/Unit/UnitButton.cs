using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>Class Unit Button</c>, herits from <c>MonoBehaviour</c>
/// Stores and handles UI of every unit that can be created in a <see cref="Caserne"/>
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public class UnitButton : MonoBehaviour
{
    public Image icon;
    public Image disabled;

    private int index;
    private UIManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<UIManager>();
        if(manager == null) { throw new System.Exception("Scene is missing a UI manager"); }
    }

    /// <summary>
    /// <c>Function setup</c>
    /// Set the icon of the Button (background image),
    /// and set the index of the button
    /// </summary>
    /// <param name="i"></param>
    /// <param name="ind"></param>
    public void setup(Sprite i, int ind)
    {
        setIcon(i);
        index = ind;
    }

    /// <summary>
    /// <c>Function set Icon</c>
    /// Set the icon of the Button (background image)
    /// </summary>
    /// <param name="i"></param>
    public void setIcon(Sprite i)
    {
        icon.sprite = i;
    }

    /// <summary>
    /// <c>Function set Disabled</c>
    /// Activates or not the button,
    /// and add a gray layer to indicates if the button is active
    /// </summary>
    /// <param name="s"></param>
    public void setDisabled(bool s)
    {
        disabled.enabled = s;
    }

    /// <summary>
    /// <c>Function on Select</c>
    /// When the button is clicked, warn the manager to add the unit to queue
    /// </summary>
    public void onSelect()
    {
        if (!disabled.enabled) { manager.addUnitToQueue(index); }
    }
}
