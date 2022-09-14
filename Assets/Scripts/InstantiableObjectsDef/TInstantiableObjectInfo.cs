using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TInstantiableObjectInfo :MonoBehaviour {
    public InstanceInfo instanceInfo;
    Vector2Int origin;
    public abstract void LoadInfo<TInstantiableObjectSO>(TInstantiableObjectSO btSO, Transform instancedObjTransform);


    public virtual TInstantiableObjectInfo Create(InstanceInfo instanceInfo, TInstantiableObjectSO instanceableObjectSO, Transform parent) {
        Transform placedObjectTransform;
        Vector3 buildPos = new Vector3();
        buildPos.x = instanceInfo.position.x;
        buildPos.y = instanceInfo.position.y;
        buildPos.z = instanceInfo.position.z;
        Vector3 buildScale = new Vector3();
        buildScale.x = instanceInfo.scale.x;
        buildScale.y = instanceInfo.scale.y;
        buildScale.z = instanceInfo.scale.z;
        Quaternion buildRot = new Quaternion();
        buildRot.x = instanceInfo.rotation.x;
        buildRot.y = instanceInfo.rotation.y;
        buildRot.z = instanceInfo.rotation.z;
        buildRot.w = instanceInfo.rotation.w;
        placedObjectTransform = Instantiate(instanceableObjectSO.transform, buildPos, buildRot);
        //placedObjectTransform = Instantiate(instanceableObjectSO.transform, buildPos, Quaternion.Euler(0, instanceableObjectSO.GetRotationAngle(instanceInfo.dir), 0));

        placedObjectTransform.localScale = buildScale;
        placedObjectTransform.transform.parent = parent;

        TInstantiableObjectInfo placedObject = placedObjectTransform.GetComponent<TInstantiableObjectInfo>();
        placedObject.LoadInfo(instanceableObjectSO, placedObjectTransform);

        return placedObject;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }


}