using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovableObjectsTypeSelectUI : TInstantiableObjectsTypeSelectUI {
    public static MovableObjectsTypeSelectUI Instance { get; private set; }

    private MovableObjectsManager moveableObjectsManager;

    private Dictionary<MovableObjectsSO, Transform> moveableObjectsBtnDictionary;

    private void Start() {
        moveableObjectsManager = MovableObjectsManager.Instance;

        Transform moveableObjectsBtnTemplate = transform.Find("buildingButtonTemplate");
        moveableObjectsBtnTemplate.gameObject.SetActive(false);


        moveableObjectsBtnDictionary = new Dictionary<MovableObjectsSO, Transform>();

        //UpdateSelectedVisual();
    }

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        //if (Input.GetKeyDown(KeyCode.Tab)) {
        //    MovableObjectsSO NextMoveableObjectsType = NextBuildingTypeOnList();
        //    moveableObjectsManager.SetActiveBuildingType(NextMoveableObjectsType);
        //    UpdateSelectedVisual();
        //}
    }
    public override void UpdateSelectedVisual() {
        //foreach (MovableObjectsSO moveableObjectsTypeSO in moveableObjectsBtnDictionary.Keys) {
        //    moveableObjectsBtnDictionary[moveableObjectsTypeSO].Find("selected").gameObject.SetActive(false);
        //}

        //MovableObjectsSO activeMoveableObjectsType = moveableObjectsManager.GetInstanceableObjectSO();
        //moveableObjectsBtnDictionary[activeMoveableObjectsType].Find("selected").gameObject.SetActive(true);
    }

    private MovableObjectsSO NextBuildingTypeOnList() {
        //int indexCurrentBT = moveableObjectsManager.movableObjectsTypeSOList.IndexOf(moveableObjectsManager.GetInstanceableObjectSO());
        //if (indexCurrentBT == moveableObjectsManager.movableObjectsTypeSOList.Count - 1)
        //    return moveableObjectsManager.movableObjectsTypeSOList[0];
        //else
        //    return moveableObjectsManager.movableObjectsTypeSOList[indexCurrentBT + 1];
        return null;
    }
}
