using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>Class Building Holder</c>, herits from <c>ScriptableObject</c>
/// Holds every information on a building, its prefab, icon, ressources need to built it
/// and its cost in those ressources.
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
[CreateAssetMenu(fileName = "New building", menuName = "RTS/Building")]
public class BuildingHolder : ScriptableObject
{
    public Sprite icon;
    public GameObject prefabs;
    public float constructionDuration;
    public List<Ressource> needRessource;
    public List<int> cost;

    /// <summary>
    /// <c>Function is Constructible</c>
    /// Indicates from the current ressources dictionnary whether or not 
    /// the building can be constructed
    /// </summary>
    /// <param name="d">Dictionnaire with Ressource as key and quantities as value</param>
    /// <returns>True if the building can be built, false otherwise</returns>
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
