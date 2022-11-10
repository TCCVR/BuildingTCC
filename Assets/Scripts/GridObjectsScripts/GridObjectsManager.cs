using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridObjectsManager :TInstantiableObjectsManager {
    public static GridObjectsManager Instance { get; private set; }

    private const float GRID_HEIGHT = 2.5f;

    GhostGridObject ghostGridObject;


    [SerializeField] public GameObject gridInstancesList;
    private List<GridObjectsSO> gridObjectsTypeSOList;
    [SerializeField] private GridObjectsSO gridObjectsType;

    private List<GridXZ<GridObject>> gridList;
    private GridXZ<GridObject> selectedGrid;
    private GridObjectsSO.Dir dir;

    private bool currentManager = false;

    [SerializeField] private LayerMask placedObjectEdgeColliderLayerMask;


    public event EventHandler OnActiveGridLevelChanged;
    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    private void Awake() {
        Instance = this;
        managedType = TInstantiableObjectSystem.IntantiableTypes.GridObjects;
        mouseClickAdd = mouseClickAddFunc;
        addFromInfo = addFromInfoFunc;
        //MovableObjectsTypeSelectUI initUI = MovableObjectsTypeSelectUI.Instance;

    }

    private void Start() {
        gridList = GameHandler.Instance.gridList;
        selectedGrid = gridList[0];

        TInstantiableObjectSystem.Instance.Managers.Add(TInstantiableObjectSystem.IntantiableTypes.GridObjects, Instance);
        TInstantiableObjectSystem.Instance.OnKeyPressed += OnKeyPressed;
        TInstantiableObjectSystem.Instance.OnMouse0 += OnMouse0;
        TInstantiableObjectSystem.Instance.OnMouse1 += OnMouse1;
        TInstantiableObjectSystem.Instance.OnMouseMid += OnMouseMid;
        TInstantiableObjectSystem.Instance.OnMouseScroll += OnMouseScroll;
        if (!TInstantiableObjectSystem.Instance.CurrentManager) {
            ActivateManager();
            Debug.Log("current manager name: " + TInstantiableObjectSystem.Instance.CurrentManager.name);
        }

        gridObjectsTypeSOList = GameAssets.Instance.gridObjectsTypeSOList;
        gridObjectsType = gridObjectsTypeSOList[0];
    }

    public override void ActivateManager() {
        TInstantiableObjectSystem.Instance.CurrentManager = Instance;
        if ( ghostGridObject is null) {
            ghostGridObject = GhostGridObject.Instance;
        }
        ghostGridObject.Activation();
        currentManager = true;
    }

    public override void DeactivateManager() {
        ghostGridObject.Activation(false);
        currentManager = false;
    }

    private void mouseClickAddFunc() {
        if (gridObjectsType.intantiableType == TInstantiableObjectSystem.IntantiableTypes.GridObjects) {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            selectedGrid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = selectedGrid.ValidateGridPosition(placedObjectOrigin);

            // Test Can Build
            List<Vector2Int> gridPositionList = gridObjectsType.GetGridPositionList(placedObjectOrigin, dir);
            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList) {
                if (!selectedGrid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild) {
                Vector2Int rotationOffset = gridObjectsType.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = selectedGrid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();

                GridObjectsInfo placedObject = GridObjectsInfo.Create(placedObjectWorldPosition, dir, gridObjectsType, GameHandler.Instance.GridObjectsInstancesParent);

                foreach (Vector2Int gridPosition in gridPositionList) {
                    selectedGrid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }
                // "?" = NULL?
                //OnObjectPlaced?.Invoke(this, EventArgs.Empty);

                //DeselectObjectType();
            }
            else {
                    // Cannot build here
            }
        }
    }


    private void addFromInfoFunc(InstanceInfo bInfo) {
        GridObjectsSO[] foundSOTypeFromSerialized;
        GridObjectsSO typeSO;

        foundSOTypeFromSerialized = gridObjectsTypeSOList
            .Where(d => d.nameString == bInfo.instanceName)
                .ToArray();

        if (foundSOTypeFromSerialized.Count<GridObjectsSO>() == 0) {
            Debug.Log("GridObjectsSO: " + bInfo.SOType + " was NOT serialized in this buildVer.");
            return;
        }
        typeSO = foundSOTypeFromSerialized[0];
        GridObjectsInfo newBuildingInfo = GridObjectsInfo.Create<GridObjectsSO, GridObjectsInfo>(bInfo, typeSO, gridInstancesList.transform) as GridObjectsInfo;
        return;
    }


    public override void OnKeyPressed(object sender, TInstantiableObjectSystem.OnKeyPressedEventArgs e) {
        if (currentManager) {
            if (e.keyPressed == KeyCode.R) {
                dir = GridObjectsSO.GetNextDir(dir);
            }
        }
    }

    public override void OnMouse0(object sender, EventArgs e) {
        if (currentManager) {
                mouseClickAdd();
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
        float gridHeight = GRID_HEIGHT;
        int newGridIndex = Mathf.Clamp(Mathf.RoundToInt(mousePosition.y / gridHeight), 0, gridList.Count - 1);
        selectedGrid = gridList[newGridIndex];
        OnActiveGridLevelChanged?.Invoke(this, EventArgs.Empty);
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        selectedGrid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition) {
        return selectedGrid.GetWorldPosition(gridPosition.x, gridPosition.y);
    }

    public GridObject GetGridObject(Vector2Int gridPosition) {
        return selectedGrid.GetGridObject(gridPosition.x, gridPosition.y);
    }

    public GridObject GetGridObject(Vector3 worldPosition) {
        return selectedGrid.GetGridObject(worldPosition);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) {
        return selectedGrid.IsValidGridPosition(gridPosition);
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
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        selectedGrid.GetXZ(mousePosition, out int x, out int z);

        if (gridObjectsType != null) {
            Vector2Int rotationOffset = gridObjectsType.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation() {
        if (gridObjectsType != null) {
            return Quaternion.Euler(0, GridObjectsSO.GetRotationAngle(dir), 0);
        }
        else {
            return Quaternion.identity;
        }
    }

    public TInstantiableObjectSO GetInstanceableObjectSO() {
        return gridObjectsType;
    }

    public int GetActiveGridLevel() {
        return gridList.IndexOf(selectedGrid);
    }
}