
using System.Collections;
using System;
using UnityEngine;
using Newtonsoft.Json;
public class GameHandler :MonoBehaviour {

    [SerializeField] private GameObject PlayerCreatedScenario;
    [SerializeField] private GameObject InstancedPrefabsList;
    private const string PLAYER_CREATED_SCENARIO = "playerCreatedScenario";
    private const string INSTANCED_PREFABS = "instancedPrefabs";

    private void Awake() {
        SaveSystem.Init();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            Load();
        }
    }

    private void Save() {
        PlacedBuildingInfo[] gameBInfoToSave = InstancedPrefabsList.GetComponentsInChildren<PlacedBuildingInfo>();
        SaveObject saveObject = new SaveObject();
        saveObject.playerCreatedScenario = new playerCreatedScenario();
        saveObject.playerCreatedScenario.instancedPrefabs = new instancedPrefabs();
        saveObject.playerCreatedScenario.instancedPrefabs.placedBuildingInfo = new InstanceInfo[gameBInfoToSave.Length];
        InstanceInfo[] listBInfo = saveObject.playerCreatedScenario.instancedPrefabs.placedBuildingInfo;
        for (int iC1 = 0; iC1 < gameBInfoToSave.Length; iC1++) {
            listBInfo[iC1] = gameBInfoToSave[iC1].instanceInfo;
        }

        Debug.Log("listBInfo: " + JsonConvert.SerializeObject(saveObject));
        
    }

    private void Load() {
    }

    [Serializable]
    public class SaveObject {
        public playerCreatedScenario playerCreatedScenario;
    }
    [Serializable]
    public class playerCreatedScenario {
        public instancedPrefabs instancedPrefabs;
    }
    [Serializable]
    public class instancedPrefabs {
        public InstanceInfo[] placedBuildingInfo;
    }
}
