using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class GridlessLoader :MonoBehaviour {
    public static GridlessLoader Instance { get; private set; }


    [SerializeField] public GameObject MoveableObjectsInstancesParent;
    [SerializeField] public GameObject GridObjectsInstancesParent;
    private List<GridObjectsSO> gridObjectsSOList;
    private List<MovableObjectsSO> movableObjectsTypeSOList;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        gridObjectsSOList = GameAssets.Instance.gridObjectsTypeSOList;
        movableObjectsTypeSOList = GameAssets.Instance.movableObjectsTypeSOList;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F3)) {
            LoadGridless();
        }
    }

    private GridObjectsSO FindGridSO(InstanceInfo instanceInfo) {
        GridObjectsSO[] foundSO;

        foundSO = (gridObjectsSOList)
            .Where(d => d.nameString == instanceInfo.SOName)
                .ToArray();

        if (foundSO.Count<GridObjectsSO>() == 0) {
            Debug.Log("GridObjectsSO: " + instanceInfo.SOName + " was NOT serialized in this buildVer.");
            return null;
        }
        return foundSO[0];
    }
    private MovableObjectsSO FindMovableSO(InstanceInfo instanceInfo) {
        MovableObjectsSO[] foundSO;

        foundSO = (movableObjectsTypeSOList)
            .Where(d => d.nameString == instanceInfo.SOName)
                .ToArray();

        if (foundSO.Count<MovableObjectsSO>() == 0) {
            Debug.Log("MovableObjectsSO: " + instanceInfo.SOName + " was NOT serialized in this buildVer.");
            return null;
        }
        return foundSO[0];
    }

    private void LoadGameObjects(SaveObject loaded) {
        InstanceInfo[] listBInfo = loaded.playerCreatedScenario.moveableObjects.placedMoveableInfo;
        foreach (InstanceInfo iInfo in listBInfo) {
            MovableObjectsSO foundSO = FindMovableSO(iInfo);
            if (iInfo is object) {
                TInstantiableObjectInfo.Create<MovableObjectsSO, MovableObjectsInfo>(iInfo, foundSO, MoveableObjectsInstancesParent.transform); 
            }
        }
        listBInfo = loaded.playerCreatedScenario.gridObjects.placedGridInfo;
        foreach (InstanceInfo iInfo in listBInfo) {
            GridObjectsSO foundSO = FindGridSO(iInfo);
            if (iInfo is object) {
                TInstantiableObjectInfo.Create<GridObjectsSO, GridObjectsInfo>(iInfo, foundSO, GridObjectsInstancesParent.transform); 
            }
        }
        return;
    }
    private void LoadGridless() {
        string saveString = SaveSystem.Load();
        if (saveString != null) {
            SaveObject savedObject = JsonConvert.DeserializeObject<SaveObject>(saveString);
            Debug.Log("Loaded: " + JsonConvert.SerializeObject(savedObject));
            LoadGameObjects(savedObject);
        }
        else {
            Debug.Log("No save");
        }
    }
}