using System;   
using System.Collections.Generic;
using UnityEngine;


namespace BuildingSystem {
    public class TInstantiableObjectSystem :MonoBehaviour, IPCInputHandler {

        public static TInstantiableObjectSystem Instance { get; private set; }
        [SerializeField] public Transform playerTransform;
        [SerializeField] public GameObject MoveableObjectsInstancesParent;
        [SerializeField] public GameObject GridObjectsInstancesParent;

        public MovableObjectsManager MovableObjectsManager { get; private set; }
        public GridObjectsManager GridObjectsManager { get; private set; }

        public List<GridXZ<GridObject>> gridList;
        public Dictionary<Constants.InstantiableTypes, TInstantiableObjectsManager> Managers;
        public TInstantiableObjectsManager CurrentManager;



        public event EventHandler<OnKeyPressedEventArgs> OnKeyPressed;
        public event EventHandler<OnMouseScrollEventArgs> OnMouseScroll;
        public event EventHandler OnMouse0;
        public event EventHandler OnMouse1;
        public event EventHandler OnMouseMid;

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
        }


        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                OnMouse0.Invoke(this, EventArgs.Empty);
            }
            else if (Input.GetMouseButtonDown(1)) {
                OnMouse1.Invoke(this, EventArgs.Empty);
            }
            else if (Input.GetMouseButtonDown(2)) {
                OnMouseMid.Invoke(this, EventArgs.Empty);
            }
            else if (Input.mouseScrollDelta != Vector2.zero) {
                OnMouseScroll.Invoke(this, new OnMouseScrollEventArgs { scrollDir = Input.mouseScrollDelta });
            }
            else if (Input.anyKeyDown) {
                if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
                    SwitchManagers();
                }
                else {
                    foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
                        if (!Constants.USEDKEYS.Contains(key)) {
                            if (Input.GetKeyDown(key)) {
                                OnKeyPressed(this, new OnKeyPressedEventArgs { keyPressed = key });
                                break;
                            };
                        };
                    };
                }
            }
        }

        private void SwitchManagers() {
            switch (CurrentManager.managedType) {
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
}