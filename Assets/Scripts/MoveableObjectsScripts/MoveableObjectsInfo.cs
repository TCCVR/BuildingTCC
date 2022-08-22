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
    public string SOName;
    public string instanceName;
    public myVector3 position;
    public myVector3 rotation;
    public myVector3 scale;
}

public class MoveableObjectsInfo: MonoBehaviour {
    public InstanceInfo instanceInfo;


    public void LoadFromMOTypeSO(MoveableObjectsSO btSO, Transform instancedObjTransform) {
        if (this.instanceInfo is null)
            this.instanceInfo = new InstanceInfo();

        this.instanceInfo.SOName = btSO.typeName;
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

}

