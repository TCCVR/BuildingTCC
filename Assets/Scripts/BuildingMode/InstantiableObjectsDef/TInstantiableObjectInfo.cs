using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TInstantiableObjectInfo: MonoBehaviour {

    public InstanceInfo instanceInfo;
    Vector2Int origin;

    //
    // Summary:
    //     Create a info from its SO and transform
    public static TInstantiableObjectInfo Create<T, TInfo>(InstanceInfo instanceInfo, T instanceableObjectSO, Transform parent) {
        Transform placedObjectTransform;
        Vector3 buildPos = instanceInfo.position.ToVector3();
        Vector3 buildScale = instanceInfo.scale.ToVector3();
        Quaternion buildRot = instanceInfo.rotation.ToQuaternion();
        placedObjectTransform = Instantiate((instanceableObjectSO as TInstantiableObjectSO).transform, buildPos, buildRot);
        //placedObjectTransform = Instantiate(instanceableObjectSO.transform, buildPos, Quaternion.Euler(0, instanceableObjectSO.GetRotationAngle(instanceInfo.dir), 0));

        placedObjectTransform.localScale = buildScale;
        placedObjectTransform.transform.parent = parent;

        TInfo placedObject = placedObjectTransform.GetComponent<TInfo>();
        (placedObject as TInstantiableObjectInfo).LoadInfo(instanceableObjectSO, placedObjectTransform);

        return (placedObject as TInstantiableObjectInfo);
    }


    public abstract void LoadInfo<TInstantiableObjectSO>(TInstantiableObjectSO btSO, Transform instancedObjTransform);


    public void DestroySelf() {
        Destroy(gameObject);
    }


}