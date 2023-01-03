using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace BuildingSystem {
    public class GridObjectsManager :TInstantiableObjectsManager, IPCInputSubscriber, ISwitchBuildingSubscriber {
        public static GridObjectsManager Instance { get; private set; }

        GhostGridObject ghostGridObject;
        GhostGridEdgeObject ghostGridEdgeObject;

        private List<GridXZ<GridObject>> gridList;
        private GridXZ<GridObject> selectedGrid;
        private Constants.Dir dir;

        private bool currentManager = false;

        [SerializeField] private LayerMask placedObjectEdgeColliderLayerMask;


        public override bool IsBuildingMode {
            get { return BuildingSystem.Instance.IsBuildingMode; }
        }
        public event EventHandler OnActiveGridLevelChanged;
        public event EventHandler OnSelectedChanged;
        public event EventHandler OnObjectPlaced;

        private void Awake() {
            Instance = this;
            ManagedType = Constants.InstantiableTypes.GridObjects;
            MouseClickAdd = MouseClickAddFunc;
            AddFromInfo = AddFromInfoFunc;
            dir = Constants.Dir.Down;
        }

        private void Start() {
            gridList = TInstantiableObjectSystem.Instance.gridList;
            selectedGrid = gridList[0];

            TInstantiableObjectSystem.Instance.Managers.Add(Constants.InstantiableTypes.GridObjects, Instance);
            TInstantiableObjectSystem.Instance.Managers.Add(Constants.InstantiableTypes.GridEdgeObjects, Instance);
            BuildingSystem.Instance.OnKeyPressed += Subs_OnKeyPressed;
            BuildingSystem.Instance.OnMouse0 += Subs_OnMouse0;
            BuildingSystem.Instance.OnMouse1 += Subs_OnMouse1;
            BuildingSystem.Instance.OnMouseMid += Subs_OnMouseMid;
            BuildingSystem.Instance.OnMouseScroll += Subs_OnMouseScroll;
            BuildingSystem.Instance.OnEnableSwitch += Subs_OnBuildingModeEnable;
            BuildingSystem.Instance.OnDisableSwitch += Subs_OnBuildingModeDisable;
            if (!TInstantiableObjectSystem.Instance.CurrentManager) {
                ActivateManager();
                //Debug.Log("current manager name: " + TInstantiableObjectSystem.Instance.CurrentManager.name);
            }

        }

        private void Update() {
            if (IsBuildingMode) {
                HandleGridSelectAutomatic();
            }
        }

        public override void ActivateManager() {
            if (IsBuildingMode) {
                TInstantiableObjectSystem.Instance.CurrentManager = Instance;
                if (ghostGridObject is null) {
                    ghostGridObject = GhostGridObject.Instance;
                }
                if (ghostGridEdgeObject is null) {
                    ghostGridEdgeObject = GhostGridEdgeObject.Instance;
                }
                if (currentSO is null) {
                    currentSO = Assets.Instance.gridObjectsTypeSOList[listCounter];
                }
                currentManager = true;
                ActivateGhostObject();
                TInstantiableObjectsTypeSelectUI.Instance.ClearUpdateButtons(Assets.Instance.gridObjectsTypeSOList);
            }
        }

        public override void DeactivateManager() {
            if (IsBuildingMode) {
                ghostGridObject?.Activation(false);
                ghostGridEdgeObject?.Activation(false);
                currentManager = false;
            }
        }

        private void AddGridObject(TInstantiableObjectSO objectsSO, Vector3 worldPosition, GridXZ<GridObject> targetGrid, 
                    Constants.Dir targetDir = Constants.Dir.Down) {
            if (Vector3.Distance(BuildingSystem.Instance.PlayerTransform.position, worldPosition) < Constants.MAXBUILDINGDISTANCE) {
                targetGrid.GetXZ(worldPosition, out int x, out int z);
                Vector2Int placedObjectOrigin = new Vector2Int(x, z);
                placedObjectOrigin = targetGrid.ValidateGridPosition(placedObjectOrigin);

                // Test Can Build
                List<Vector2Int> gridPositionList = GridXZ<GridObject>.GetGridPositionList(objectsSO.width, objectsSO.height, 
                    placedObjectOrigin, targetDir);
                bool canBuild = true;
                foreach (Vector2Int gridPosition in gridPositionList) {
                    if (!targetGrid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                        canBuild = false;
                        break;
                    }
                }
                if (canBuild) {
                    Vector2Int rotationOffset = objectsSO.GetRotationOffset(targetDir);
                    Vector3 placedObjectWorldPosition = targetGrid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y)
                        + new Vector3(rotationOffset.x, 0, rotationOffset.y) * targetGrid.GetCellSize();

                    GridObjectsInfo placedObject = GridObjectsInfo.Create(placedObjectWorldPosition, targetDir, objectsSO, 
                        TInstantiableObjectSystem.Instance.GridObjectsInstancesParent);

                    foreach (Vector2Int gridPosition in gridPositionList) {
                        targetGrid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                    }
                    OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void AddGridEdgeObject(TInstantiableObjectSO objectsSO, Vector3 worldPosition, Constants.Dir dir) {
            GridXZ<GridObject> targetGrid = gridList[GetGridLevel(worldPosition)];
            Vector2Int objCoodrinates = GetGridPosition(worldPosition, targetGrid);
            GridObject gridObject = targetGrid.GetGridObject(objCoodrinates.x, objCoodrinates.y);
            if (gridObject.gridObjectsInfo is object) {
                gridObject.gridObjectsInfo.PlaceEdge(dir, objectsSO, InstancesList);
            }
        }


        private void MouseClickAddFunc() {
            if (currentSO.instantiableType == Constants.InstantiableTypes.GridObjects) {
                Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
                AddGridObject(currentSO, mousePosition, selectedGrid, dir);
            }
            else if (currentSO.instantiableType == Constants.InstantiableTypes.GridEdgeObjects) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, placedObjectEdgeColliderLayerMask)) {
                    // Raycast Hit Edge Object
                    if (raycastHit.collider.TryGetComponent(out GridEdgeObjectsPosition floorEdgePosition)) {
                        if (raycastHit.collider.transform.parent.TryGetComponent(out GridObjectsInfo floorPlacedObject)) {
                            // Found parent FloorPlacedObject
                            floorPlacedObject.PlaceEdge(floorEdgePosition.edge, currentSO, InstancesList);
                        }
                    }
                }
            }
        }


        private void AddFromInfoFunc(InstanceInfo bInfo) {
            TInstantiableObjectSO foundSOTypeFromSerialized = Assets.Instance.gridObjectsTypeSOList
                .FirstOrDefault(d => d.nameString == bInfo.SOName);

            if (foundSOTypeFromSerialized is null) {
                Debug.Log("GridObjectsSO: " + bInfo.SOName + " was NOT serialized in this buildVer.");
                return;
            }
            TInstantiableObjectSO typeSO = foundSOTypeFromSerialized;
            Vector3 worldPosition = bInfo.position.ToVector3();

            if (typeSO.instantiableType is Constants.InstantiableTypes.GridObjects) {
                AddGridObject(typeSO, worldPosition, gridList[GetGridLevel(worldPosition)], bInfo.dir);
            }
            else if (typeSO.instantiableType is Constants.InstantiableTypes.GridEdgeObjects) {
                AddGridEdgeObject(typeSO, worldPosition, bInfo.dir);
            }
            return;
        }

        public override void Subs_OnBuildingModeEnable(object sender, EventArgs eventArgs) {
            if (IsBuildingMode) return;

        }

        public override void Subs_OnBuildingModeDisable(object sender, EventArgs eventArgs) {
            if (!IsBuildingMode) return;

        }


        public override void Subs_OnKeyPressed(object sender, OnKeyPressedEventArgs keyPressedArgs) {
            if (IsBuildingMode && currentManager) {
                if (keyPressedArgs.keyPressed == KeyCode.R) {
                    dir = TInstantiableObjectSO.GetNextDir(dir);
                }
                else if (keyPressedArgs.keyPressed == KeyCode.Tab) {
                    NextSO();
                }
            }
        }

        public override void Subs_OnMouse0(object sender, EventArgs e) {
            if (IsBuildingMode && currentManager) {
                MouseClickAdd();
            }
        }

        public override void Subs_OnMouse1(object sender, EventArgs e) {
            if (IsBuildingMode && currentManager) {

            }
        }

        public override void Subs_OnMouseMid(object sender, EventArgs e) {
            if (IsBuildingMode && currentManager) {

            }
        }

        public override void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs e) {
            if (IsBuildingMode && currentManager) {

            }
        }


        private void HandleGridSelectAutomatic() {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            int newGridIndex = GetGridLevel(mousePosition);
            selectedGrid = gridList[newGridIndex];
            OnActiveGridLevelChanged?.Invoke(this, EventArgs.Empty);
        }


        public int GetGridLevel(Vector3 worldPosition) {
            float gridHeight = Constants.GRIDVERTICALSIZE;
            return Mathf.Clamp(Mathf.RoundToInt(worldPosition.y / gridHeight), 0, gridList.Count - 1);
        }

        public Vector2Int GetGridPosition(Vector3 worldPosition, GridXZ<GridObject> targetGrid = null) {
            if (targetGrid is null) {
                targetGrid = selectedGrid;
            }
            targetGrid.GetXZ(worldPosition, out int x, out int z);
            return new Vector2Int(x, z);
        }

        public Vector3 GetWorldPosition(Vector2Int gridPosition, GridXZ<GridObject> targetGrid = null) {
            if (targetGrid is null) {
                targetGrid = selectedGrid;
            }
            return targetGrid.GetWorldPosition(gridPosition.x, gridPosition.y);
        }

        public GridObject GetGridObject(Vector2Int gridPosition, GridXZ<GridObject> targetGrid = null) {
            if (targetGrid is null) {
                targetGrid = selectedGrid;
            }
            return targetGrid.GetGridObject(gridPosition.x, gridPosition.y);
        }

        public GridObject GetGridObject(Vector3 worldPosition, GridXZ<GridObject> targetGrid = null) {
            if (targetGrid is null) {
                targetGrid = selectedGrid;
            }
            return targetGrid.GetGridObject(worldPosition);
        }

        public bool IsValidGridPosition(Vector2Int gridPosition, GridXZ<GridObject> targetGrid = null) {
            if (targetGrid is null) {
                targetGrid = selectedGrid;
            }
            return targetGrid.IsValidGridPosition(gridPosition);
        }

        public GridEdgeObjectsPosition GetMouseFloorEdgePosition() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, placedObjectEdgeColliderLayerMask)) {
                // Raycast Hit Edge Object
                if (raycastHit.collider.TryGetComponent(out GridEdgeObjectsPosition floorEdgePosition)) {
                    return floorEdgePosition;
                }
            }

            return null;
        }

        public Vector3 GetMouseWorldSnappedPosition(GridXZ<GridObject> targetGrid = null) {
            if (targetGrid is null) {
                targetGrid = selectedGrid;
            }
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            targetGrid.GetXZ(mousePosition, out int x, out int z);

            if (currentSO is object) {
                Vector2Int rotationOffset = currentSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
                return placedObjectWorldPosition;
            }
            else {
                return mousePosition;
            }
        }

        public Quaternion GetPlacedObjectRotation() {
            if (currentSO is object) {
                return Quaternion.Euler(0, TInstantiableObjectSO.GetRotationAngle(dir), 0);
            }
            else {
                return Quaternion.identity;
            }
        }

        public TInstantiableObjectSO GetInstanceableObjectSO() {
            return currentSO;
        }

        public int GetActiveGridLevel() {
            return gridList.IndexOf(selectedGrid);
        }

        public void ActivateGhostObject() {
            if (currentSO?.instantiableType == Constants.InstantiableTypes.GridObjects) {
                ghostGridEdgeObject.Activation(false);
                ghostGridObject.Activation();
            }
            else if (currentSO?.instantiableType == Constants.InstantiableTypes.GridEdgeObjects) {
                ghostGridObject.Activation(false);
                ghostGridEdgeObject.Activation();
            }
        }

        public void NextSO() {
            if (currentSO is null) {
                currentSO = Assets.Instance.gridObjectsTypeSOList[listCounter];
                ActivateGhostObject();
            }
            else {
                Constants.InstantiableTypes pastType = Assets.Instance.gridObjectsTypeSOList[listCounter].instantiableType;
                if (listCounter < Assets.Instance.gridObjectsTypeSOList.Count - 1) {
                    listCounter++;
                }
                else {
                    listCounter = 0;
                }
                currentSO = Assets.Instance.gridObjectsTypeSOList[listCounter];
                if (currentSO.instantiableType != pastType) {
                    ActivateGhostObject();
                }
            }
            OnSelectedChanged.Invoke(this, EventArgs.Empty);
        }
    }
}