using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridObjectsManager :TInstantiableObjectsManager {
    public static GridObjectsManager Instance { get; private set; }

    GhostGridObject ghostGridObject;
    GhostGridEdgeObject ghostGridEdgeObject;


    [SerializeField] public GameObject gridInstancesList;
    private List<GridObjectsSO> gridObjectsSOList;
    [SerializeField] private GridObjectsSO currentSO;
    private int listCounter = 0;
    

    private List<GridXZ<GridObject>> gridList;
    private GridXZ<GridObject> selectedGrid;
    private Constants.Dir dir;

    private bool currentManager = false;

    [SerializeField] private LayerMask placedObjectEdgeColliderLayerMask;


    public event EventHandler OnActiveGridLevelChanged;
    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    private void Awake() {
        Instance = this;
        managedType = Constants.InstantiableTypes.GridObjects;
        MouseClickAdd = MouseClickAddFunc;
        AddFromInfo = AddFromInfoFunc;
        dir = Constants.Dir.Up;

    }

    private void Start() {
        gridList = GameHandler.Instance.gridList;
        selectedGrid = gridList[0];
        gridObjectsSOList = GameAssets.Instance.gridObjectsTypeSOList;

        TInstantiableObjectSystem.Instance.Managers.Add(Constants.InstantiableTypes.GridObjects, Instance);
        TInstantiableObjectSystem.Instance.Managers.Add(Constants.InstantiableTypes.GridEdgeObjects, Instance);
        TInstantiableObjectSystem.Instance.OnKeyPressed += OnKeyPressed;
        TInstantiableObjectSystem.Instance.OnMouse0 += OnMouse0;
        TInstantiableObjectSystem.Instance.OnMouse1 += OnMouse1;
        TInstantiableObjectSystem.Instance.OnMouseMid += OnMouseMid;
        TInstantiableObjectSystem.Instance.OnMouseScroll += OnMouseScroll;
        if (!TInstantiableObjectSystem.Instance.CurrentManager) {
            ActivateManager();
            Debug.Log("current manager name: " + TInstantiableObjectSystem.Instance.CurrentManager.name);
        }

    }

    private void Update() {
        HandleGridSelectAutomatic();
    }

    public override void ActivateManager() {
        TInstantiableObjectSystem.Instance.CurrentManager = Instance;
        if (ghostGridObject is null) {
            ghostGridObject = GhostGridObject.Instance;
        }
        if (ghostGridEdgeObject is null) {
            ghostGridEdgeObject = GhostGridEdgeObject.Instance;
        }
        if (currentSO is null) {
            currentSO = gridObjectsSOList[listCounter];
        }
        currentManager = true;
        ActivateGhostObject();
    }

    public override void DeactivateManager() {
        ghostGridObject?.Activation(false);
        ghostGridEdgeObject?.Activation(false);
        currentManager = false;
    }

    private void AddGridObject(GridObjectsSO objectsSO, Vector3 worldPosition, GridXZ<GridObject> targetGrid, Constants.Dir targetDir = Constants.Dir.Down) {
        if (Vector3.Distance(TInstantiableObjectSystem.Instance.playerTransform.position, worldPosition) < Constants.MAXBUILDINGDISTANCE) {
            targetGrid.GetXZ(worldPosition, out int x, out int z);
            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = targetGrid.ValidateGridPosition(placedObjectOrigin);

            // Test Can Build
            List<Vector2Int> gridPositionList = objectsSO.GetGridPositionList(placedObjectOrigin, targetDir);
            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList) {
                if (!targetGrid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                    canBuild = false;
                    break;
                }
            }
            if (canBuild) {
                Vector2Int rotationOffset = objectsSO.GetRotationOffset(targetDir);
                Vector3 placedObjectWorldPosition = targetGrid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * targetGrid.GetCellSize();

                GridObjectsInfo placedObject = GridObjectsInfo.Create(placedObjectWorldPosition, targetDir, objectsSO, GameHandler.Instance.GridObjectsInstancesParent);

                foreach (Vector2Int gridPosition in gridPositionList) {
                    targetGrid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }
                // "?" = NULL?
                OnObjectPlaced?.Invoke(this, EventArgs.Empty);

                //DeselectObjectType();
            }
            else {
                // Cannot build here
            }
        }
    }

    private void AddGridEdgeObject(GridObjectsSO objectsSO, Vector3 worldPosition, Constants.Dir dir) {
        GridXZ<GridObject> targetGrid = gridList[GetGridLevel(worldPosition)];
        Vector2Int objCoodrinates = GetGridPosition(worldPosition, targetGrid);
        GridObject gridObject = targetGrid.GetGridObject(objCoodrinates.x, objCoodrinates.y);
        if (gridObject.gridObjectsInfo is object) {
            gridObject.gridObjectsInfo.PlaceEdge(dir, objectsSO, gridInstancesList);
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
                        floorPlacedObject.PlaceEdge(floorEdgePosition.edge, currentSO, gridInstancesList);
                    }
                }
            }
        }
    }


    private void AddFromInfoFunc(InstanceInfo bInfo) {
        GridObjectsSO[] foundSOTypeFromSerialized;
        GridObjectsSO typeSO;
        Vector3 worldPosition;

        foundSOTypeFromSerialized = (gridObjectsSOList)
            .Where(d => d.nameString == bInfo.SOName)
                .ToArray();

        if (foundSOTypeFromSerialized.Count<GridObjectsSO>() == 0) {
            Debug.Log("GridObjectsSO: " + bInfo.SOName + " was NOT serialized in this buildVer.");
            return;
        }
        typeSO = foundSOTypeFromSerialized[0];
        worldPosition = bInfo.position.ToVector3();

        if (typeSO.instantiableType == Constants.InstantiableTypes.GridObjects) {
            AddGridObject(typeSO, worldPosition, gridList[GetGridLevel(worldPosition)], bInfo.dir);
        }
        else if (typeSO.instantiableType == Constants.InstantiableTypes.GridEdgeObjects) {
            AddGridEdgeObject(typeSO, worldPosition, bInfo.dir);
        }
        GridObjectsInfo newBuildingInfo = GridObjectsInfo.Create<GridObjectsSO, GridObjectsInfo>(bInfo, typeSO, gridInstancesList.transform) as GridObjectsInfo;
        return;
    }


    public override void OnKeyPressed(object sender, TInstantiableObjectSystem.OnKeyPressedEventArgs e) {
        if (currentManager) {
            if (e.keyPressed == KeyCode.R) {
                dir = GridObjectsSO.GetNextDir(dir);
            }
            else if (e.keyPressed == KeyCode.Tab) {
                NextSO();
            }
        }
    }

    public override void OnMouse0(object sender, EventArgs e) {
        if (currentManager) {
            MouseClickAdd();
        }
    }

    public override void OnMouse1(object sender, EventArgs e) {
        if (currentManager) {

        }
    }

    public override void OnMouseMid(object sender, EventArgs e) {
        if (currentManager) {

        }
    }

    public override void OnMouseScroll(object sender, TInstantiableObjectSystem.OnMouseScrollEventArgs e) {
        if (currentManager) {

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
            return Quaternion.Euler(0, GridObjectsSO.GetRotationAngle(dir), 0);
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
            currentSO = gridObjectsSOList[listCounter];
            ActivateGhostObject();
        }
        else {
            Constants.InstantiableTypes pastType = gridObjectsSOList[listCounter].instantiableType;
            if (listCounter < gridObjectsSOList.Count - 1) {
                listCounter++;
            }
            else {
                listCounter = 0;
            }
            currentSO = gridObjectsSOList[listCounter];
            if (currentSO.instantiableType != pastType) {
                ActivateGhostObject();
            }
        }
        OnSelectedChanged.Invoke(this, EventArgs.Empty);
    }
}