using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Unit",menuName ="RTS/unit")]
public class UnitHolder : ScriptableObject
{
    public string unitName;
    public Unit prefab;
    public Sprite icon;
    public List<Ressource> neededRessource;
    public List<int> cost;
    public float creationCoolDown;
}
