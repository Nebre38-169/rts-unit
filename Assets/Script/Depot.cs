using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depot : MonoBehaviour
{
    private float currentQuantity;
    private List<Unit> harvester;

    private void Awake()
    {
        harvester = new List<Unit>();
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

    public void onUnLoad(Unit u, float quantity)
    {
        currentQuantity += quantity;
        Debug.Log(currentQuantity);
    }
}
