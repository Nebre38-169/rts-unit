using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Caserne : Building
{
    [SerializeField] public List<UnitHolder> spawnableUnit;
    [SerializeField] public Transform spawnPoint;
    [SerializeField] public LayerMask spawnObstacleLayer;

    private UIManager manager;
    private SpriteRenderer selectedRenderer;
    private List<UnitHolder> queuedUnit;
    private RessourceManager ressourceManager;
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

    /* Doit gérer la possibilité de plusieurs caserne
     * donc un affichage qui pourrait changer en fonction de la caserne,
     * aussi il serait bon de pouvoir afficher la queue de production
     * et de pouvoir retirer des éléments de cette queue avec un retour
     * des ressources. Il faut gérer ce changement d'affichage en fonction
     * de la caserne selectionner
     */
    

    private void FixedUpdate()
    {
        if(queuedUnit.Count > 0)
        {
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

    public void addUnitToQueue(int index)
    {
        if(queuedUnit.Count < 10)
        {
            debugMessage(""+ index);
            queuedUnit.Add(spawnableUnit[index]);
            manager.queueUpdate(queuedUnit.ToArray());
            for(int i = 0; i < spawnableUnit[i].neededRessource.Count; i++)
            {
                ressourceManager.removeRessourceQuantity(
                    spawnableUnit[index].neededRessource[i],
                    spawnableUnit[index].cost[i]
                    );
            }
        }
    }

    public void removeFromQueue(int index)
    {
        if(index < queuedUnit.Count)
        {
            UnitHolder unit = queuedUnit[index];
            queuedUnit.RemoveAt(index);
            manager.queueUpdate(queuedUnit.ToArray());
            for (int i = 0; i < unit.neededRessource.Count; i++)
            {
                ressourceManager.addRessourceQuantity(
                    spawnableUnit[index].neededRessource[i],
                    spawnableUnit[index].cost[i]
                    );
            }
        }
    }

    public void onSelected()
    {
        debugMessage("Selected");
        selectedRenderer.enabled = true;
        manager.activateCaserneUI(spawnableUnit.ToArray(), queuedUnit.ToArray(), this);
    }

    public void onUnSelected()
    {
        debugMessage("Unselected");
        selectedRenderer.enabled = false;
        manager.deactivateCaserneUI();
    }

    public bool[] checkRessource(IDictionary<Ressource,int> ressources)
    {
        bool[] result = new bool[spawnableUnit.Count];
        for(int i=0; i < spawnableUnit.Count; i++)
        {
            result[i] = spawnableUnit[i].isConstructible(ressources);
        }
        return result;
    }

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
                //debugMessage(intersecting[i].gameObject.name);
                //debugMessage(intersecting[i].ToString());
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
