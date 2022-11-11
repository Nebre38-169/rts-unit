using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para><c>Class Ressource Source</c>, herits from MonoBehaviour</para>
/// <para>Stores a given quantity of a specified ressource,
/// handle notifing unit when it is empty</para>
/// Made by : Nebre 38-169
/// Last Update : 25/10/2022 by Nebre 38-169
/// </summary>
public class RessourceSource : MonoBehaviour
{
    //Max and initial quantity of the ressource
    public int maxQuantity = 10;
    //Max quantity given
    public int givenQuantity;
    //Hold the ressource that this source gives
    [SerializeField] public Ressource ressource;

    private int currentQuantity;
    //Hold every unit that harvest from this source to avoid comming back here for nothing
    private List<Unit> harvester;

    private void Awake()
    {
        currentQuantity = maxQuantity;
        harvester = new List<Unit>();
    }

    /// <summary>
    /// <para><c>Function on Ressource Empty</c></para>
    /// <para>Triggered when there is no more ressource in this source,
    /// Find a replacement source and warn every harvester that 
    /// this is empty and gives them the replacement</para>
    /// </summary>
    private void onRessourceEmpty()
    {
        RessourceSource remplacement = getClosestRessource();
        foreach(Unit unit in harvester)
        {
            unit.onRessourceEmpty(remplacement);
        }
        //In the futur, empty source will not be destroyed 
        Destroy(this.gameObject);
    }

    /// <summary>
    /// <para><c>Function add Harvester</c></para>
    /// <para>Add a unit to the harvester list,
    /// check if the unit is not already in the list</para>
    /// </summary>
    /// <param name="u">The unit added</param>
    public void addHarvester(Unit u)
    {
        if (!harvester.Contains(u)) { harvester.Add(u); }
    }

    /// <summary>
    /// <para><c>Function remove Harvester</c></para>
    /// <para>Remove a unit from the harvester list,
    /// check if the unit is in the list</para>
    /// </summary>
    /// <param name="u">The unit to remove</param>
    public void removeHarvester(Unit u)
    {
        if (harvester.Contains(u))
        {
            harvester.Remove(u);
        }
    }

    /// <summary>
    /// <para><c>Function on Harvest</c></para>
    /// <para>Trigger by the unit that harvest this source,
    /// It ensures that the asked quantity is below the givenQuantity 
    /// and that there is enough quantity.</para>
    /// <para>Also, it checks if the remaining quantity is above 0,
    /// if not, triggers the on Ressource Empty method</para>
    /// </summary>
    /// <param name="u">The unit that harvest</param>
    /// <param name="quantity">The quantity asked by the unit</param>
    /// <returns>The quantity the source can give</returns>
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
        return quantity;
    }

    /// <summary>
    /// <para><c>Function get Closest Ressource</c></para>
    /// <para>Get the closest source that gives the same ressource</para>
    /// <para>Find all source and filter by ressource, then cycle through them 
    /// to find the closest one</para>
    /// </summary>
    /// <returns>The closest ressource source that gives the same ressource,
    /// null if none was found</returns>
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
