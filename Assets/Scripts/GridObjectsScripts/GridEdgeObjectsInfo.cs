using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEdgeObjectsInfo :TInstantiableObjectInfo {

    public static TInstantiableObjectInfo Create(Vector3 worldPosition, GridObjectsSO.Dir dir, TInstantiableObjectSO instanceableObjectSO) {
        Transform placedObjectTransform = Instantiate(instanceableObjectSO.transform, worldPosition, Quaternion.Euler(0, GridObjectsSO.GetRotationAngle(dir), 0));

        TInstantiableObjectInfo placedObject = placedObjectTransform.GetComponent<TInstantiableObjectInfo>();
        placedObject.LoadInfo(instanceableObjectSO, placedObjectTransform);
        placedObject.instanceInfo.dir = dir;

        return placedObject;
    }

    public override void LoadInfo<TInstantiableObjectSO>(TInstantiableObjectSO btSO, Transform instancedObjTransform) {
        throw new System.NotImplementedException();
    }


    public List<Vector2Int> GetGridPositionList() {
        return GridObjectsSO.GetGridPositionList(instanceInfo.width, instanceInfo.height, new Vector2Int(Mathf.FloorToInt(instanceInfo.position.x / 2), Mathf.FloorToInt(instanceInfo.position.z / 2)), instanceInfo.dir);
    }
}