using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// <para><c>Class RessourceDiv</c>, herits from <c>MonoBehaviour</c></para>
/// Stores UI element and handle update of count and icon for a given ressource
/// Made by : Nebre 38-169
/// Last Update : 25/11/2022 by Nebre 38-169
/// </summary>
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

    /// <summary>
    /// <para><c>Function updateAmount</c></para>
    /// Update the TextMeshPro that hold the amount of ressource
    /// </summary>
    /// <param name="amout"></param>
    public void updateAmount(int amout)
    {
        text.SetText(amout.ToString());
    }

    /// <summary>
    /// <para><c>Function updateImage</c></para>
    /// Update the Image that hold the icon of the ressource
    /// </summary>
    /// <param name="icon"></param>
    public void updateImage(Sprite icon)
    {
        image.sprite = icon;
    }
}
