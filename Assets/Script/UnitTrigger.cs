using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para><c>Class Unit Trigger</c>,herits from MonoBehaviour</para>
/// <para>Used to detect ressource or depot in range,
/// should be hold by an empty object, child of the unit</para>
/// </summary>
public class UnitTrigger : MonoBehaviour
{
    //Stores every ressource in range
    private List<RessourceSource> ressourcesInRange;
    //Stores every depot in range
    private List<Depot> depotInRange;
    private List<Builder> buildInRange;

    private void Awake()
    {
        ressourcesInRange = new List<RessourceSource>();
        depotInRange = new List<Depot>();
        buildInRange = new List<Builder>();

    }

    private void OnTriggerEnter(Collider other)
    {
        RessourceSource r = other.GetComponent<RessourceSource>();
        Depot d = other.GetComponent<Depot>();
        Builder b = other.GetComponent<Builder>();
        //If a source enter in the trigger, the source is in range
        if (r != null)
        {
            if (!ressourcesInRange.Contains(r))
            {
                ressourcesInRange.Add(r);
            }
        }
        //If a depot enter in the trigger, the depot is in range
        else if (d != null)
        {
            if (!depotInRange.Contains(d))
            {
                depotInRange.Add(d);
            }
        }
        else if(b != null)
        {
            if (!buildInRange.Contains(b))
            {
                buildInRange.Add(b);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RessourceSource r = other.GetComponent<RessourceSource>();
        Depot d = other.GetComponent<Depot>();
        Builder b = other.GetComponent<Builder>();
        //If a source leave the trigger, the source is no longer in range
        if (r != null)
        {
            if (ressourcesInRange.Contains(r))
            {
                ressourcesInRange.Remove(r);
            }
        }
        //If a depot leave the trigger, the depot is no longer in range
        else if (d != null)
        {
            if (depotInRange.Contains(d))
            {
                depotInRange.Remove(d);
            }
        }
        else if(b != null)
        {
            if (buildInRange.Contains(b))
            {
                buildInRange.Remove(b);
            }
        }
    }

    /// <summary>
    /// <para><c>Function is Target Ressource In Range</c></para>
    /// <para>Checks if the specified ressource source is in range,
    /// by checking if the source is in the list</para>
    /// </summary>
    /// <param name="target">The RessourceSource that must be in range</param>
    /// <returns>True if the source is in range, false otherwise</returns>
    public bool isTargetRessourceInRange(RessourceSource target)
    {
        return ressourcesInRange.Contains(target);
    }

    /// <summary>
    /// <para><c>Function is Target Depot In Range</c></para>
    /// <para>Checks if the specified depot is in range,
    /// by checking if the depot is in the list</para>
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool isTargetDepotInRange(Depot target)
    {
        return depotInRange.Contains(target);
    }

    public bool isBuildInRange(Builder target)
    {
        return buildInRange.Contains(target);
    }


}
