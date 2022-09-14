﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GridObjectsInfo :TInstantiableObjectInfo {
    public static TInstantiableObjectInfo Create(Vector3 worldPosition, GridObjectsSO.Dir dir, TInstantiableObjectSO instanceableObjectSO) {
        Transform placedObjectTransform = Instantiate(instanceableObjectSO.transform, worldPosition, Quaternion.Euler(0, GridObjectsSO.GetRotationAngle(dir), 0));

        TInstantiableObjectInfo placedObject = placedObjectTransform.GetComponent<TInstantiableObjectInfo>();
        placedObject.LoadInfo(instanceableObjectSO, placedObjectTransform);
        placedObject.instanceInfo.dir = dir;

        return placedObject;
    }

    public override void LoadInfo<T>(T btSO, Transform instancedObjTransform) {
        if (this.instanceInfo is null)
            this.instanceInfo = new InstanceInfo();

        this.instanceInfo.SOType = TInstantiableObjectSystem.IntantiableTypes.GridObjects;
        GridObjectsSO btSO2 = btSO as GridObjectsSO;
        this.instanceInfo.SOName = btSO2.nameString;
        this.instanceInfo.width = btSO2.width;
        this.instanceInfo.height = btSO2.height;
        this.instanceInfo.instanceName = instancedObjTransform.name;

        this.instanceInfo.position = new myVector3 { };
        this.instanceInfo.position.x = instancedObjTransform.position.x;
        this.instanceInfo.position.y = instancedObjTransform.position.y;
        this.instanceInfo.position.z = instancedObjTransform.position.z;

        this.instanceInfo.rotation = new myVector3 { };
        this.instanceInfo.rotation.x = instancedObjTransform.rotation.x;
        this.instanceInfo.rotation.y = instancedObjTransform.rotation.y;
        this.instanceInfo.rotation.z = instancedObjTransform.rotation.z;
        this.instanceInfo.rotation.w = instancedObjTransform.rotation.w;

        this.instanceInfo.scale = new myVector3 { x = 1, y = 1, z = 1 };
        this.instanceInfo.scale.x = instancedObjTransform.lossyScale.x;
        this.instanceInfo.scale.y = instancedObjTransform.lossyScale.y;
        this.instanceInfo.scale.z = instancedObjTransform.lossyScale.z;
        return;
    }

    public List<Vector2Int> GetGridPositionList() {
        return GridObjectsSO.GetGridPositionList(instanceInfo.width, instanceInfo.height, new Vector2Int(Mathf.FloorToInt(instanceInfo.position.x / 2), Mathf.FloorToInt(instanceInfo.position.z / 2)), instanceInfo.dir);
    }
}

