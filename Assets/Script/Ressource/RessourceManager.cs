using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para><c>Class Ressource Manager</c>, herits from <c>MonoBehaviour</c></para>
/// <para>Gather every ressource from depot and totalise them.</para>
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public class RessourceManager : MonoBehaviour
{
    //Hold every ressource that must be counted
    public List<Ressource> ressources;
    [SerializeField] public UIManager manager;

    //At index i, indicates quantity of ressource i
    private List<int> ressourceQuantities;
    //Every depot is stored here until it is destroyed
    private List<Depot> depots;
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
        //On startup, create a RessourceDiv and a null quantity for every ressource
        for(int i = 0; i < ressources.Count; i++)
        {
            ressourceQuantities.Add(0);
        }
    }

    private void Start()
    {
        gameRTSBuilder.onRessourceUpdate(getQuantities());
        manager.generetaRessourceDiv(ressources.ToArray());
        manager.onRessourceUpdate(getQuantities());
    }

    /// <summary>
    /// <c>Function get Quantities</c>
    /// Calculates and returns a dictionary with every ressources
    /// as an entry and the available amount of the ressources as the value
    /// </summary>
    /// <returns>Dictionnairy using Ressources as key and quantities as value</returns>
    public IDictionary<Ressource, int> getQuantities()
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

    /// <summary>
    /// <c>Function add Ressources</c>
    /// Adds a list of quantities for matching ressources to the current quantities.
    /// Warm the <see cref="UIManager"/> and the <see cref="GameRTSBuilder"/> for ressource changes
    /// </summary>
    /// <param name="ressourceList">A list of every ressources to update</param>
    /// <param name="quantities">A list of quantities of each ressources in the same order</param>
    public void addRessources(Ressource[] ressourceList, int[] quantities)
    {
        for(int j = 0; j < ressourceList.Length; j++)
        {
            addOneRessourceQuantity(ressourceList[j], quantities[j]);
        }
        manager.onRessourceUpdate(getQuantities());
        gameRTSBuilder.onRessourceUpdate(getQuantities());
    }

    /// <summary>
    /// <c>Function add Ressources</c>
    /// Adds a quantity of ressource to the current quantities.
    /// Warm the <see cref="UIManager"/> and the <see cref="GameRTSBuilder"/> for ressource changes
    /// </summary>
    /// <param name="r">A ressource to update</param>
    /// <param name="quantity">The quantity to increase the amount of the ressource</param>
    public void addRessources(Ressource r, int quantity)
    {
        addOneRessourceQuantity(r, quantity);
        manager.onRessourceUpdate(getQuantities());
        gameRTSBuilder.onRessourceUpdate(getQuantities());
    }

    /// <summary>
    /// <c>Function revmoce Ressources</c>
    /// Removes a list of quantities for matching ressources to the current quantities.
    /// Warm the <see cref="UIManager"/> and the <see cref="GameRTSBuilder"/> for ressource changes
    /// </summary>
    /// <param name="ressourceList">A list of every ressources to update</param>
    /// <param name="quantities">A list of quantities of each ressources in the same order</param>
    public void removeRessources(Ressource[] ressourceList, int[] quantities)
    {
        for (int j = 0; j < ressourceList.Length; j++)
        {
            removeOneRessourceQuantity(ressourceList[j], quantities[j]);
        }
        manager.onRessourceUpdate(getQuantities());
        gameRTSBuilder.onRessourceUpdate(getQuantities());
    }

    /// <summary>
    /// <c>Function remove Ressources</c>
    /// Removes a quantity of ressource to the current quantities.
    /// Warm the <see cref="UIManager"/> and the <see cref="GameRTSBuilder"/> for ressource changes
    /// </summary>
    /// <param name="r">A ressource to update</param>
    /// <param name="quantity">The quantity to increase the amount of the ressource</param>
    public void removeRessources(Ressource r, int quantity)
    {
        removeOneRessourceQuantity(r, quantity);
        manager.onRessourceUpdate(getQuantities());
        gameRTSBuilder.onRessourceUpdate(getQuantities());
    }

    /// <summary>
    /// <c>Function add One Ressoure Quantity</c>
    /// Adds the quantity to the matchin ressource.
    /// </summary>
    /// <param name="r">The ressource concerned</param>
    /// <param name="quantity">The quantity added</param>
    private void addOneRessourceQuantity(Ressource r, int quantity)
    {
        int index = ressources.IndexOf(r);
        if(index > -1)
        {
            ressourceQuantities[index] += quantity;
        }
    }

    /// <summary>
    /// <c>Function remove One Ressoure Quantity</c>
    /// Removes the quantity to the matchin ressource.
    /// </summary>
    /// <param name="r">The ressource concerned</param>
    /// <param name="quantity">The quantity removed</param>
    private void removeOneRessourceQuantity(Ressource r, int quantity)
    {
        int index = ressources.IndexOf(r);
        if(index > -1)
        {
            ressourceQuantities[index] -= quantity;
        }
    }
}
