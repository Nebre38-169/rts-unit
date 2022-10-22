using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceSource : MonoBehaviour
{
    public int maxQuantity = 10;
    public int givenQuantity;
    [SerializeField] public Ressource ressource;

    private int currentQuantity;
    private List<Unit> harvester;

    private void Awake()
    {
        currentQuantity = maxQuantity;
        harvester = new List<Unit>();
    }

    private void onRessourceEmpty()
    {
        RessourceSource remplacement = getClosestRessource();
        Debug.Log(remplacement);
        foreach(Unit unit in harvester)
        {
            unit.onRessourceEmpty(remplacement);
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

    public int onHarvest(Unit u, int quantity)
    {
        //Debug.Log(quantity);
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
        Debug.Log("Remaining :"+ currentQuantity);
        return quantity;
    }

    private RessourceSource getClosestRessource()
    {
        RessourceSource[] ressourceArray = GameObject.FindObjectsOfType<RessourceSource>();
        List<RessourceSource> ressourceList = new List<RessourceSource>();
        for (int i = 0; i < ressourceArray.Length; i++)
        {
            if(ressourceArray[i].ressource == ressource) { ressourceList.Add(ressourceArray[i]); }
        }

        if (ressourceList.Contains(this)) { ressourceList.Remove(this); }

        if(ressourceList.Count <= 0)
        {
            return null;
        }
        else if(ressourceList.Count == 1)
        {
            return ressourceList[0];
        }
        else
        {
            float closesDistance = Mathf.Infinity;
            RessourceSource candidate = ressourceList[0];
            foreach (RessourceSource source in ressourceList)
            {
                float dist = Mathf.Abs(Vector3.Distance(transform.position, source.transform.position));
                if (dist < closesDistance)
                {
                    closesDistance = dist;
                    candidate = source;
                }
            }
            return candidate;
        }
    }

}
