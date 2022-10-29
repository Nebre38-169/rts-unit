using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para><c>Class Ressource Manager</c>, herits from <c>MonoBehaviour</c></para>
/// <para>Gather every ressource from depot and totalise them.</para>
/// <para>Also, it handles the UI with update every time a depot get new ressource</para>
/// </summary>
public class RessourceManager : MonoBehaviour
{
    //Hold every ressource that must be counted
    public List<Ressource> ressources;
    //Use for UI
    public RessourceDiv prefabs;
    public GameObject ressourcePanel;
    //At index i, indicates quantity of ressource i
    private List<int> ressourceQuantities;
    //Every depot is stored here until it is destroyed
    private List<Depot> depots;
    private List<RessourceDiv> displays;
    private GameRTSBuilder gameRTSBuilder;

    private void Awake()
    {
        gameRTSBuilder = GameObject.FindObjectOfType<GameRTSBuilder>();
        if(gameRTSBuilder == null)
        {
            throw new SystemException("Missing GameRTSBuilder");
        }
        ressourceQuantities = new List<int>();
        depots = new List<Depot>();
        displays = new List<RessourceDiv>();
        //On startup, create a RessourceDiv and a null quantity for every ressource
        for(int i = 0; i < ressources.Count; i++)
        {
            ressourceQuantities.Add(0);
            RessourceDiv div = Instantiate<RessourceDiv>(prefabs);
            div.transform.SetParent(ressourcePanel.transform, false);
            div.updateAmount(0);
            div.updateImage(ressources[i].icon);
            displays.Add(div);
        }
    }

    private void Start()
    {
        gameRTSBuilder.onRessourceUpdate(calculQuantity());
    }

    private IDictionary<Ressource, int> calculQuantity()
    {
        IDictionary<Ressource, int> quantity = new Dictionary<Ressource, int>();
        for(int i = 0; i < ressources.Count; i++)
        {
            quantity.Add(ressources[i], ressourceQuantities[i]);
        }
        return quantity;
    }

    /// <summary>
    /// <para><c>Function add Depot</c></para>
    /// <para>Add a depot to the list</para>
    /// <para>Check if the depot is not already in the list to avoid doublon</para>
    /// </summary>
    /// <param name="d">the depot that must be added</param>
    public void addDepot(Depot d)
    {
        if (!depots.Contains(d)) { depots.Add(d); }
    }

    /// <summary>
    /// <para><c>Function remove Depot</c></para>
    /// <para>Remove a depot from the list</para>
    /// <para>Check if the depot is in the list before removing it</para>
    /// </summary>
    /// <param name="d">the depot that must be removed</param>
    public void removeDepot(Depot d)
    {
        if (depots.Contains(d)) { depots.Remove(d); }
    }

    public void addRessourceQuantity(Ressource r, int quantity)
    {
        int index = ressources.IndexOf(r);
        if(index > -1)
        {
            ressourceQuantities[index] += quantity;
            displays[index].updateAmount(ressourceQuantities[index]);
            gameRTSBuilder.onRessourceUpdate(calculQuantity());
        }
    }

    public void removeRessourceQuantity(Ressource r, int quantity)
    {
        int index = ressources.IndexOf(r);
        if(index > -1)
        {
            ressourceQuantities[index] -= quantity;
            displays[index].updateAmount(ressourceQuantities[index]);
            gameRTSBuilder.onRessourceUpdate(calculQuantity());
        }
    }
}
