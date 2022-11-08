using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void setup(Sprite i, int ind)
    {
        setIcon(i);
        index = ind;
    }

    public void setIcon(Sprite i)
    {
        icon.sprite = i;
    }

    public void setDisabled(bool s)
    {
        disabled.enabled = s;
    }

    public void onSelect()
    {
        if (!disabled.enabled) { manager.addUnitToQueue(index); }
    }
}
