using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] public RectTransform ressourcePanel;
    [SerializeField] public RectTransform buildingPanel;
    [SerializeField] public RectTransform casernePanel;
    [SerializeField] public RessourceDiv ressourceDivPrefab;
    [SerializeField] public BuildingButton buildingButtonPrefab;

    private List<RessourceDiv> ressourceDisplays;
    private List<BuildingButton> buildingButtons;
    private GameRTSBuilder builder;

    private void Awake()
    {
        buildingButtons = new List<BuildingButton>();
        builder = FindObjectOfType<GameRTSBuilder>();
        if (builder == null){ throw new System.Exception("Missing GameRTSBuilder in scene"); }
        ressourceDisplays = new List<RessourceDiv>();
        activateBuildingUI();
    }

    public void generetaRessourceDiv(Ressource[] r)
    {
        foreach(Ressource ressource in r)
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
        foreach(BuildingButton item in buildingButtons)
        {
            item.setSelected(false);
        }
    }

    public void setDisableBuilding(int index, bool disable)
    {
        buildingButtons[index].setDisabled(disable);
    }

    private void activateCaserneUI()
    {
        buildingPanel.gameObject.SetActive(false);

        casernePanel.gameObject.SetActive(true);
    }

    private void activateBuildingUI()
    {
        casernePanel.gameObject.SetActive(false);
        buildingPanel.gameObject.SetActive(true);
        foreach(BuildingHolder building in builder.buildings)
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

}
