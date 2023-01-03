using System;   
using System.Collections.Generic;
using UnityEngine;


namespace BuildingSystem {
    public class TInstantiableObjectSystem :MonoBehaviour, IPCInputSubscriber, ISwitchBuildingSubscriber {
        public static TInstantiableObjectSystem Instance { get; private set; }
        public bool IsBuildingMode {
            get { return BuildingSystem.Instance.IsBuildingMode; }
        }
        [SerializeField] public GameObject MoveableObjectsInstancesParent;
        [SerializeField] public GameObject GridObjectsInstancesParent;

        public MovableObjectsManager MovableObjectsManager { get; private set; }
        public GridObjectsManager GridObjectsManager { get; private set; }

        public List<GridXZ<GridObject>> gridList;
        public Dictionary<Constants.InstantiableTypes, TInstantiableObjectsManager> Managers;
        public TInstantiableObjectsManager CurrentManager;

        //public event EventHandler OnSelectedChanged;
        //public event EventHandler OnObjectPlaced;


        private void Awake() {
            Instance = this;
            Managers = new Dictionary<Constants.InstantiableTypes, TInstantiableObjectsManager>();
            int gridWidth = Constants.GRIDWIDTH;
            int gridHeight = Constants.GRIDHEIGHT;
            float cellSize = Constants.CELLSIZE;
            gridList = new List<GridXZ<GridObject>>();
            int gridVerticalCount = Constants.GRIDVERTICALCOUNT;
            float gridVerticalSize = Constants.GRIDVERTICALSIZE;
            for (int i = 0; i < gridVerticalCount; i++) {
                GridXZ<GridObject> grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, gridVerticalSize * i, 0), (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));
                gridList.Add(grid);
            }
        }

        private void Start() {
            MovableObjectsManager = MovableObjectsManager.Instance;
            GridObjectsManager = GridObjectsManager.Instance;
            BuildingSystem.Instance.OnKeyPressed += Subs_OnKeyPressed;
            //BuildingSystem.Instance.OnMouse0 += Subs_OnMouse0;
            //BuildingSystem.Instance.OnMouse1 += Subs_OnMouse1;
            //BuildingSystem.Instance.OnMouseMid += Subs_OnMouseMid;
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
                Debug.Log("current manager name: " + Instance.CurrentManager.name);
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

        }

        public void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs eventArgs) {

        }
    }
}