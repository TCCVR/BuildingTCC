﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovableObjectsInfo: TInstantiableObjectInfo {
    public override void LoadInfo<T>(T btSO, Transform instancedObjTransform) {
        if (this.instanceInfo is null)
            this.instanceInfo = new InstanceInfo();

        this.instanceInfo.SOType = TInstantiableObjectSystem.InstantiableTypes.MoveableObjects;
        MovableObjectsSO btSO2 = btSO as MovableObjectsSO;
        this.instanceInfo.SOName = btSO2.nameString;
        this.instanceInfo.instanceName = instancedObjTransform.name;

        this.instanceInfo.position = new MyVector3 { };
        this.instanceInfo.position.x = instancedObjTransform.position.x;
        this.instanceInfo.position.y = instancedObjTransform.position.y;
        this.instanceInfo.position.z = instancedObjTransform.position.z;

        this.instanceInfo.rotation = new MyVector3 { };
        this.instanceInfo.rotation.x = instancedObjTransform.rotation.x;
        this.instanceInfo.rotation.y = instancedObjTransform.rotation.y;
        this.instanceInfo.rotation.z = instancedObjTransform.rotation.z;
        this.instanceInfo.rotation.w = instancedObjTransform.rotation.w;

        this.instanceInfo.scale = new MyVector3 { x = 1, y = 1, z = 1 };
        this.instanceInfo.scale.x = instancedObjTransform.lossyScale.x;
        this.instanceInfo.scale.y = instancedObjTransform.lossyScale.y;
        this.instanceInfo.scale.z = instancedObjTransform.lossyScale.z;

        return;
    }

}
