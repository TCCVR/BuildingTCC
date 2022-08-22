using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveableObjectsTypeSelectUI : MonoBehaviour {
    [SerializeField] private GameObject ManagerObject;
    private MoveableObjectsManager moveableObjectsManager;
    private List<MoveableObjectsSO> moveableObjectsTypeSOList;

    private Dictionary<MoveableObjectsSO, Transform> moveableObjectsBtnDictionary;
    private void Start() {


        //try catch?
        moveableObjectsManager = ManagerObject.GetComponent<MoveableObjectsManager>();
        moveableObjectsTypeSOList = moveableObjectsManager.moveableObjectsTypeSOList;
        //

        Transform moveableObjectsBtnTemplate = transform.Find("buildingButtonTemplate");
        moveableObjectsBtnTemplate.gameObject.SetActive(false);


        moveableObjectsBtnDictionary = new Dictionary<MoveableObjectsSO, Transform>();
        int index = 0;
        
        foreach(MoveableObjectsSO moveableObjectsTypeSO in moveableObjectsTypeSOList) {
            
            Transform moveableObjectsBtnTransform  = Instantiate(moveableObjectsBtnTemplate, transform);
            moveableObjectsBtnTransform.gameObject.SetActive(true);

            moveableObjectsBtnTransform.GetComponent<RectTransform>().anchoredPosition += new Vector2(index * 110, 0);
            moveableObjectsBtnTransform.Find("image").GetComponent<Image>().sprite = moveableObjectsTypeSO.sprite;

            moveableObjectsBtnTransform.GetComponent<Button>().onClick.AddListener(() => {
                moveableObjectsManager.SetActiveBuildingType(moveableObjectsTypeSO);
                Debug.Log(moveableObjectsTypeSO);
                UpdateSelectedVisual();
            });

            moveableObjectsBtnDictionary[moveableObjectsTypeSO] = moveableObjectsBtnTransform;
            index++;
        }


        UpdateSelectedVisual();
    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            MoveableObjectsSO NextMoveableObjectsType = NextBuildingTypeOnList();
            moveableObjectsManager.SetActiveBuildingType(NextMoveableObjectsType);
            UpdateSelectedVisual();
        }
    }
    private void UpdateSelectedVisual() {
        foreach (MoveableObjectsSO moveableObjectsTypeSO in moveableObjectsBtnDictionary.Keys) {
            moveableObjectsBtnDictionary[moveableObjectsTypeSO].Find("selected").gameObject.SetActive(false);
        }

        MoveableObjectsSO activeMoveableObjectsType = moveableObjectsManager.GetActiveMOType();
        moveableObjectsBtnDictionary[activeMoveableObjectsType].Find("selected").gameObject.SetActive(true);
    }

    private MoveableObjectsSO NextBuildingTypeOnList() {
        int indexCurrentBT = moveableObjectsTypeSOList.IndexOf(moveableObjectsManager.GetActiveMOType());
        if (indexCurrentBT == moveableObjectsTypeSOList.Count - 1)
            return moveableObjectsTypeSOList[0];
        else
            return moveableObjectsTypeSOList[indexCurrentBT + 1];
    }
}
