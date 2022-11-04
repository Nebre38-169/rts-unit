using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHolder : ScriptableObject
{
    public string unitName;
    public Unit prefab;
    public List<Ressource> neededRessource;
    public List<int> cost;
    public float creationCoolDown;
}
