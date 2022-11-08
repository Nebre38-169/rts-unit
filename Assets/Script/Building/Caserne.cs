using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Caserne : Building
{
    [SerializeField] public List<UnitHolder> spawnableUnit;

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
                Instantiate(queuedUnit[0].prefab);
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

}
