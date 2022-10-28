using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameRTSBuilder : MonoBehaviour
{
    [SerializeField] public List<Building> buildings;
    [SerializeField] public BuildingButton itemButton;
    public GameObject buildingPanel;
    public Transform placeholder;
    public Builder builderPrefabs;
    public LayerMask spaceDetectionMask;

    private Building selectedBuilding;
    private List<BuildingButton> buttonList;

    private void Awake()
    {
        placeholder.gameObject.SetActive(false);
        buttonList = new List<BuildingButton>();
        foreach(Building building in buildings)
        {
            BuildingButton item = Instantiate(itemButton);
            buttonList.Add(item);
            item.transform.SetParent(buildingPanel.transform);
            item.setIcon(building.icon);
            item.setSelected(false);
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedBuilding = building;
                Vector3 buildingSize = selectedBuilding.prefabs.GetComponent<BoxCollider>().size;
                Vector3 placeholderSize = placeholder.GetComponent<Renderer>().bounds.size;
                Vector3 scale = new Vector3(
                buildingSize.x / placeholderSize.x,
                1,
                buildingSize.z / placeholderSize.z
                ); 
                placeholder.localScale = scale;
                item.setSelected(true);
            });
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            resetSelection();
        }
        if(Input.GetMouseButtonDown(0) && selectedBuilding != null)
        {
            Vector3 position = Utils.getMousePositionOnFloor();
            if (canSpawn(selectedBuilding, position))
            {
                Builder b = Instantiate(builderPrefabs, position, Quaternion.identity);
                b.startBuilding(selectedBuilding);
                resetSelection();
            }
        }

        if (selectedBuilding != null)
        {
            placeholder.gameObject.SetActive(true);
            Vector3 position = Utils.getMousePositionOnFloor();
            placeholder.position = transform.InverseTransformPoint(new Vector3(position.x, 0.5f, position.z));
            if (canSpawn(selectedBuilding, position))
            {
                placeholder.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            }
            else
            {
                placeholder.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (selectedBuilding)
        {
            BoxCollider buildingBoxCollider = selectedBuilding.prefabs.GetComponent<BoxCollider>();
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Utils.getMousePositionOnFloor(), buildingBoxCollider.size);
        }
    }

    private bool canSpawn(Building build, Vector3 pos)
    {
        BoxCollider buildingBoxCollider = build.prefabs.GetComponent<BoxCollider>();
        Collider[] collision = Physics.OverlapBox(pos, buildingBoxCollider.size/2, Quaternion.identity,spaceDetectionMask);
        return collision.Length <= 0;

    }

    private void resetSelection()
    {
        placeholder.gameObject.SetActive(false);
        selectedBuilding = null;
        foreach (BuildingButton item in buttonList)
        {
            item.setSelected(false);
        }
    }
    
}
