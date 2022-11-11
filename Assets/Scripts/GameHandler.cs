
using System.Collections.Generic;
using System;
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
            Load();
        }

    }

    private void Save() {
        MovableObjectsInfo[] gameBInfoToSave = MoveableObjectsInstancesParent.GetComponentsInChildren<MovableObjectsInfo>();
        SaveObject saveObject = new SaveObject();
        saveObject.playerCreatedScenario = new playerCreatedScenario();
        saveObject.playerCreatedScenario.moveableObjects = new MoveableObjects();
        saveObject.playerCreatedScenario.moveableObjects.placedMoveableInfo = new InstanceInfo[gameBInfoToSave.Length];
        InstanceInfo[] listBInfo = saveObject.playerCreatedScenario.moveableObjects.placedMoveableInfo;
        for (int iC1 = 0; iC1 < gameBInfoToSave.Length; iC1++) {
            listBInfo[iC1] = gameBInfoToSave[iC1].instanceInfo;
        }

        SaveSystem.Save(JsonConvert.SerializeObject(saveObject));
        Debug.Log("listBInfo: " + JsonConvert.SerializeObject(saveObject));

    }

    private void LoadInstancedPrefabs(SaveObject loaded) {
        InstanceInfo[] listBInfo = loaded.playerCreatedScenario.moveableObjects.placedMoveableInfo;
        foreach (InstanceInfo iInfo in listBInfo) {
            movableObjectsManager.addFromInfo(iInfo);
            
        }
        return;
    }


    private void Load() {
        string saveString = SaveSystem.Load();
        if (saveString != null) {
            SaveObject savedObject = JsonConvert.DeserializeObject<SaveObject>(saveString);
            Debug.Log("Loaded: " + JsonConvert.SerializeObject(savedObject));
            LoadInstancedPrefabs(savedObject);
        }
        else {
            Debug.Log("No save");
        }
    }

    [Serializable]
    public class SaveObject {
        public playerCreatedScenario playerCreatedScenario {
            set;
            get;
        }
    }
    [Serializable]
    public class playerCreatedScenario {
        public MoveableObjects moveableObjects {
            set;
            get;
        }

    }
    [Serializable]
    public class MoveableObjects {
        public InstanceInfo[] placedMoveableInfo {
            set;
            get;
        }

    }
}


[System.Serializable]
public class myVector3 {
    public float x;
    public float y;
    public float z;
    public float w;
}
