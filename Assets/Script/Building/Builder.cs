using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Builder : MonoBehaviour
{
    
    private BuildingHolder construstedBuilding;
    private GameObject building;
    private List<Unit> builder;
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
            //Debug.Log("Builder in range : " + builderInRange.Count);
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

    public void addBuilder(Unit u)
    {
        if (!builder.Contains(u)) { builder.Add(u); }
    }

    public void removeBuilder(Unit u)
    {
        if(builder.Contains(u)) { builder.Remove(u); }
    }

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
