using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace BuildingSystem {
    public class GridObjectsManager :TInstantiableObjectsManager, IPCInputSubscriber, ISwitchBuildingSubscriber {
        public static GridObjectsManager Instance { get; private set; }

        GhostGridObject ghostGridObject;
        GhostGridEdgeObject ghostGridEdgeObject;

        private List<GridLevel> gridList;
        private GridLevel selectedGrid;
        private BuildingSystemConstants.Dir dir;

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
            ManagedType = BuildingSystemConstants.InstantiableTypes.GridObjects;
            MouseClickAdd = MouseClickAddFunc;
            AddFromInfo = AddFromInfoFunc;
            dir = BuildingSystemConstants.Dir.Down;
        }

        private void Start() {
            gridList = TInstantiableObjectSystem.Instance.gridList;
            selectedGrid = gridList[0];

            TInstantiableObjectSystem.Instance.Managers.Add(BuildingSystemConstants.InstantiableTypes.GridObjects, Instance);
            TInstantiableObjectSystem.Instance.Managers.Add(BuildingSystemConstants.InstantiableTypes.GridEdgeObjects, Instance);
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

        private void AddGridObject(TInstantiableObjectSO objectsSO, Vector3 worldPosition, GridLevel targetGrid, 
                    BuildingSystemConstants.Dir targetDir = BuildingSystemConstants.Dir.Down) {
            if (Vector3.Distance(BuildingSystem.Instance.PlayerTransform.position, worldPosition) < BuildingSystemConstants.MAXBUILDINGDISTANCE) {
                Vector2Int placedObjectOrigin = GridLevel.PlaneCoordinatesOf(worldPosition);

                // Test Can Build
                List<Vector2Int> unitsOccupied = GridLevel.CoordinatesListOf(objectsSO.width, objectsSO.depth, 
                    placedObjectOrigin, targetDir);
                bool canBuild = true;
                foreach (Vector2Int coordinates in unitsOccupied) {
                    if (targetGrid[coordinates.x, coordinates.y] != default) {
                        canBuild = false;
                        break;
                    }
                }
                if (canBuild) {
                    Vector2Int rotationOffset = objectsSO.GetRotationOffset(targetDir);
                    Vector3 placedObjectWorldPosition = targetGrid.GetWorldPosition(placedObjectOrigin)
                        + new Vector3(rotationOffset.x, 0, rotationOffset.y) * BuildingSystemConstants.UNITSIZE;

                    GridObjectsInfo placedObject = GridObjectsInfo.Create(placedObjectWorldPosition, targetDir, objectsSO, 
                        TInstantiableObjectSystem.Instance.GridObjectsInstancesParent);

                    foreach (Vector2Int coordinates in unitsOccupied) {
                        targetGrid[coordinates.x, coordinates.y] = placedObject;
                    }
                    OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void AddGridEdgeObject(TInstantiableObjectSO objectsSO, Vector3 worldPosition, BuildingSystemConstants.Dir dir) {
            GridLevel targetGrid = gridList[GetGridLevel(worldPosition)];
            Vector2Int objCoodrinates = GridLevel.PlaneCoordinatesOf(worldPosition);
            GridObjectsInfo gridObject = targetGrid[objCoodrinates.x, objCoodrinates.y];
            if (gridObject != default) {
                gridObject.PlaceEdge(dir, objectsSO, InstancesList);
            }
        }


        private void MouseClickAddFunc() {
            if (currentSO.instantiableType == BuildingSystemConstants.InstantiableTypes.GridObjects) {
                Vector3 mousePosition = RaycastPoint.PointPosition;
                AddGridObject(currentSO, mousePosition, selectedGrid, dir);
            }
            else if (currentSO.instantiableType == BuildingSystemConstants.InstantiableTypes.GridEdgeObjects) {
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
                //Debug.Log("GridObjectsSO: " + bInfo.SOName + " was NOT serialized in this buildVer.");
                return;
            }
            TInstantiableObjectSO typeSO = foundSOTypeFromSerialized;
            Vector3 worldPosition = bInfo.position.ToVector3();

            if (typeSO.instantiableType is BuildingSystemConstants.InstantiableTypes.GridObjects) {
                AddGridObject(typeSO, worldPosition, gridList[GetGridLevel(worldPosition)], bInfo.dir);
            }
            else if (typeSO.instantiableType is BuildingSystemConstants.InstantiableTypes.GridEdgeObjects) {
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
            Vector3 mousePosition = RaycastPoint.PointPosition;
            int newGridIndex = GetGridLevel(mousePosition);
            selectedGrid = gridList[newGridIndex];
            OnActiveGridLevelChanged?.Invoke(this, EventArgs.Empty);
        }


        public int GetGridLevel(Vector3 worldPosition) {
            float gridHeight = BuildingSystemConstants.GRIDHEIGHT;
            return Mathf.Clamp(Mathf.RoundToInt(worldPosition.y / gridHeight), 0, gridList.Count - 1);
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

        public Vector3 GetMouseWorldSnappedPosition() {
            Vector3 mousePosition = RaycastPoint.PointPosition;
            Vector2Int coordinates = GridLevel.PlaneCoordinatesOf(mousePosition);
            if (currentSO is object) {
                Vector2Int rotationOffset = currentSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = selectedGrid.GetWorldPosition(coordinates) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * BuildingSystemConstants.UNITSIZE;
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
        public void ActivateGhostObject() {
            if (currentSO?.instantiableType == BuildingSystemConstants.InstantiableTypes.GridObjects) {
                ghostGridEdgeObject.Activation(false);
                ghostGridObject.Activation();
            }
            else if (currentSO?.instantiableType == BuildingSystemConstants.InstantiableTypes.GridEdgeObjects) {
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
                BuildingSystemConstants.InstantiableTypes pastType = Assets.Instance.gridObjectsTypeSOList[listCounter].instantiableType;
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