using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceManager : MonoBehaviour
{
    public List<Ressource> ressources;
    public RessourceDiv prefabs;
    public GameObject ressourcePanel;
    private List<int> ressourceQuantities;
    private List<Depot> depots;
    private List<RessourceDiv> displays;

    private void Awake()
    {
        ressourceQuantities = new List<int>();
        depots = new List<Depot>();
        displays = new List<RessourceDiv>();
        for(int i = 0; i < ressources.Count; i++)
        {
            ressourceQuantities.Add(0);
            RessourceDiv div = Instantiate<RessourceDiv>(prefabs);
            div.transform.SetParent(ressourcePanel.transform, false);
            div.updateAmount(0);
            div.updateImage(ressources[i].icon);
            displays.Add(div);
        }
    }

    public void addDepot(Depot d)
    {
        if (!depots.Contains(d)) { depots.Add(d); }
    }

    public void removeDepot(Depot d)
    {
        if (depots.Contains(d)) { depots.Remove(d); }
    }

    public void onQuantityUpdate(Ressource r)
    {
        Debug.Log("Updating ressource " + r.ressourceName);
        int index = ressources.IndexOf(r);
        if(index > -1)
        {
            int quantity = 0;
            foreach(Depot d in depots)
            {
                if (d.isRessourceUnloadable(r))
                {
                    quantity += d.getRessourceQuantity(r);
                }
            }
            ressourceQuantities[index] = quantity;
            Debug.Log("Current quantity :" + quantity);
            displays[index].updateAmount(quantity);
        }
    }

}
