using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class GameHandler :MonoBehaviour {

    public static GameHandler Instance { get; private set; }

    [SerializeField] public GameObject MoveableObjectsInstancesParent;
    [SerializeField] public GameObject GridObjectsInstancesParent;

    private MovableObjectsManager movableObjectsManager;
    private GridObjectsManager gridObjectsManager;

    public List<GridXZ<GridObject>> gridList;





    private void Awake() {
        Instance = this;
        SaveSystem.Init();

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
        movableObjectsManager = MovableObjectsManager.Instance;
        gridObjectsManager = GridObjectsManager.Instance;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            LoadToGrid();
        }

    }

    private void Save() {
        MovableObjectsInfo[] movableObjectsToSave = MoveableObjectsInstancesParent.GetComponentsInChildren<MovableObjectsInfo>();
        GridObjectsInfo[] gridObjectsToSave = GridObjectsInstancesParent.GetComponentsInChildren<GridObjectsInfo>();
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

        SaveSystem.Save(JsonConvert.SerializeObject(saveObject));
        Debug.Log("listBInfo: " + JsonConvert.SerializeObject(saveObject));

    }

    private void LoadGameObjectsIntoGrid(SaveObject loaded) {
        InstanceInfo[] listBInfo = loaded.playerCreatedScenario.moveableObjects.placedMoveableInfo;
        foreach (InstanceInfo iInfo in listBInfo) {
            movableObjectsManager.AddFromInfo(iInfo);
        }
        listBInfo = loaded.playerCreatedScenario.gridObjects.placedGridInfo;
        foreach (InstanceInfo iInfo in listBInfo) {
            if (iInfo.SOType == Constants.InstantiableTypes.GridObjects) {
                gridObjectsManager.AddFromInfo(iInfo); 
            }
        }
        foreach (InstanceInfo iInfo in listBInfo) {
            if (iInfo.SOType == Constants.InstantiableTypes.GridEdgeObjects) {
                gridObjectsManager.AddFromInfo(iInfo);
            }
        }
        return;
    }


    private void LoadToGrid() {
        string saveString = SaveSystem.Load();
        if (saveString != null) {
            SaveObject savedObject = JsonConvert.DeserializeObject<SaveObject>(saveString);
            Debug.Log("Loaded: " + JsonConvert.SerializeObject(savedObject));
            LoadGameObjectsIntoGrid(savedObject);
        }
        else {
            Debug.Log("No save");
        }
    }
}


