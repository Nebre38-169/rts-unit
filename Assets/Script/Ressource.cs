using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour
{
    public float maxQuantity = 10f;
    public float givenQuantity;

    private float currentQuantity;
    private List<Unit> harvester;

    private void Awake()
    {
        currentQuantity = maxQuantity;
        harvester = new List<Unit>();
    }

    private void onRessourceEmpty()
    {
        foreach(Unit unit in harvester)
        {
            unit.onRessourceEmpty();
        }
        Destroy(this.gameObject);
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

    public float getGivenQuantity()
    {
        if(currentQuantity < maxQuantity) { return currentQuantity; }
        else { return givenQuantity; }
    }
    public float onHarvest(Unit u, float quantity)
    {
        Debug.Log(quantity);
        if(quantity > givenQuantity)
        {
            quantity = givenQuantity;
        }
        if(quantity > currentQuantity)
        {
            quantity = currentQuantity;
        }
        currentQuantity -= quantity;
        if (currentQuantity <= 0)
        {
            onRessourceEmpty();
        }
        Debug.Log(currentQuantity);
        return quantity;
    }

}
