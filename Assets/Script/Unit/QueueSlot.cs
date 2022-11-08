using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueSlot : MonoBehaviour
{
    public Sprite baseImage;
    private Image icon;

    private void Awake()
    {
        icon = GetComponent<Image>();
        if(icon == null) { throw new System.Exception("Icon missing on slot " + gameObject.name); }
    }

    public void setIcon(Sprite i)
    {
        icon.sprite = i;
    }

    public void resetIcon()
    {
        icon.sprite = baseImage;
    }

}
