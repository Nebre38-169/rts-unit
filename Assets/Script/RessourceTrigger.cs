using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceTrigger : MonoBehaviour
{
    private List<RessourceSource> ressourcesInRange;
    private List<Depot> depotInRange;

    private void Awake()
    {
        ressourcesInRange = new List<RessourceSource>();
        depotInRange = new List<Depot>();
    }

    public bool isTargetRessourceInRange(RessourceSource target)
    {
        return ressourcesInRange.Contains(target);
    }

    public bool isTargetDepotInRange(Depot target)
    {
        return depotInRange.Contains(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        RessourceSource r = other.GetComponent<RessourceSource>();
        Depot d = other.GetComponent<Depot>();
        if (r != null)
        {
            if (!ressourcesInRange.Contains(r))
            {
                ressourcesInRange.Add(r);
            }
        }
        else if (d != null)
        {
            if (!depotInRange.Contains(d))
            {
                depotInRange.Add(d);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RessourceSource r = other.GetComponent<RessourceSource>();
        Depot d = other.GetComponent<Depot>();
        if (r != null)
        {
            if (ressourcesInRange.Contains(r))
            {
                ressourcesInRange.Remove(r);
            }
        }
        else if (d != null)
        {
            if (depotInRange.Contains(d))
            {
                depotInRange.Remove(d);
            }
        }
    }
}
