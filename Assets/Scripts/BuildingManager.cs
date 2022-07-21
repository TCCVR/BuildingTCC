using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour {
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject gameHandlerBaseObj;
    public List<BuildingTypeSO> buildingTypeSOList;
    [SerializeField] private BuildingTypeSO activeBuildingType;
    [SerializeField] private GameObject PlayerCreatedScenario;
    [SerializeField] private GameObject InstancedPrefabsList;
    private int counter;

    private void Awake() {
        BuildingTypeSO[] loadedBTSO;
        loadedBTSO = FileUtils.GetAtPath<BuildingTypeSO>("BuildingTypes/");
        foreach (BuildingTypeSO BTSO in loadedBTSO) {
            buildingTypeSOList.Add(BTSO);
        }
        BuildingTypeSelectUI initUI = gameHandlerBaseObj.GetComponent<BuildingTypeSelectUI>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            MouseClickAddBuildingWithInfo();
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

    public void SetActiveBuildingType (BuildingTypeSO buildingTypeSO) {
        activeBuildingType = buildingTypeSO;
    }

    public BuildingTypeSO GetActiveBuildingType() {
        return activeBuildingType;
    }

    private void MouseClickAddBuildingWithInfo() {
        Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
        float maxBuildDistance = 10f;

        if (Vector3.Distance(playerTransform.position, mouseWorldPosition) >= maxBuildDistance) {
            return;
        }

        Transform newPlacedBuilding = Instantiate(activeBuildingType.transform, mouseWorldPosition, transform.rotation * Quaternion.Euler(0, 45 * counter, 0));
        newPlacedBuilding.transform.parent = InstancedPrefabsList.transform;
        PlacedBuildingInfo newBuildingInfo = newPlacedBuilding.gameObject.AddComponent<PlacedBuildingInfo>();
        newBuildingInfo.LoadFromBuildingTypeSO(activeBuildingType, newPlacedBuilding);
    }

    private void AddBuildingFromInfo(PlacedBuildingInfo bInfo) {
        BuildingTypeSO[] foundSOTypeFromSerialized;
        BuildingTypeSO typeSO;
        foundSOTypeFromSerialized = buildingTypeSOList.Where(d => d.typeName == bInfo.instanceInfo.buildingTypeSOName).ToArray();
        if (foundSOTypeFromSerialized.Count<BuildingTypeSO>() == 0) {
            Debug.Log("BuildingTypeSO: " + bInfo.instanceInfo.buildingTypeSOName + " was NOT serialized in this buildVer.");
            return;
        }
        typeSO = foundSOTypeFromSerialized[0];
        Vector3 buildPos = new Vector3();
        buildPos.x = bInfo.instanceInfo.buildingPosition.x;
        buildPos.y = bInfo.instanceInfo.buildingPosition.y;
        buildPos.z = bInfo.instanceInfo.buildingPosition.z;
        Quaternion buildRot = new Quaternion();
        buildRot.x = bInfo.instanceInfo.buildingRotation.x;
        buildRot.y = bInfo.instanceInfo.buildingRotation.y;
        buildRot.z = bInfo.instanceInfo.buildingRotation.z;
        buildRot.w = bInfo.instanceInfo.buildingRotation.w;
        Transform newPlacedBuilding = Instantiate(typeSO.transform, buildPos, buildRot);
        Vector3 buildScale = new Vector3();
        buildScale.x = bInfo.instanceInfo.buildingScale.x;
        buildScale.y = bInfo.instanceInfo.buildingScale.y;
        buildScale.z = bInfo.instanceInfo.buildingScale.z;
        newPlacedBuilding.localScale = buildScale;

        newPlacedBuilding.transform.parent = InstancedPrefabsList.transform;
        return;
    }



}
