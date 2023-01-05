using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System;

namespace BuildingSystem {
    public class GridlessLoader :MonoBehaviour, ISwitchBuildingSubscriber, IPCInputSubscriber {
        public static GridlessLoader Instance { get; private set; }

        public bool IsBuildingMode {
            get { return BuildingSystem.Instance.IsBuildingMode; }
        }

        [SerializeField] public GameObject MoveableObjectsInstancesParent;
        [SerializeField] public GameObject GridObjectsInstancesParent;


        private void Awake() {
            Instance = this;
        }

        private void Start() {
            BuildingSystem.Instance.OnKeyPressed += Subs_OnKeyPressed;
            BuildingSystem.Instance.OnMouse0 += Subs_OnMouse0;
            BuildingSystem.Instance.OnMouse1 += Subs_OnMouse1;
            BuildingSystem.Instance.OnMouseMid += Subs_OnMouseMid;
            BuildingSystem.Instance.OnMouseScroll += Subs_OnMouseScroll;
            BuildingSystem.Instance.OnEnableSwitch += Subs_OnBuildingModeEnable;
            BuildingSystem.Instance.OnDisableSwitch += Subs_OnBuildingModeDisable;
        }

        public void Subs_OnBuildingModeEnable(object sender, EventArgs eventArgs) {
            if (IsBuildingMode) return;
        }

        public void Subs_OnBuildingModeDisable(object sender, EventArgs eventArgs) {
            if (!IsBuildingMode) return;
        }

        private TInstantiableObjectSO FindInstanceableObjectSO(InstanceInfo instanceInfo) =>
            Assets.Instance.gridObjectsTypeSOList.FirstOrDefault(d => d.nameString == instanceInfo.SOName);

        private void LoadGameObjects(SaveObject loaded) {
            InstanceInfo[] listBInfo = loaded.playerCreatedScenario.moveableObjects.placedMoveableInfo;
            foreach (InstanceInfo iInfo in listBInfo) {
                TInstantiableObjectSO foundSO = Assets.Instance.movableObjectsTypeSOList
                                                    .FirstOrDefault(d => d.nameString == iInfo.SOName);
                if ((iInfo != null) && (foundSO != null)){
                    TInstantiableObjectInfo.Create<TInstantiableObjectSO, MovableObjectsInfo>(iInfo, foundSO, MoveableObjectsInstancesParent.transform);
                }
            }
            listBInfo = loaded.playerCreatedScenario.gridObjects.placedGridInfo;
            foreach (InstanceInfo iInfo in listBInfo) {
                TInstantiableObjectSO foundSO = Assets.Instance.gridObjectsTypeSOList
                                                    .FirstOrDefault(d => d.nameString == iInfo.SOName);
                if ((iInfo != null) && (foundSO != null)) {
                    TInstantiableObjectInfo.Create<TInstantiableObjectSO, GridObjectsInfo>(iInfo, foundSO, GridObjectsInstancesParent.transform);
                }
            }
            return;
        }
        private void LoadGridless() {
            string saveString = BuildingJSONSaveSystem.Load();
            if (saveString != null) {
                SaveObject savedObject = JsonConvert.DeserializeObject<SaveObject>(saveString);
                //Debug.Log("Loaded: " + JsonConvert.SerializeObject(savedObject));
                LoadGameObjects(savedObject);
            }
            else {
                //Debug.Log("No save");
            }
        }

        public void Subs_OnKeyPressed(object sender, OnKeyPressedEventArgs keyPressedArgs) {
            if (!IsBuildingMode) {
                if (keyPressedArgs.keyPressed == BuildingSystem.Instance.GridlessLoadKey) {
                    //Debug.Log($"KeyCode {keyPressedArgs.keyPressed}");
                    LoadGridless();
                }
            }
        }

        public void Subs_OnMouse0(object sender, EventArgs e) {}

        public void Subs_OnMouse1(object sender, EventArgs e) {}

        public void Subs_OnMouseMid(object sender, EventArgs e) {}

        public void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs e) {}
    }
}