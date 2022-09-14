using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableObjectsManager : TInstantiableObjectsManager {
    public static MovableObjectsManager Instance { get; private set; }

    [SerializeField] public List<MovableObjectsSO> moveableObjectsTypeSOList;
    [SerializeField] private MovableObjectsSO activeMoveableObjectsType;
    [SerializeField] public GameObject MOInstancesList;
    private int counter;


    [SerializeField] private GameObject GameHandlerBaseObj;
    [SerializeField] private GameObject PlayerCreatedScenario;


    private void Awake() {
        Instance = this;
        this.managedType = TInstantiableObjectSystem.IntantiableTypes.MoveableObjects;
        MovableObjectsTypeSelectUI initUI = GameHandlerBaseObj.GetComponent<MovableObjectsTypeSelectUI>();
        activeMoveableObjectsType = moveableObjectsTypeSOList[0];
        mouseClickAdd = mouseClickAddFunc;
        addFromInfo = addFromInfoFunc;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            mouseClickAddFunc();
        }

        if (Input.GetMouseButtonDown(1)) {
            counter += 1;
            if (counter == 8) {
                counter = 0;
            }
            print(counter);
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            counter = 0;
            print("Counter Reset");
        }
    }

    public void SetActiveBuildingType (MovableObjectsSO moveableObjectsTypeSO) {
        activeMoveableObjectsType = moveableObjectsTypeSO;
    }

    public MovableObjectsSO GetActiveMOType() {
        return activeMoveableObjectsType;
    }

    private void mouseClickAddFunc() {
        Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
        float maxBuildDistance = 10f;

        if (Mouse3D.GetDistanceToPlayer() >= maxBuildDistance) {
            return;
        }

        Transform newPlacedMoveableObjects = Instantiate(activeMoveableObjectsType.transform, mouseWorldPosition, transform.rotation * Quaternion.Euler(0, 45 * counter, 0));
        newPlacedMoveableObjects.transform.parent = MOInstancesList.transform;
        MovableObjectsInfo newMoveableObjectsInfo = newPlacedMoveableObjects.gameObject.AddComponent<MovableObjectsInfo>();
        newMoveableObjectsInfo.LoadInfo(activeMoveableObjectsType, newPlacedMoveableObjects);
    }

    public MovableObjectsSO[] FindSOTypeFromName(string name) {
        return moveableObjectsTypeSOList
            .Where(d => d.nameString == name)
                .ToArray();
    }

    private void addFromInfoFunc(InstanceInfo bInfo) {
        MovableObjectsSO[] foundSOTypeFromSerialized;
        MovableObjectsSO typeSO;

        foundSOTypeFromSerialized = moveableObjectsTypeSOList
            .Where(d => d.nameString == bInfo.instanceName)
                .ToArray();

        if (foundSOTypeFromSerialized.Count<MovableObjectsSO>() == 0) {
            Debug.Log("MovableObjectsSO: " + bInfo.SOType + " was NOT serialized in this buildVer.");
            return;
        }
        typeSO = foundSOTypeFromSerialized[0];
        Vector3 buildPos = new Vector3();
        buildPos.x = bInfo.position.x;
        buildPos.y = bInfo.position.y;
        buildPos.z = bInfo.position.z;
        Quaternion buildRot = new Quaternion();
        buildRot.x = bInfo.rotation.x;
        buildRot.y = bInfo.rotation.y;
        buildRot.z = bInfo.rotation.z;
        buildRot.w = bInfo.rotation.w;
        Transform newPlacedBuilding = Instantiate(typeSO.transform, buildPos, buildRot);
        Vector3 buildScale = new Vector3();
        buildScale.x = bInfo.scale.x;
        buildScale.y = bInfo.scale.y;
        buildScale.z = bInfo.scale.z;
        newPlacedBuilding.localScale = buildScale;
        MovableObjectsInfo newBuildingInfo = newPlacedBuilding.gameObject.AddComponent<MovableObjectsInfo>();
        newBuildingInfo.LoadInfo(activeMoveableObjectsType, newPlacedBuilding);

        newPlacedBuilding.transform.parent = MOInstancesList.transform;
        return;
    }



}
