
using System.Collections;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
public class GameHandler :MonoBehaviour {

    [SerializeField] private GameObject MoveableObjectsInstancesList;

    private MovableObjectsManager BuildingManager;

    //test

    private void Awake() {
        SaveSystem.Init();
    }

    private void Start() {
        BuildingManager = MovableObjectsManager.Instance;
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
        MovableObjectsInfo[] gameBInfoToSave = MoveableObjectsInstancesList.GetComponentsInChildren<MovableObjectsInfo>();
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
            BuildingManager.addFromInfo(iInfo);
            
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
