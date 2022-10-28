using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// <para><c>Class Depot</c>, herits from <c>MonoBehaviour</c></para>
/// <para>This class define behaviour of unloading site, where unit can unload
/// ressources they have gathered.</para>
/// <para>The storedRessource list indicates which ressources can be stored
/// and the quantity of each is stored in the currenQuantity list</para>
/// Made by : Nebre 38-169
/// Last Update : 25/10/2022 by Nebre 38-169
/// </summary>
public class Depot : MonoBehaviour
{
    //Store each ressource than can be stored in this depot
    public List<Ressource> storedRessource;
    //Store the manager, which handle UI and total of ressources
    [SerializeField] public RessourceManager manager;

    /*
     * Hold the current quantity of each ressource, 
     * the quantity at index i is the ressource at index i
     * in the storedRessource list
     */
    private List<int> currentQuantity;
    //Store harvester in case this building is destroyed
    private List<Unit> harvester;
    private bool constructed;
    public Material initialMat;

    private void Awake()
    {
        string gfxName = transform.GetChild(0).name;
        string[] gfx = AssetDatabase.FindAssets("Color");
        foreach (string s in gfx) { Debug.Log(s); }
        harvester = new List<Unit>();
        currentQuantity = new List<int>();
        foreach(Ressource ressource in storedRessource)
        {
            currentQuantity.Add(0);
        }
        if(manager == null)
        {
            manager = GameObject.FindObjectOfType<RessourceManager>();
        }
    }

    //We do not add the depot in the Awake function as the manager could be not initialised yet
    private void Start()
    {
        manager.addDepot(this);
    }

    /// <summary>
    /// <para><c>Function isRessourceUnloadable</c></para>
    /// Indicates wether or not the specified ressource can be unloaded in this depot.
    /// It is done by checking if the ressource appears in the storedRessource list.
    /// </summary>
    /// <param name="r">A ressource to store in this depot</param>
    /// <returns>True if the ressource can be stored here, false otherwise</returns>
    public bool isRessourceUnloadable(Ressource r)
    {
        if (constructed)
        {
            return storedRessource.Contains(r);
        }
        return false;
    }

    /// <summary>
    /// <para><c>Function addHarvester</c></para>
    /// Add an unit to the harvester list, to notify if this depot is destroyed
    /// </summary>
    /// <param name="u"></param>
    public void addHarvester(Unit u)
    {
        if (constructed)
        {
            if (!harvester.Contains(u)) { harvester.Add(u); }
        }
    }

    /// <summary>
    /// <para><c>Function removeHarvest</c></para>
    /// Remove an unit of the harvester list, to avoid over notify unit.
    /// </summary>
    /// <param name="u"></param>
    public void removeHarvest(Unit u)
    {
        if (harvester.Contains(u) && constructed)
        {
            harvester.Remove(u);
        }
    }

    /// <summary>
    /// <para><c>Function getRessourceQuantity</c></para>
    /// Returns the quantity of the specified ressource,
    /// if the ressource is not stored returns -1
    /// </summary>
    /// <param name="r">A ressource</param>
    /// <returns></returns>
    public int getRessourceQuantity(Ressource r)
    {
        if (constructed)
        {
            int index = storedRessource.IndexOf(r);
            if (index > -1)
            {
                return currentQuantity[index];
            }
            else
            {
                return -1;
            }
        } 
        else
        {
            return -1;
        }
        
    }

    /// <summary>
    /// <para><c>Function onUnLoad</c></para>
    /// Handle adding the specified quantity into the right index.
    /// Find the corresponding index using the ressource and
    /// if the ressource is found, add the corresponding quantity
    /// and warn the manager of an update
    /// </summary>
    /// <param name="r"></param>
    /// <param name="quantity"></param>
    /// <returns>Returns true if the quantity was added, false otherwise</returns>
    public bool onUnLoad(Ressource r, int quantity)
    {
        if (constructed)
        {
            int index = storedRessource.IndexOf(r);
            if (index > -1)
            {
                currentQuantity[index] += quantity;
                manager.onQuantityUpdate(r);
                return true;
            }
            return false;
        }
        return false;
        
    }

    public void setConstructed(bool b)
    {
        constructed = b;
        if (constructed)
        {
            Debug.Log("Constructed");
            GetComponentInChildren<Renderer>().material = initialMat;
        }
        else
        {
            GetComponentInChildren<Renderer>().material.color = Color.gray;
        }
    }
}
