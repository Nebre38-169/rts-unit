using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// <c>Class Caserne</c>, herits from <c><see cref="Building"/></c>
/// This building allow for unit creation. The selected unit goes to a queue
/// and spawn after it creation cool down. see <c><see cref="Unit"/></c>. It consumme
/// ressources to put a unit the the building queue, 
/// and the order can be cancel with ressources returned
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public class Caserne : Building
{
    //Indicates the list of unit this casern can create
    [SerializeField] public List<UnitHolder> spawnableUnit;
    //Indicates where units spanw
    [SerializeField] public Transform spawnPoint;
    //Use to detect unit at the spawn point
    [SerializeField] public LayerMask spawnObstacleLayer;

    private UIManager manager;
    private RessourceManager ressourceManager;
    private SpriteRenderer selectedRenderer;
    private List<UnitHolder> queuedUnit;
    private float frameCounter = 0;

    new private void Awake()
    {
        base.Awake();
        manager = FindObjectOfType<UIManager>();
        if(manager == null) { throw new System.Exception("Scene is missing UI manager"); }
        ressourceManager = FindObjectOfType<RessourceManager>();
        if(ressourceManager == null ) { throw new System.Exception("Scene is missing Ressource manager"); }
        selectedRenderer = GetComponentInChildren<SpriteRenderer>();
        if(selectedRenderer == null) { throw new System.Exception("Caserne " + gameObject.name + " missing spriterendere"); }
        else { selectedRenderer.enabled = false; }
        queuedUnit = new List<UnitHolder>();
    }
    
    private void FixedUpdate()
    {
        if(queuedUnit.Count > 0)
        {
            //If there are units in the queue, either we create it or we increase the counter
            float coolDown = queuedUnit[0].creationCoolDown;
            if(frameCounter > coolDown * 60)
            {
                instantiateUnit(queuedUnit[0]);
                queuedUnit.RemoveAt(0);
                manager.queueUpdate(queuedUnit.ToArray());
                frameCounter = 0;
            }
            else
            {
                frameCounter++;
            }
        }
    }

    /// <summary>
    /// <c>Function add Unit to Queue</c>
    /// Add a unit to the queue and indicates to
    /// the ressource manager which ressources 
    /// where consumme in the process.
    /// </summary>
    /// <param name="index">The index of the unit holder in the <see cref="Caserne.spawnableUnit"/> list</param>
    public void addUnitToQueue(int index)
    {
        if(queuedUnit.Count < 10)
        {
            queuedUnit.Add(spawnableUnit[index]);
            manager.queueUpdate(queuedUnit.ToArray());
            ressourceManager.removeRessources(
                spawnableUnit[index].neededRessource.ToArray(),
                spawnableUnit[index].cost.ToArray()
                );
        }
    }

    /// <summary>
    /// <c>Function remove Unit From Queue</c>
    /// Remove a unit from the queue and indicates to 
    /// the ressource manager which ressources are 
    /// given back in the process
    /// </summary>
    /// <param name="index"></param>
    public void removeUnitFromQueue(int index)
    {
        if(index < queuedUnit.Count)
        {
            UnitHolder unit = queuedUnit[index];
            queuedUnit.RemoveAt(index);
            manager.queueUpdate(queuedUnit.ToArray());
            ressourceManager.addRessources(
                spawnableUnit[index].neededRessource.ToArray(),
                spawnableUnit[index].cost.ToArray()
                );
        }
    }

    /// <summary>
    /// <c>Function on Selected</c>
    /// Used when the caserne is selected.
    /// Gives the UI manager all the infos needed for UI
    /// </summary>
    public void onSelected()
    {
        debugMessage("Selected");
        selectedRenderer.enabled = true;
        manager.activateCaserneUI(spawnableUnit.ToArray(), queuedUnit.ToArray(), this);
    }

    /// <summary>
    /// <c>Function on Unselected</c>
    /// Used when the caserne is unselected.
    /// Warn the manager that it must deactivate the caserne UI
    /// </summary>
    public void onUnselected()
    {
        debugMessage("Unselected");
        selectedRenderer.enabled = false;
        manager.deactivateCaserneUI();
    }

    /// <summary>
    /// <c>Function check Ressource</c>
    /// For each unit in <see cref="Caserne.spawnableUnit"/>,
    /// calcultes whether or not the unit can be created
    /// with the current ressource
    /// </summary>
    /// <param name="ressources">Dict using Ressource as key and quantities as value</param>
    /// <returns>A boolean array indicating wheter or not the ith unit can be created</returns>
    public bool[] checkRessource(IDictionary<Ressource,int> ressources)
    {
        bool[] result = new bool[spawnableUnit.Count];
        for(int i=0; i < spawnableUnit.Count; i++)
        {
            result[i] = spawnableUnit[i].isConstructible(ressources);
        }
        return result;
    }

    /// <summary>
    /// <c>Function instantiat Unit</c>
    /// Handles unit creation, by seeking a free where the unit can spawn.
    /// It does so by cast a Sphere collider at a position and increate the x position
    /// if there are obstacle in the way.
    /// </summary>
    /// <param name="unit"></param>
    private void instantiateUnit(UnitHolder unit)
    {
        Vector3 spawnPos = spawnPoint.position;
        float radius = unit.prefab.GetComponentInChildren<Renderer>().bounds.size.x/2;
        bool found = false;
        while (!found)
        {
            bool obstacle = false;
            Collider[] intersecting = Physics.OverlapSphere(spawnPos, radius,spawnObstacleLayer);
            int i = 0;
            while (i<intersecting.Length && !obstacle)
            {
                obstacle = intersecting[i].GetType() == typeof(CapsuleCollider) || intersecting[i].GetType() == typeof(BoxCollider);
                i++;
            }
            if (obstacle)
            {
                debugMessage("Increment");
                spawnPos = spawnPos + new Vector3(radius, 0, 0);
            }
            else { found = true; }
        }
        Instantiate(queuedUnit[0].prefab, spawnPos, Quaternion.identity);
    }


}
