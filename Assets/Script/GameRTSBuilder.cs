using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>Class Game RTS Builder</c>, herits from MonoBehaviour
/// Handles mouse interface for placing bluiding
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public class GameRTSBuilder : MonoBehaviour
{
    //Stores building that can be built
    [SerializeField] public List<BuildingHolder> buildings;
    public Builder builderPrefab;
    //This placeholder is used to indicates the space taken by a building
    public Transform placeholder;
    //Used to detect collision with the futur building, to avoid stacking building
    public LayerMask spaceDetectionMask;

    private BuildingHolder selectedBuilding;
    private RessourceManager ressourceManager;
    private UIManager manager;

    private void Awake()
    {
        ressourceManager = GameObject.FindObjectOfType<RessourceManager>();
        if(ressourceManager == null) { throw new SystemException("Scene missing RessourceManager"); }
        manager = FindObjectOfType<UIManager>();
        if(manager == null) { throw new SystemException("Scene missing manager"); }
        placeholder.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            resetSelection();
        }
        if(Input.GetMouseButtonDown(0) && selectedBuilding != null)
        {
            //When the user place a building, we check if it fits, then place the builder and start building
            //We also remove ressources
            Vector3 position = Utils.getMousePositionOnFloor();
            if (canSpawn(selectedBuilding, position))
            {
                ressourceManager.removeRessources(
                    selectedBuilding.needRessource.ToArray(),
                    selectedBuilding.cost.ToArray()
                    );
                Builder b = Instantiate(builderPrefab, position, Quaternion.identity);
                b.startBuilding(selectedBuilding);
                resetSelection();
            }
        }

        if (selectedBuilding != null)
        {
            //When there is a selected building, we make the placeholder match the size of the futur building
            //and moves it with the mouse. It also changes color to indicates if a building can be placed
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

    /// <summary>
    /// <c>Function can Spawn</c>
    /// Indicates if the building can be created at the given position.
    /// For this, it cast a box collider the size of the building and look for collider within this box.
    /// If the collider is  a unit, a detector or a turret and a spherical collider then it is not considered
    /// </summary>
    /// <param name="build"></param>
    /// <param name="pos"></param>
    /// <returns>True if the building can be place at the position, false otherwise </returns>
    private bool canSpawn(BuildingHolder build, Vector3 pos)
    {
        BoxCollider buildingBoxCollider = build.prefabs.GetComponent<BoxCollider>();
        Collider[] collision = Physics.OverlapBox(pos, buildingBoxCollider.size/2, Quaternion.identity,spaceDetectionMask);
        List<Collider> collisions = new List<Collider>();
        foreach(Collider col in collision) {
            Debug.Log(col);
            Debug.Log(col.gameObject.name);
            if((col.gameObject.name.Contains("unit") || col.gameObject.name.Contains("Detector") || col.gameObject.name.Contains("Turret")) && col.GetType() == typeof(SphereCollider))
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

    /// <summary>
    /// <c>Function reset Selection</c>
    /// Set the placeholder inactive, remove the selected building and reset UI
    /// </summary>
    private void resetSelection()
    {
        placeholder.gameObject.SetActive(false);
        selectedBuilding = null;
        manager.resetSelectedBuilding();
    }

    /// <summary>
    /// <c>Function on Ressource Update</c>
    /// When ressources' quantities are updated,
    /// calculates if all building can be built
    /// </summary>
    /// <param name="q">Dictionnary using Ressources as key and quantities as value</param>
    public void onRessourceUpdate(IDictionary<Ressource,int> q)
    {
        for(int i = 0; i < buildings.Count; i++)
        {
            bool constructible = buildings[i].isConstructible(q);
            manager.setDisableBuilding(i, !constructible);
        }
    }

    /// <summary>
    /// <c>Function set Setected Building</c>
    /// Set the selected building and calculates the size of the placeholder
    /// </summary>
    /// <param name="b"></param>
    public void setSelectedBuilding(BuildingHolder b)
    {
        selectedBuilding = b;
        Vector3 buildingSize = selectedBuilding.prefabs.GetComponent<BoxCollider>().size;
        Vector3 placeholderSize = placeholder.GetComponent<Renderer>().bounds.size;
        Vector3 scale = new Vector3(
        buildingSize.x / placeholderSize.x,
        1,
        buildingSize.z / placeholderSize.z
        );
        placeholder.localScale = scale;
    }
    
}
