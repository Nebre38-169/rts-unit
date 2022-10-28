using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    
    private Building construstedBuilding;
    private GameObject building;
    private List<Unit> builder;
    private int frameCounter;
    private float incr;

    private void FixedUpdate()
    {
        if(construstedBuilding != null)
        {
            frameCounter++;
            if(building.transform.position.y < 0)
            {
                building.transform.position = new Vector3(
                building.transform.position.x,
                building.transform.position.y + incr,
                building.transform.position.z);
            }
            if(frameCounter >= construstedBuilding.constructionDuration * 60)
            {
                Debug.Log("Builded");
                building.GetComponent<Depot>().setConstructed(true);
                Destroy(this.gameObject);
            }
        }
    }
    public void startBuilding(Building build)
    {
        construstedBuilding = build;
        Vector3 size = construstedBuilding.prefabs.GetComponent<BoxCollider>().size;
        Vector3 position = transform.position;
        Debug.Log(size);
        position.y = -size.y;
        incr = size.z / (60 * construstedBuilding.constructionDuration);
        Debug.Log(incr);
        building = Instantiate(construstedBuilding.prefabs, position,Quaternion.identity);
        building.GetComponent<Depot>().setConstructed(false);
        Vector3 newSize = building.GetComponentInChildren<Renderer>().bounds.size;
        Debug.Log(newSize);
    }
}
