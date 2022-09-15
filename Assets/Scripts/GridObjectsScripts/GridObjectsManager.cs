using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridObjectsManager :TInstantiableObjectsManager {
    public static GridObjectsManager Instance { get; private set; }
    [SerializeField] public GameObject gridInstancesList;

    [SerializeField] public List<GridObjectsSO> gridObjectsTypeSOList;
    [SerializeField] private GridObjectsSO gridObjectsType;

    private List<GridXZ<GridObject>> gridList;
    private GridXZ<GridObject> selectedGrid;
    private GridObjectsSO.Dir dir;

    private bool currentManager = false;


    private void Start() {
        Instance = this;
        managedType = TInstantiableObjectSystem.IntantiableTypes.GridObjects;
        mouseClickAdd = mouseClickAddFunc;
        addFromInfo = addFromInfoFunc;
        //MovableObjectsTypeSelectUI initUI = MovableObjectsTypeSelectUI.Instance;


        int gridWidth = 1000;
        int gridHeight = 1000;
        float cellSize = 1f;

        gridList = new List<GridXZ<GridObject>>();
        int gridVerticalCount = 4;
        float gridVerticalSize = 2f;
        for (int i = 0; i < gridVerticalCount; i++) {
            GridXZ<GridObject> grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, gridVerticalSize * i, 0), (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));
            gridList.Add(grid);
        }

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
    }


    private void mouseClickAddFunc() {
        Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
        float maxBuildDistance = 10f;

        if (Mouse3D.GetDistanceToPlayer() >= maxBuildDistance) {
            return;
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

    public override void ActivateManager() {
        TInstantiableObjectSystem.Instance.CurrentManager = Instance;
        currentManager = true;
    }

    public override void DeactivateManager() {
        currentManager = false;
    }

    public override void OnKeyPressed(object sender, TInstantiableObjectSystem.OnKeyPressedEventArgs e) {
        if (currentManager) {

        }
    }

    public override void OnMouse0(object sender, EventArgs e) {
        if (currentManager) {

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

    public class GridObject {

        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        public GridObjectsInfo gridObjectsInfo;

        public GridObject(GridXZ<GridObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            gridObjectsInfo = null;
        }

        public override string ToString() {
            return x + ", " + y + "\n" + gridObjectsInfo;
        }

        public void TriggerGridObjectChanged() {
            grid.TriggerGridObjectChanged(x, y);
        }

        public void SetPlacedObject(GridObjectsInfo placedObject) {
            this.gridObjectsInfo = placedObject;
            TriggerGridObjectChanged();
        }

        public void ClearPlacedObject() {
            gridObjectsInfo = null;
            TriggerGridObjectChanged();
        }

        public GridObjectsInfo GetPlacedObject() {
            return gridObjectsInfo;
        }

        public bool CanBuild() {
            return gridObjectsInfo == null;
        }

    }

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        selectedGrid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
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


}