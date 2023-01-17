﻿using System;   
using System.Collections.Generic;
using UnityEngine;


namespace BuildingSystem {
    public class TInstantiableObjectSystem :MonoBehaviour, IPCInputSubscriber, ISwitchBuildingSubscriber {
        public static TInstantiableObjectSystem Instance { get; private set; }
        public bool IsBuildingMode {
            get {
                if (BuildingSystem.Instance != null)
                    return BuildingSystem.Instance.IsBuildingMode;
                else return false;
            }
        }
        [SerializeField] public GameObject MoveableObjectsInstancesParent;
        [SerializeField] public GameObject GridObjectsInstancesParent;

        public MovableObjectsManager MovableObjectsManager { get; private set; }
        public GridObjectsManager GridObjectsManager { get; private set; }

        public List<GridLevel> gridList;
        public Dictionary<Constants.InstantiableTypes, TInstantiableObjectsManager> Managers;
        public TInstantiableObjectsManager CurrentManager;

        //public event EventHandler OnSelectedChanged;
        //public event EventHandler OnObjectPlaced;


        private void Awake() {
            Instance = this;
            Managers = new Dictionary<Constants.InstantiableTypes, TInstantiableObjectsManager>();
            gridList = new List<GridLevel>();
            for (int i = 0; i < Constants.QNTYGRIDLEVELS; i++) {
                GridLevel grid = new GridLevel(i);
                gridList.Add(grid);
            }
        }

        private void Start() {



            MovableObjectsManager = MovableObjectsManager.Instance;
            GridObjectsManager = GridObjectsManager.Instance;
            BuildingSystem.Instance.OnKeyPressed += Subs_OnKeyPressed;
            //BuildingSystem.Instance.OnMouse0 += Subs_OnMouse0;
            //BuildingSystem.Instance.OnMouse1 += Subs_OnMouse1;
            BuildingSystem.Instance.OnMouseMid += Subs_OnMouseMid;
            //BuildingSystem.Instance.OnMouseScroll += Subs_OnMouseScroll;
            BuildingSystem.Instance.OnEnableSwitch += Subs_OnBuildingModeEnable;
            BuildingSystem.Instance.OnDisableSwitch += Subs_OnBuildingModeDisable;
        }


        private void SwitchManagers() {
            if (IsBuildingMode) {
                switch (CurrentManager.ManagedType) {
                    case Constants.InstantiableTypes.GridObjects:
                        if (Managers.ContainsKey(Constants.InstantiableTypes.MoveableObjects)) {
                            CurrentManager.DeactivateManager();
                            Managers[Constants.InstantiableTypes.MoveableObjects].ActivateManager();
                        }
                        break;

                    case Constants.InstantiableTypes.MoveableObjects:
                        if (Managers.ContainsKey(Constants.InstantiableTypes.GridObjects)) {
                            CurrentManager.DeactivateManager();
                            Managers[Constants.InstantiableTypes.GridObjects].ActivateManager();
                        }
                        break;

                }
                //Debug.Log("current manager name: " + Instance.CurrentManager.name);
                return;
            }
        }

        public void Subs_OnBuildingModeEnable(object sender, EventArgs eventArgs) {
            if (IsBuildingMode) return;
        }

        public void Subs_OnBuildingModeDisable(object sender, EventArgs eventArgs) {
            if (!IsBuildingMode) return;
        }

        public void Subs_OnKeyPressed(object sender, OnKeyPressedEventArgs keyPressedArgs) {
            if (IsBuildingMode) {
                if (keyPressedArgs.keyPressed == KeyCode.KeypadEnter) {
                    SwitchManagers();
                }
            }
        }

        public void Subs_OnMouse0(object sender, EventArgs eventArgs) {

        }

        public void Subs_OnMouse1(object sender, EventArgs eventArgs) {

        }

        public void Subs_OnMouseMid(object sender, EventArgs eventArgs) {
            if (IsBuildingMode) {
                SwitchManagers();
            }

        }

        public void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs eventArgs) {

        }
    }
}