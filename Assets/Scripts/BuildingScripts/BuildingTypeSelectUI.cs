using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingTypeSelectUI : MonoBehaviour {
    [SerializeField] private GameObject ManagerObject;
    private BuildingManager buildingManager;
    private List<BuildingTypeSO> buildingTypeSOList;

    private Dictionary<BuildingTypeSO, Transform> buildingBtnDictionary;
    private void Start() {


        //try catch?
        buildingManager = ManagerObject.GetComponent<BuildingManager>();
        buildingTypeSOList = buildingManager.buildingTypeSOList;
        //

        Transform buildingBtnTemplate = transform.Find("buildingButtonTemplate");
        buildingBtnTemplate.gameObject.SetActive(false);


        buildingBtnDictionary = new Dictionary<BuildingTypeSO, Transform>();
        int index = 0;
        
        foreach(BuildingTypeSO buildingTypeSO in buildingTypeSOList) {
            
            Transform buildingBtnTransform  = Instantiate(buildingBtnTemplate, transform);
            buildingBtnTransform.gameObject.SetActive(true);

            buildingBtnTransform.GetComponent<RectTransform>().anchoredPosition += new Vector2(index * 110, 0);
            buildingBtnTransform.Find("image").GetComponent<Image>().sprite = buildingTypeSO.sprite;

            buildingBtnTransform.GetComponent<Button>().onClick.AddListener(() => {
                buildingManager.SetActiveBuildingType(buildingTypeSO);
                Debug.Log(buildingTypeSO);
                UpdateSelectedVisual();
            });

            buildingBtnDictionary[buildingTypeSO] = buildingBtnTransform;
            index++;
        }


        UpdateSelectedVisual();
    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            BuildingTypeSO NextBuidingType = NextBuildingTypeOnList();
            buildingManager.SetActiveBuildingType(NextBuidingType);
            UpdateSelectedVisual();
        }
    }
    private void UpdateSelectedVisual() {
        foreach (BuildingTypeSO buildingTypeSO in buildingBtnDictionary.Keys) {
            buildingBtnDictionary[buildingTypeSO].Find("selected").gameObject.SetActive(false);
        }

        BuildingTypeSO activeBuildingType = buildingManager.GetActiveBuildingType();
        buildingBtnDictionary[activeBuildingType].Find("selected").gameObject.SetActive(true);
    }

    private BuildingTypeSO NextBuildingTypeOnList() {
        int indexCurrentBT = buildingTypeSOList.IndexOf(buildingManager.GetActiveBuildingType());
        if (indexCurrentBT == buildingTypeSOList.Count - 1)
            return buildingTypeSOList[0];
        else
            return buildingTypeSOList[indexCurrentBT + 1];
    }
}
