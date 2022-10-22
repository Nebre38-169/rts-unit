using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New ressource",menuName ="RTS/ressource")]
public class Ressource: ScriptableObject
{
    public string ressourceName;
    public float weight;
    public Ressource(string name, float w)
    {
        ressourceName = name;
        weight = w;
    }
}
