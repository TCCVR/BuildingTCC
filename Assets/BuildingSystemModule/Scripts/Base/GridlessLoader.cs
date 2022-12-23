using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

namespace BuildingSystem {
    public class GridlessLoader :MonoBehaviour {
        public static GridlessLoader Instance { get; private set; }


        [SerializeField] public GameObject MoveableObjectsInstancesParent;
        [SerializeField] public GameObject GridObjectsInstancesParent;
        private List<TInstantiableObjectSO> gridObjectsSOList;
        private List<TInstantiableObjectSO> movableObjectsTypeSOList;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            gridObjectsSOList = Assets.Instance.gridObjectsTypeSOList;
            movableObjectsTypeSOList = Assets.Instance.movableObjectsTypeSOList;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F3)) {
                Debug.Log("(Input.GetKeyDown(KeyCode.F3))");
                LoadGridless();
            }
        }

        private TInstantiableObjectSO FindInstanceableObjectSO(InstanceInfo instanceInfo) =>
            gridObjectsSOList.FirstOrDefault(d => d.nameString == instanceInfo.SOName);

        private void LoadGameObjects(SaveObject loaded) {
            InstanceInfo[] listBInfo = loaded.playerCreatedScenario.moveableObjects.placedMoveableInfo;
            foreach (InstanceInfo iInfo in listBInfo) {
                TInstantiableObjectSO foundSO = FindInstanceableObjectSO(iInfo);
                if ((iInfo is null) && (foundSO is object)){
                    TInstantiableObjectInfo.Create<TInstantiableObjectSO, MovableObjectsInfo>(iInfo, foundSO, MoveableObjectsInstancesParent.transform);
                }
            }
            listBInfo = loaded.playerCreatedScenario.gridObjects.placedGridInfo;
            foreach (InstanceInfo iInfo in listBInfo) {
                TInstantiableObjectSO foundSO = FindInstanceableObjectSO(iInfo);
                if ((iInfo is null) && (foundSO is object)) {
                    TInstantiableObjectInfo.Create<TInstantiableObjectSO, GridObjectsInfo>(iInfo, foundSO, GridObjectsInstancesParent.transform);
                }
            }
            return;
        }
        private void LoadGridless() {
            string saveString = SaveSystem.Load();
            if (saveString != null) {
                SaveObject savedObject = JsonConvert.DeserializeObject<SaveObject>(saveString);
                Debug.Log("Loaded: " + JsonConvert.SerializeObject(savedObject));
                LoadGameObjects(savedObject);
            }
            else {
                Debug.Log("No save");
            }
        }
    }
}