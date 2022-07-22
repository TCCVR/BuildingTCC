using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class myVector3 {
    public float x;
    public float y;
    public float z;
    public float w;
}


[System.Serializable]
public class InstanceInfo {
    public string buildingTypeSOName;
    public string instanceName;
    public myVector3 buildingPosition;
    public myVector3 buildingRotation;
    public myVector3 buildingScale;
}

public class PlacedBuildingInfo: MonoBehaviour {
    public InstanceInfo instanceInfo;


    public void LoadFromBuildingTypeSO(BuildingTypeSO btSO, Transform instancedObjTransform) {
        if (this.instanceInfo is null)
            this.instanceInfo = new InstanceInfo();

        this.instanceInfo.buildingTypeSOName = btSO.typeName;
        this.instanceInfo.instanceName = instancedObjTransform.name;

        this.instanceInfo.buildingPosition = new myVector3 { };
        this.instanceInfo.buildingPosition.x = instancedObjTransform.position.x;
        this.instanceInfo.buildingPosition.y = instancedObjTransform.position.y;
        this.instanceInfo.buildingPosition.z = instancedObjTransform.position.z;

        this.instanceInfo.buildingRotation = new myVector3 { };
        this.instanceInfo.buildingRotation.x = instancedObjTransform.rotation.x;
        this.instanceInfo.buildingRotation.y = instancedObjTransform.rotation.y;
        this.instanceInfo.buildingRotation.z = instancedObjTransform.rotation.z;
        this.instanceInfo.buildingRotation.w = instancedObjTransform.rotation.w;

        this.instanceInfo.buildingScale = new myVector3 { x = 1, y = 1, z = 1 };
        this.instanceInfo.buildingScale.x = instancedObjTransform.lossyScale.x;
        this.instanceInfo.buildingScale.y = instancedObjTransform.lossyScale.y;
        this.instanceInfo.buildingScale.z = instancedObjTransform.lossyScale.z;
        return;
    }

}

