using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RessourceDiv : MonoBehaviour
{
    private TMP_Text text;
    private Image image;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        image = GetComponentInChildren<Image>();
        if(name == null || image == null)
        {
            throw new System.Exception("Missing name or image on " + gameObject.name);
        }
    }

    public void updateAmount(int amout)
    {
        text.SetText(amout.ToString());
    }

    public void updateImage(Sprite icon)
    {
        image.sprite = icon;
    }
}
