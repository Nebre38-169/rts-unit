using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Caserne : Building
{
    [SerializeField] public List<UnitHolder> spawnableUnit;
    [SerializeField] public UnitButton unitButton;

    private GameObject unitPanel;
    private List<UnitButton> unitButtons;
    private List<UnitHolder> queuedUnit;
    private RessourceManager ressourceManager;
    private float frameCounter = 0;

    new private void Awake()
    {
        base.Awake();
        unitPanel = GameObject.Find("UnitPanel");
        if(unitPanel == null) { throw new System.Exception("Scene missing a unit pane"); }
        ressourceManager = FindObjectOfType<RessourceManager>();
        if(ressourceManager == null ) { throw new System.Exception("Scene is missing Ressource manager"); }
        unitButtons = new List<UnitButton>();
        foreach( UnitHolder unitHolder in spawnableUnit )
        {
            UnitButton item = Instantiate(unitButton);
            unitButtons.Add(item);
            item.transform.SetParent(unitPanel.transform);
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!item.disabled.IsActive())
                {
                    //Consommation de ressource
                    //Ajout de l'unité à la liste des unités à générer
                }
            });
        }
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
                frameCounter = 0;
            }
            else
            {
                frameCounter++;
            }
        }
    }

    public void addUnitToQueue(UnitHolder u)
    {
        if(queuedUnit.Count < 10)
        {
            queuedUnit.Add(u);
        }
    }

}
