using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>Class UI Manager</c>, herits from MonoBehaviour, implements <see cref="RessourceObserver"/>
/// Handles UI for building, casernes and ressources.
/// </summary>
public class UIManager : MonoBehaviour, RessourceObserver
{
    //Ressource panel in the scene, holds ressources indicators
    [SerializeField] public RectTransform ressourcePanel;
    //Building panel in the scene, holds button for choosing building
    [SerializeField] public RectTransform buildingPanel;
    //Caserne panel in the scene, holds queue for creating unit
    [SerializeField] public RectTransform casernePanel;
    //Selection panel for caserne in the scene, holds button for choosing unit
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

    /// <summary>
    /// <c>Function generate Ressource Div</c>
    /// Generates a <see cref="RessourceDiv"/> for every ressources,
    /// and stores it. They are placed in the <see cref="UIManager.ressourcePanel"/>
    /// </summary>
    /// <param name="r">List of all ressources</param>
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

    /// <summary>
    /// <c>Function update One Ressource</c>
    /// Updates the displaied amount for the specified ressource
    /// </summary>
    /// <param name="index"></param>
    /// <param name="quantity"></param>
    public void updateOneRessource(int index, int quantity)
    {
        ressourceDisplays[index].updateAmount(quantity);
    }

    /// <summary>
    /// <c>Function reset Selection Building</c>
    /// Resets display of the selected building 
    /// </summary>
    public void resetSelectedBuilding()
    {
        foreach (BuildingButton item in buildingButtons)
        {
            item.setSelected(false);
        }
    }

    /// <summary>
    /// <c>Function set Disable Building</c>
    /// Sets the button at index active or not
    /// </summary>
    /// <param name="index"></param>
    /// <param name="disable"></param>
    public void setDisableBuilding(int index, bool disable)
    {
        buildingButtons[index].setDisabled(disable);
    }

    /// <summary>
    /// <c>Function activate Caserne UI</c>
    /// Activates the UI for caserne using info from the selected caserne.
    /// Resets the queue and generate the selection panel
    /// </summary>
    /// <param name="unitList"></param>
    /// <param name="queue"></param>
    /// <param name="c"></param>
    public void activateCaserneUI(UnitHolder[] unitList, UnitHolder[] queue, Caserne c)
    {
        buildingPanel.gameObject.SetActive(false);
        casernePanel.gameObject.SetActive(true);
        currentCaserne = c;
        resetQueue();
        setQueue(queue);
        resetSelectionPanel();
        setSelectionPanel(unitList);
        onRessourceUpdate(ressourceManager.getQuantities());
    }

    /// <summary>
    /// <c>Function deactivate Caserne UI</c>
    /// </summary>
    public void deactivateCaserneUI()
    {
        activateBuildingUI();
    }

    /// <summary>
    /// <c>Function queue Update</c>
    /// Resets and sets the queue using the given list
    /// </summary>
    /// <param name="queue"></param>
    public void queueUpdate(UnitHolder[] queue)
    {
        resetQueue();
        setQueue(queue);
    }

    /// <summary>
    /// <c>Function add Unit To Queue</c>
    /// Warns the caserne to add the unit matching the index to the queue
    /// </summary>
    /// <param name="index"></param>
    public void addUnitToQueue(int index)
    {
        currentCaserne.addUnitToQueue(index);
    }

    /// <summary>
    /// <c>Function cancel Unit In Queue</c>
    /// Warns the caserne to remove the unit in the matching queue index
    /// </summary>
    /// <param name="index"></param>
    public void cancelUnitInQueue(int index)
    {
        currentCaserne.removeUnitFromQueue(index);
    }

    /// <summary>
    /// <c>Function on Ressource Update</c>
    /// If a caserne is selected, asks it to check which 
    /// unit can be created and updates the UI accordingly
    /// </summary>
    /// <param name="ressources"></param>
    public void onRessourceUpdate(IDictionary<Ressource, int> ressources)
    {
        for(int i = 0; i < ressources.Count; i++)
        {
            updateOneRessource(i, ressources[ressources.Keys.ToArray()[i]]);
        }
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
    
    /// <summary>
    /// <c>Function activate Building UI</c>
    /// Set the Building panel active and deactivate the Caserne panel
    /// </summary>
    private void activateBuildingUI()
    {
        casernePanel.gameObject.SetActive(false);
        buildingPanel.gameObject.SetActive(true);

    }
    
    /// <summary>
    /// <c>Function init Building UI</c>
    /// Create the UI for buildings by creating a button for each of them
    /// </summary>
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

    /// <summary>
    /// <c>Function reset Queue</c>
    /// Reset the unit creation queue by removing icon and listeners
    /// </summary>
    private void resetQueue()
    {
        foreach (QueueSlot q in queueSlots)
        {
            q.resetIcon();
            q.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// <c></c>
    /// </summary>
    /// <param name="unitList"></param>
    private void setQueue(UnitHolder[] unitList)
    {
        for (int i = 0; i < unitList.Length; i++)
        {
            queueSlots[i].setIcon(unitList[i].icon);
        }
    }

    /// <summary>
    /// <c>Function set Selection Panel</c>
    /// Generates <see cref="UnitButton"/> in the caserne/selection panel
    /// for each <see cref="UnitHolder"/> hold by the selected caserne
    /// </summary>
    /// <param name="unitList"></param>
    private void setSelectionPanel(UnitHolder[] unitList)
    {
        for(int i=0;i<unitList.Length; i++)
        {
            UnitButton u = Instantiate<UnitButton>(unitButtonPrefab, selectionPanel);
            unitButtons.Add(u);
            u.setup(unitList[i].icon, i);
        }
    }

    /// <summary>
    /// <c>Function reset Selection Panel</c>
    /// Resets the selcetion panel by deleting every unit buttons
    /// </summary>
    private void resetSelectionPanel()
    {
        for (int i = 0; i < unitButtons.Count; i++){
            Destroy(unitButtons[i].gameObject);
        }
        unitButtons.Clear();
    }

    /// <summary>
    /// <c>Function set Unit Button</c>
    /// Allows to set <see cref="UnitButton"/> as disabled or not
    /// </summary>
    /// <param name="index">Index of the unit button</param>
    /// <param name="active">True if the unit button must be available, false otherwise</param>
    private void setUnitButton(int index, bool active)
    {
        unitButtons[index].setDisabled(!active);
    }
}
