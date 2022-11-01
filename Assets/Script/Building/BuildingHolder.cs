using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New building", menuName = "RTS/Building")]
public class BuildingHolder : ScriptableObject
{
    public Sprite icon;
    public GameObject prefabs;
    public float constructionDuration;
    public List<Ressource> needRessource;
    public List<int> cost;

    public bool isConstructible(IDictionary<Ressource,int> d)
    {
        int j = 0;
        bool result = true;
        while (j < needRessource.Count && result)
        {
            if (d[needRessource[j]] < cost[j])
            {
                result = false;
            }
            j++;
        }
        return result;
    }
}
