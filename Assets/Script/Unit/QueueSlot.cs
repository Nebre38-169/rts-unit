using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>Class Queue Slot</c>
/// UI element of one slot in the unit queue.
/// Holds the image and handles interaction with the slot.
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public class QueueSlot : MonoBehaviour
{
    public Sprite baseImage;


    private int index;
    private Image icon;
    private UIManager manager;

    private void Awake()
    {
        icon = GetComponent<Image>();
        if(icon == null) { throw new System.Exception("Icon missing on slot " + gameObject.name); }
        //We calculate the index of the slot using its name
        //If the name has a number in it, it is after the openning parenthes
        //If there is no number, then it is the first one (index=0)
        string name = gameObject.name;
        char c = name[name.IndexOf('(') + 1];
        if(c == 'S') { index = 0; }
        else { index = int.Parse(name[name.IndexOf('(') + 1].ToString()); }
        manager = FindObjectOfType<UIManager>();
        if(manager == null) { throw new System.Exception("Scene missing UImanager"); }
    }

    /// <summary>
    /// <c>Function set Icon</c>
    /// Sets the icon of the slot (back image)
    /// </summary>
    /// <param name="i"></param>
    public void setIcon(Sprite i)
    {
        icon.sprite = i;
    }

    /// <summary>
    /// <c>Function reset Icon</c>
    /// Empty the icon for the default background
    /// </summary>
    public void resetIcon()
    {
        icon.sprite = baseImage;
    }

    /// <summary>
    /// <c>Function on Cancel</c>
    /// Warn the <see cref="UIManager"/> that the unit in slot index
    /// must be canceled
    /// </summary>
    public void onCancel()
    {
        manager.cancelUnitInQueue(index);
    }

}
