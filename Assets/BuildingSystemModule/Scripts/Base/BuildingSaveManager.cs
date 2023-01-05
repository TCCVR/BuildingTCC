﻿using UnityEngine;
using Newtonsoft.Json;
using System;

namespace BuildingSystem {
    public class BuildingSaveManager :MonoBehaviour, IPCInputSubscriber {

        public static BuildingSaveManager Instance { get; private set; }

        private void Awake() {
            Instance = this;
            BuildingJSONSaveSystem.Init();
        }

        private void Update() {
            if (Input.GetKeyDown(BuildingSystem.Instance.SaveKey)) {
                Save();
            }

            if (Input.GetKeyDown(BuildingSystem.Instance.BuildingModeLoadKey)) {
                LoadToGrid();
            }

        }
        private void Start() {
            BuildingSystem.Instance.OnKeyPressed += Subs_OnKeyPressed;
        }

        private void Save() {
            MovableObjectsInfo[] movableObjectsToSave = TInstantiableObjectSystem.Instance.MoveableObjectsInstancesParent.GetComponentsInChildren<MovableObjectsInfo>();
            GridObjectsInfo[] gridObjectsToSave = TInstantiableObjectSystem.Instance.GridObjectsInstancesParent.GetComponentsInChildren<GridObjectsInfo>();
            SaveObject saveObject = new SaveObject();
            saveObject.playerCreatedScenario = new playerCreatedScenario();
            saveObject.playerCreatedScenario.moveableObjects = new MoveableObjects();
            saveObject.playerCreatedScenario.moveableObjects.placedMoveableInfo = new InstanceInfo[movableObjectsToSave.Length];
            InstanceInfo[] listMOInfo = saveObject.playerCreatedScenario.moveableObjects.placedMoveableInfo;
            for (int iC1 = 0; iC1 < movableObjectsToSave.Length; iC1++) {
                listMOInfo[iC1] = movableObjectsToSave[iC1].instanceInfo;
            }
            saveObject.playerCreatedScenario.gridObjects = new GridObjects();
            saveObject.playerCreatedScenario.gridObjects.placedGridInfo = new InstanceInfo[gridObjectsToSave.Length];
            InstanceInfo[] listGOInfo = saveObject.playerCreatedScenario.gridObjects.placedGridInfo;
            for (int iC1 = 0; iC1 < gridObjectsToSave.Length; iC1++) {
                listGOInfo[iC1] = gridObjectsToSave[iC1].instanceInfo;
            }

            BuildingJSONSaveSystem.Save(JsonConvert.SerializeObject(saveObject));
            //Debug.Log("listBInfo: " + JsonConvert.SerializeObject(saveObject));

        }

        private void LoadGameObjectsIntoGrid(SaveObject loaded) {
            InstanceInfo[] listBInfo = loaded.playerCreatedScenario.moveableObjects.placedMoveableInfo;
            foreach (InstanceInfo iInfo in listBInfo) {
                TInstantiableObjectSystem.Instance.MovableObjectsManager.AddFromInfo(iInfo);
            }
            listBInfo = loaded.playerCreatedScenario.gridObjects.placedGridInfo;
            foreach (InstanceInfo iInfo in listBInfo) {
                if (iInfo.SOType == BuildingSystemConstants.InstantiableTypes.GridObjects) {
                    TInstantiableObjectSystem.Instance.GridObjectsManager.AddFromInfo(iInfo);
                }
                else if (iInfo.SOType == BuildingSystemConstants.InstantiableTypes.GridEdgeObjects) {
                    TInstantiableObjectSystem.Instance.GridObjectsManager.AddFromInfo(iInfo);
                }
            }
            return;
        }


        private void LoadToGrid() {
            string saveString = BuildingJSONSaveSystem.Load();
            if (saveString != null) {
                SaveObject savedObject = JsonConvert.DeserializeObject<SaveObject>(saveString);
                //Debug.Log("Loaded: " + JsonConvert.SerializeObject(savedObject));
                LoadGameObjectsIntoGrid(savedObject);
            }
            else {
                //Debug.Log("No save");
            }
        }

        public void Subs_OnKeyPressed(object sender, OnKeyPressedEventArgs keyPressedArgs) {
            if (BuildingSystem.Instance.IsBuildingMode) {
                if (keyPressedArgs.keyPressed == BuildingSystem.Instance.SaveKey) {
                    Save();
                }

                if (keyPressedArgs.keyPressed == BuildingSystem.Instance.BuildingModeLoadKey) {
                    LoadToGrid();
                }
            }
        }

        public void Subs_OnMouse0(object sender, EventArgs eventArgs) {}

        public void Subs_OnMouse1(object sender, EventArgs eventArgs) {}

        public void Subs_OnMouseMid(object sender, EventArgs eventArgs) {}

        public void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs eventArgs) {}
    }
}