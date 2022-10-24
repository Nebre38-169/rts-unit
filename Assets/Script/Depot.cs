using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depot : MonoBehaviour
{
    public List<Ressource> storedRessource;
    [SerializeField] public RessourceManager manager;

    public List<int> currentQuantity;
    private List<Unit> harvester;

    private void Awake()
    {
        harvester = new List<Unit>();
        currentQuantity = new List<int>();
        foreach(Ressource ressource in storedRessource)
        {
            currentQuantity.Add(0);
        }
        
    }

    private void Start()
    {
        manager.addDepot(this);
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

    public int getRessourceQuantity(Ressource r)
    {
        int index = storedRessource.IndexOf(r);
        if(index > -1)
        {
            return currentQuantity[index];
        } else
        {
            return -1;
        }
    }

    public bool onUnLoad(Ressource r, int quantity)
    {
        int index = storedRessource.IndexOf(r);
        if(index > -1)
        {
            currentQuantity[index] += quantity;
            manager.onQuantityUpdate(r);
            return true;
        }
        return false;
    }
}
