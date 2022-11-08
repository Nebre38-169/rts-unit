using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] public RectTransform ressourcePanel;
    [SerializeField] public RectTransform buildingPanel;
    [SerializeField] public RectTransform casernePanel;
    [SerializeField] public RectTransform selectionPanel;
    [SerializeField] public RessourceDiv ressourceDivPrefab;
    [SerializeField] public BuildingButton buildingButtonPrefab;
    [SerializeField] public UnitButton unitButtonPrefab;

    private List<RessourceDiv> ressourceDisplays;
    private List<BuildingButton> buildingButtons;
    private List<UnitButton> unitButtons;
    private List<QueueSlot> queueSlots;
    private GameRTSBuilder builder;
    private RessourceManager ressourceManager;
    private Caserne currentCaserne;

    private void Awake()
    {
        buildingButtons = new List<BuildingButton>();
        builder = FindObjectOfType<GameRTSBuilder>();
        if (builder == null) { throw new System.Exception("Missing GameRTSBuilder in scene"); }
        ressourceDisplays = new List<RessourceDiv>();
        QueueSlot[] q = GetComponentsInChildren<QueueSlot>();
        if (q == null || q.Length < 9) { throw new System.Exception("Missing slot on UI"); }
        else { queueSlots = new List<QueueSlot>(q); }
        ressourceManager = FindObjectOfType<RessourceManager>();
        if(ressourceManager == null) { throw new System.Exception("Missing Ressource Manager in scene"); }
        unitButtons = new List<UnitButton>();
        activateBuildingUI();
        initBuildingUI();
    }

    public void generetaRessourceDiv(Ressource[] r)
    {
        foreach (Ressource ressource in r)
        {
            RessourceDiv div = Instantiate<RessourceDiv>(ressourceDivPrefab, ressourcePanel);
            div.updateAmount(0);
            div.updateImage(ressource.icon);
            ressourceDisplays.Add(div);
        }
    }

    public void updateOneRessource(int index, int quantity)
    {
        ressourceDisplays[index].updateAmount(quantity);
    }

    public void resetSelectedBuilding()
    {
        foreach (BuildingButton item in buildingButtons)
        {
            item.setSelected(false);
        }
    }

    public void setDisableBuilding(int index, bool disable)
    {
        buildingButtons[index].setDisabled(disable);
    }

    public void activateCaserneUI(UnitHolder[] unitList, UnitHolder[] queue, Caserne c)
    {
        buildingPanel.gameObject.SetActive(false);
        casernePanel.gameObject.SetActive(true);
        currentCaserne = c;
        resetQueue();
        setQueue(queue);
        resetSelectionPanel();
        setSelectionPanel(unitList);
        onRessourceUpdate(ressourceManager.calculQuantity());
    }

    public void deactivateCaserneUI()
    {
        activateBuildingUI();
    }

    public void queueUpdate(UnitHolder[] queue)
    {
        resetQueue();
        setQueue(queue);
    }

    public void addUnitToQueue(int index)
    {
        currentCaserne.addUnitToQueue(index);
    }

    public void cancelUnitInQueue(int index)
    {
        currentCaserne.removeFromQueue(index);
    }

    public void onRessourceUpdate(IDictionary<Ressource, int> ressources)
    {
        Debug.Log(currentCaserne);
        if(currentCaserne != null)
        {
            bool[] activated = currentCaserne.checkRessource(ressources);
            for(int i=0;i<activated.Length; i++)
            {
                Debug.Log(activated[i]);
                setUnitButton(i, activated[i]);
            }
        }
    }

    private void activateBuildingUI()
    {
        casernePanel.gameObject.SetActive(false);
        buildingPanel.gameObject.SetActive(true);

    }

    private void initBuildingUI()
    {
        foreach (BuildingHolder building in builder.buildings)
        {
            BuildingButton item = Instantiate(buildingButtonPrefab, buildingPanel);
            buildingButtons.Add(item);
            item.setIcon(building.icon);
            item.setSelected(false);
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!item.disabled.IsActive())
                {
                    builder.setSelectedBuilding(building);
                    item.setSelected(true);
                }
            });
        }
    }

    private void resetQueue()
    {
        foreach (QueueSlot q in queueSlots)
        {
            q.resetIcon();
            q.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void setQueue(UnitHolder[] unitList)
    {
        for (int i = 0; i < unitList.Length; i++)
        {
            queueSlots[i].setIcon(unitList[i].icon);
        }
    }

    private void setSelectionPanel(UnitHolder[] unitList)
    {
        for(int i=0;i<unitList.Length; i++)
        {
            UnitButton u = Instantiate<UnitButton>(unitButtonPrefab, selectionPanel);
            unitButtons.Add(u);
            u.setup(unitList[i].icon, i);
        }
    }

    private void resetSelectionPanel()
    {
        for (int i = 0; i < unitButtons.Count; i++){
            Destroy(unitButtons[i].gameObject);
        }
        unitButtons.Clear();
    }

    private void setUnitButton(int index, bool active)
    {
        unitButtons[index].setDisabled(!active);
    }
}
