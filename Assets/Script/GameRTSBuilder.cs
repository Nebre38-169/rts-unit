using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameRTSBuilder : MonoBehaviour
{
    [SerializeField] public List<BuildingHolder> buildings;
    [SerializeField] public BuildingButton itemButton;
    public GameObject buildingPanel;
    public Transform placeholder;
    public Builder builderPrefabs;
    public LayerMask spaceDetectionMask;

    private BuildingHolder selectedBuilding;
    private List<BuildingButton> buttonList;
    private RessourceManager ressourceManager;

    private void Awake()
    {
        ressourceManager = GameObject.FindObjectOfType<RessourceManager>();
        if(ressourceManager == null) { throw new SystemException("Scene missing RessourceManager"); }
        placeholder.gameObject.SetActive(false);
        buttonList = new List<BuildingButton>();
        foreach(BuildingHolder building in buildings)
        {
            BuildingButton item = Instantiate(itemButton);
            buttonList.Add(item);
            item.transform.SetParent(buildingPanel.transform);
            item.setIcon(building.icon);
            item.setSelected(false);
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!item.disabled.IsActive())
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
                }
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
                for(int i = 0; i < selectedBuilding.needRessource.Count; i++)
                {
                    ressourceManager.removeRessourceQuantity(
                        selectedBuilding.needRessource[i],
                        selectedBuilding.cost[i]);
                }
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

    private bool canSpawn(BuildingHolder build, Vector3 pos)
    {
        BoxCollider buildingBoxCollider = build.prefabs.GetComponent<BoxCollider>();
        Collider[] collision = Physics.OverlapBox(pos, buildingBoxCollider.size/2, Quaternion.identity,spaceDetectionMask);
        List<Collider> collisions = new List<Collider>();
        foreach(Collider col in collision) {
            if((col.gameObject.name.Contains("unit") || col.gameObject.name.Contains("Detector")) && col.GetType() == typeof(SphereCollider))
            {
                //nothing
            }
            else
            {
                collisions.Add(col);
            }
        }
        return collisions.Count <= 0;
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

    public void onRessourceUpdate(IDictionary<Ressource,int> q)
    {
        for(int i = 0; i < buildings.Count; i++)
        {
            bool constructible = buildings[i].isConstructible(q);
            buttonList[i].setDisabled(!constructible);
        }
    }
    
}
