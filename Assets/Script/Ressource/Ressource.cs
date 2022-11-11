using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para><c>Class Ressource</c>, herits from <c>ScriptableObject</c></para>
/// Stores the name, the weight and the icon of a ressource
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
[CreateAssetMenu(fileName ="New ressource",menuName ="RTS/ressource")]
public class Ressource: ScriptableObject
{
    public string ressourceName;
    public float weight;
    public Sprite icon;
}
