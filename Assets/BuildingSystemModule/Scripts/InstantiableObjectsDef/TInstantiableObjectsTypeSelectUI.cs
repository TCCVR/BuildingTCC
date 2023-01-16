using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BuildingSystem {
    public class TInstantiableObjectsTypeSelectUI :MonoBehaviour, ISwitchBuildingSubscriber {
        public static TInstantiableObjectsTypeSelectUI Instance { get; private set; }

        [SerializeField] private GameObject BtnList;
        [SerializeField] private Transform BuildingButtonTemplate;
        private Dictionary<TInstantiableObjectSO, Transform> btnDictionary = new Dictionary<TInstantiableObjectSO, Transform>();

        public bool IsBuildingMode {
            get {
                if (BuildingSystem.Instance != null)
                    return BuildingSystem.Instance.IsBuildingMode;
                else return false;
            }
        }
        private void Awake() {
            Instance = this;
            Transform moveableObjectsBtnTemplate = transform.Find("buildingButtonTemplate");
            moveableObjectsBtnTemplate.gameObject.SetActive(false);
        }

        private void Start() {
            BuildingSystem.Instance.OnEnableSwitch += Subs_OnBuildingModeEnable;
            BuildingSystem.Instance.OnDisableSwitch += Subs_OnBuildingModeDisable;
        }

        public void ClearButtonList() {
            if (!IsBuildingMode) return;
            foreach (Transform soObj in btnDictionary.Values)
                soObj.gameObject.SetActive(false);
        }

        public void ClearUpdateButtons(List<TInstantiableObjectSO> soList) {
            if (!IsBuildingMode) return;
            ClearButtonList();
            for (int index = 0; index < soList.Count; index++) {
                if (btnDictionary.ContainsKey(soList[index])) {
                    btnDictionary[soList[index]].gameObject.SetActive(true);
                    continue; 
                }
                Transform buttonTransform = Instantiate(BuildingButtonTemplate, BtnList.transform);
                buttonTransform.gameObject.SetActive(true);
                buttonTransform.GetComponent<RectTransform>().position = new Vector2(index * 60 + 75, 0);
                buttonTransform.Find("image").GetComponent<Image>().sprite = soList[index].sprite;
                btnDictionary.Add(soList[index], buttonTransform);
            }
        }

        public void NextButton() {

        }

        public void Subs_OnBuildingModeEnable(object sender, EventArgs eventArgs) {
            if (IsBuildingMode) return;
            ClearUpdateButtons(Assets.Instance.gridObjectsTypeSOList);
        }

        public void Subs_OnBuildingModeDisable(object sender, EventArgs eventArgs) {
            if (!IsBuildingMode) return;
            ClearButtonList();
        }

    }
}