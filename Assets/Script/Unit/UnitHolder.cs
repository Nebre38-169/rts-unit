using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// <c>Class Unit Holder</c>, herits from <c>ScriptableObject</c>
/// Stores info on a unit, its name, prefab, icon, the ressources need 
/// to create it and its costs in those.
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
[CreateAssetMenu(fileName ="New Unit",menuName ="RTS/unit")]
public class UnitHolder : ScriptableObject
{
    public string unitName;
    public Unit prefab;
    public Sprite icon;
    public List<Ressource> neededRessource;
    public List<int> cost;
    public float creationCoolDown;

    /// <summary>
    /// <c>Function is Constructible</c>
    /// Indicates wheter or not the unit can be constructed 
    /// with the current ressource
    /// </summary>
    /// <param name="d">Dictionnary using ressources as key and quantities as value</param>
    /// <returns>True if the unit can be created, false otherwise</returns>
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
