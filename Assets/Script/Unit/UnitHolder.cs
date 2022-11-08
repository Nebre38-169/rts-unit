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

    public bool isConstructible(IDictionary<Ressource, int> d)
    {
        int j = 0;
        bool result = true;
        while (j < neededRessource.Count && result)
        {
            if (d[neededRessource[j]] < cost[j])
            {
                result = false;
            }
            j++;
        }
        return result;
    }
}
