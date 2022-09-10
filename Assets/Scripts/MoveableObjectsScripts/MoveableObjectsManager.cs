using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveableObjectsManager : TInstanceableObjectsManager {
    [SerializeField] private Transform playerTransform;
    public List<MoveableObjectsSO> moveableObjectsTypeSOList;
    [SerializeField] private MoveableObjectsSO activeMoveableObjectsType;
    [SerializeField] private GameObject MOInstancesList;
    private int counter;


    [SerializeField] private GameObject GameHandlerBaseObj;
    [SerializeField] private GameObject PlayerCreatedScenario;

    private void Awake() {
        MoveableObjectsTypeSelectUI initUI = GameHandlerBaseObj.GetComponent<MoveableObjectsTypeSelectUI>();
        activeMoveableObjectsType = moveableObjectsTypeSOList[0];
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            MouseClickAddMOWithInfo();
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

    public void SetActiveBuildingType (MoveableObjectsSO moveableObjectsTypeSO) {
        activeMoveableObjectsType = moveableObjectsTypeSO;
    }

    public MoveableObjectsSO GetActiveMOType() {
        return activeMoveableObjectsType;
    }

    private void MouseClickAddMOWithInfo() {
        Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
        float maxBuildDistance = 10f;

        if (Vector3.Distance(playerTransform.position, mouseWorldPosition) >= maxBuildDistance) {
            return;
        }

        Transform newPlacedMoveableObjects = Instantiate(activeMoveableObjectsType.transform, mouseWorldPosition, transform.rotation * Quaternion.Euler(0, 45 * counter, 0));
        newPlacedMoveableObjects.transform.parent = MOInstancesList.transform;
        MoveableObjectsInfo newMoveableObjectsInfo = newPlacedMoveableObjects.gameObject.AddComponent<MoveableObjectsInfo>();
        newMoveableObjectsInfo.LoadFromTypeSO(activeMoveableObjectsType, newPlacedMoveableObjects);
    }

    public void AddMoveableObjectsFromInfo(InstanceInfo bInfo) {
        MoveableObjectsSO[] foundSOTypeFromSerialized;
        MoveableObjectsSO typeSO;

        foundSOTypeFromSerialized = moveableObjectsTypeSOList
            .Where(d => d.nameString == bInfo.SOName)
                .ToArray();

        if (foundSOTypeFromSerialized.Count<MoveableObjectsSO>() == 0) {
            Debug.Log("MoveableObjectsSO: " + bInfo.SOName + " was NOT serialized in this buildVer.");
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
        MoveableObjectsInfo newBuildingInfo = newPlacedBuilding.gameObject.AddComponent<MoveableObjectsInfo>();
        newBuildingInfo.LoadFromTypeSO(activeMoveableObjectsType, newPlacedBuilding);

        newPlacedBuilding.transform.parent = MOInstancesList.transform;
        return;
    }



}
