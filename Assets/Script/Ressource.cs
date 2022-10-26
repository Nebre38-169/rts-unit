using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para><c>Class Ressource</c>, herits from <c>ScriptableObject</c></para>
/// Stores data about a Ressource.
/// <param name="ressourceName">The name of the ressource</param>
/// <param name="weight">Weight of one unit</param>
/// <param name="icon">Sprite of the icon for the UI</param>
/// </summary>
[CreateAssetMenu(fileName ="New ressource",menuName ="RTS/ressource")]
public class Ressource: ScriptableObject
{
    public string ressourceName;
    public float weight;
    public Sprite icon;

    public Ressource(string name, float w)
    {
        ressourceName = name;
        weight = w;
    }
}
