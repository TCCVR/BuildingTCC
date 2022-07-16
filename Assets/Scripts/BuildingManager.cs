using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using CodeMonkey.Utils;

public class BuildingManager : MonoBehaviour
{
    //[SerializeField] private Transform pfWoodHarvester;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private BuildingTypeSO activeBuildingType;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mouseWorldPosition =  Mouse3D.GetMouseWorldPosition();
            Debug.Log(mouseWorldPosition);
            float maxBuildDistance = 10f;
            if (Vector3.Distance(playerTransform.position, mouseWorldPosition)<maxBuildDistance) {
                Instantiate(activeBuildingType.prefab, mouseWorldPosition, Quaternion.identity);
            }
            
        }
    }

    public void SetActiveBuildingType (BuildingTypeSO buildingTypeSO)
    {
        activeBuildingType = buildingTypeSO;
    }

    public BuildingTypeSO GetActiveBuildingType()
    {
        return activeBuildingType;
    }
}
