using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BuildingSystem {
    public class TInstantiableObjectsTypeSelectUI :MonoBehaviour {
        public static TInstantiableObjectsTypeSelectUI Instance { get; private set; }

        [SerializeField] private GameObject BtnList;
        [SerializeField] private Transform BuildingButtonTemplate;
        private Dictionary<TInstantiableObjectSO, Transform> btnDictionary = new Dictionary<TInstantiableObjectSO, Transform>();

        private void Start() {

            Transform moveableObjectsBtnTemplate = transform.Find("buildingButtonTemplate");
            moveableObjectsBtnTemplate.gameObject.SetActive(false);

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
        public void UpdateSelectedVisual() {
            //foreach (MovableObjectsSO moveableObjectsTypeSO in moveableObjectsBtnDictionary.Keys) {
            //    moveableObjectsBtnDictionary[moveableObjectsTypeSO].Find("selected").gameObject.SetActive(false);
            //}

            //MovableObjectsSO activeMoveableObjectsType = moveableObjectsManager.GetInstanceableObjectSO();
            //moveableObjectsBtnDictionary[activeMoveableObjectsType].Find("selected").gameObject.SetActive(true);
        }

        public void ClearButtonList() {
            List<Transform> btnLister = new List<Transform>();
            foreach (TInstantiableObjectSO soObj in btnDictionary.Keys) {
                btnLister.Add(btnDictionary[soObj]);
            }
            Debug.Log($"count = {btnLister.Count}");
            btnDictionary.Clear();
            btnLister.ForEach(x => Destroy(x.gameObject));
        }

        public void ClearUpdateButtons(List<TInstantiableObjectSO> soList) {
            ClearButtonList();
            for (int index = 0; index < soList.Count; index++) {
                Transform buttonTransform = Instantiate(BuildingButtonTemplate, BtnList.transform);
                buttonTransform.gameObject.SetActive(true);
                buttonTransform.GetComponent<RectTransform>().position = new Vector2(index * 60 + 75, 0);
                buttonTransform.Find("image").GetComponent<Image>().sprite = soList[index].sprite;
                btnDictionary.Add(soList[index], buttonTransform);
            }
        }

        public void NextButton() {

        }

    }
}