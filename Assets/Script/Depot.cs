using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depot : MonoBehaviour
{
    public List<Ressource> storedRessource;
    private List<float> currentQuantity;
    private List<Unit> harvester;

    private void Awake()
    {
        harvester = new List<Unit>();
        currentQuantity = new List<float>();
        foreach(Ressource ressource in storedRessource)
        {
            currentQuantity.Add(0f);
        }
    }

    public bool isRessourceUnloadable(Ressource r)
    {
        return storedRessource.Contains(r);
    }

    public void addHarvester(Unit u)
    {
        if (!harvester.Contains(u)) { harvester.Add(u); }
    }

    public void removeHarvest(Unit u)
    {
        if (harvester.Contains(u))
        {
            harvester.Remove(u);
        }
    }

    public bool onUnLoad(Ressource r, float quantity)
    {
        int index = storedRessource.IndexOf(r);
        if(index > -1)
        {
            currentQuantity[index] += quantity;
            return true;
        }
        return false;
    }
}
