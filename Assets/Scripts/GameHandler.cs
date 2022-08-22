
using System.Collections;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
public class GameHandler :MonoBehaviour {

    [SerializeField] private GameObject PlayerCreatedScenario;
    [SerializeField] private GameObject BuildingInstancesList;
    [SerializeField] private GameObject ManagerObject;



    private MoveableObjectsManager BuildingManager;

    //test


    private void Awake() {
        SaveSystem.Init();

    }

    private void Start() {
        BuildingManager = ManagerObject.GetComponent<MoveableObjectsManager>();
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
        MoveableObjectsInfo[] gameBInfoToSave = BuildingInstancesList.GetComponentsInChildren<MoveableObjectsInfo>();
        SaveObject saveObject = new SaveObject();
        saveObject.playerCreatedScenario = new playerCreatedScenario();
        saveObject.playerCreatedScenario.instancedPrefabs = new instancedPrefabs();
        saveObject.playerCreatedScenario.instancedPrefabs.placedBuildingInfo = new InstanceInfo[gameBInfoToSave.Length];
        InstanceInfo[] listBInfo = saveObject.playerCreatedScenario.instancedPrefabs.placedBuildingInfo;
        for (int iC1 = 0; iC1 < gameBInfoToSave.Length; iC1++) {
            listBInfo[iC1] = gameBInfoToSave[iC1].instanceInfo;
        }

        SaveSystem.Save(JsonConvert.SerializeObject(saveObject));
        Debug.Log("listBInfo: " + JsonConvert.SerializeObject(saveObject));

    }

    private void LoadInstancedPrefabs(SaveObject loaded) {
        InstanceInfo[] listBInfo = loaded.playerCreatedScenario.instancedPrefabs.placedBuildingInfo;
        foreach (InstanceInfo iInfo in listBInfo) {
            BuildingManager.AddMoveableObjectsFromInfo(iInfo);
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
        public instancedPrefabs instancedPrefabs {
            set;
            get;
        }

    }
    [Serializable]
    public class instancedPrefabs {
        public InstanceInfo[] placedBuildingInfo {
            set;
            get;
        }

    }
}
