using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        string name = gameObject.name;
        char c = name[name.IndexOf('(') + 1];
        if(c == 'S') { index = 0; }
        else { index = int.Parse(name[name.IndexOf('(') + 1].ToString()); }
        manager = FindObjectOfType<UIManager>();
        if(manager == null) { throw new System.Exception("Scene missing UImanager"); }
    }

    public void setIcon(Sprite i)
    {
        icon.sprite = i;
    }

    public void resetIcon()
    {
        icon.sprite = baseImage;
    }

    public void onCancel()
    {
        manager.cancelUnitInQueue(index);
    }

}
