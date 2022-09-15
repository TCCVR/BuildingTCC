using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovableObjectsTypeSelectUI : MonoBehaviour {
    [SerializeField] private GameObject ManagerObject;
    private MovableObjectsManager moveableObjectsManager;
    public static MovableObjectsTypeSelectUI Instance { get; private set; }

    private Dictionary<MovableObjectsSO, Transform> moveableObjectsBtnDictionary;
    private void Start() {
        moveableObjectsManager = MovableObjectsManager.Instance;

        Transform moveableObjectsBtnTemplate = transform.Find("buildingButtonTemplate");
        moveableObjectsBtnTemplate.gameObject.SetActive(false);


        moveableObjectsBtnDictionary = new Dictionary<MovableObjectsSO, Transform>();
        int index = 0;
        
        foreach(MovableObjectsSO moveableObjectsTypeSO in moveableObjectsManager.movableObjectsTypeSOList) {
            
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

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            MovableObjectsSO NextMoveableObjectsType = NextBuildingTypeOnList();
            moveableObjectsManager.SetActiveBuildingType(NextMoveableObjectsType);
            UpdateSelectedVisual();
        }
    }
    private void UpdateSelectedVisual() {
        foreach (MovableObjectsSO moveableObjectsTypeSO in moveableObjectsBtnDictionary.Keys) {
            moveableObjectsBtnDictionary[moveableObjectsTypeSO].Find("selected").gameObject.SetActive(false);
        }

        MovableObjectsSO activeMoveableObjectsType = moveableObjectsManager.GetActiveMOType();
        moveableObjectsBtnDictionary[activeMoveableObjectsType].Find("selected").gameObject.SetActive(true);
    }

    private MovableObjectsSO NextBuildingTypeOnList() {
        int indexCurrentBT = moveableObjectsManager.movableObjectsTypeSOList.IndexOf(moveableObjectsManager.GetActiveMOType());
        if (indexCurrentBT == moveableObjectsManager.movableObjectsTypeSOList.Count - 1)
            return moveableObjectsManager.movableObjectsTypeSOList[0];
        else
            return moveableObjectsManager.movableObjectsTypeSOList[indexCurrentBT + 1];
    }
}
