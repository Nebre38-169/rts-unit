using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/// <summary>
/// <para><c>Class Builder</c>, herits from <c>MonoBehaviour</c></para>
/// Handle construction of any bulding. It is placed by the <see cref="GameRTSBuilder"/>
/// and hold the futur building. Building rise from the ground and
/// once it as reach its maximum height the building is instantiated
/// and the builder disapear. Friendly unit can help build faster
/// </summary>
public class Builder : MonoBehaviour
{
    //Indicates how much each unit can contributes to the construction
    public float unitWeight = 0.2f;
    //Holds the futur building
    private BuildingHolder construstedBuilding;
    //Holds the animated GFX of the building rising
    private GameObject building;
    //Holds the list of unit that target this builder as their task objectiv
    private List<Unit> builder;
    //Count how many unit are in range to increase the building speed
    private List<Unit> builderInRange;
    private float incr;

    private void Awake()
    {
        builder = new List<Unit>();
        builderInRange = new List<Unit>();
    }

    private void FixedUpdate()
    {
        if(construstedBuilding != null)
        {
            //The increment is calculated as a percentage of the incr variable.
            //Each unit in range contributes the same.
            float increment = incr * (1 + 0.2f * builderInRange.Count);
            if(building.transform.position.y < 0)
            {
                building.transform.position = new Vector3(
                building.transform.position.x,
                building.transform.position.y + incr,
                building.transform.position.z);
            }
            else
            {
                onBuildFinished();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //The interaction range is indicated by the RessourceTrigger object that every unit as.
        //When the RessourceTrigger collide with the builder trigger, the unit is added to the list 
        //if it is already stored in the builder list.
        RessourceTrigger u = other.GetComponent<RessourceTrigger>();
        if(u != null)
        {
            Unit unitInRange = u.gameObject.GetComponentInParent<Unit>();
            if (builder.Contains(unitInRange))
            {
                builderInRange.Add(unitInRange);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RessourceTrigger u = other.GetComponent<RessourceTrigger>();
        if (u != null)
        {
            Unit unitInRange = u.gameObject.GetComponentInParent<Unit>();
            removeBuilder(unitInRange);
        }
    }

    /// <summary>
    /// <c>Function on Building Finished</c>.
    /// When the Building is finished, the builder instanciates the building
    /// and destroy it self by unselecting it self from every unit that previously targeted its
    /// </summary>
    private void onBuildFinished()
    {
        Debug.Log("Builded");
        building.GetComponent<Depot>().setConstructed(true);
        foreach(Unit u in builder)
        {
            u.unsetBuildingTarger();
        }
        Destroy(this.gameObject);
    }

    /// <summary>
    /// <c>Function add Builder</c>
    /// Add a unit to the builder list.
    /// </summary>
    /// <param name="u">a Unit</param>
    public void addBuilder(Unit u)
    {
        if (!builder.Contains(u)) { builder.Add(u); }
    }

    /// <summary>
    /// <c>Function remove Builder</c>
    /// Remove a builder from the builder list
    /// </summary>
    /// <param name="u"></param>
    public void removeBuilder(Unit u)
    {
        if(builder.Contains(u)) { builder.Remove(u); }
    }

    /// <summary>
    /// <c>Function start Building</c>
    /// After the Builder has been placed, the <see cref="GameRTSBuilder"/>
    /// launch the construction by giving a building to build.
    /// </summary>
    /// <param name="build">Scriptable object that holds the Building</param>
    public void startBuilding(BuildingHolder build)
    {
        construstedBuilding = build;
        Vector3 size = construstedBuilding.prefabs.GetComponent<BoxCollider>().size;
        Vector3 position = transform.position;
        position.y = -size.y;
        incr = size.z / (60 * construstedBuilding.constructionDuration);
        building = Instantiate(construstedBuilding.prefabs, position,Quaternion.identity);
        building.GetComponent<Depot>().setConstructed(false);
        GetComponent<BoxCollider>().size = new Vector3(size.x, 1, size.z);
    }
}
