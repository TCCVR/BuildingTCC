using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem {
    public abstract class TInstantiableObjectInfo :MonoBehaviour {

        public InstanceInfo instanceInfo;

        //
        // Summary:
        //     Create a info from its SO and transform
        public static TInstantiableObjectInfo Create<T, TInfo>(InstanceInfo instanceInfo, T instanceableObjectSO, Transform parent) {
            Transform placedObjectTransform;
            Vector3 buildPos = instanceInfo.position.ToVector3();
            Vector3 buildScale = instanceInfo.scale.ToVector3();
            Quaternion buildRot = instanceInfo.rotation.ToQuaternion();
            placedObjectTransform = Instantiate((instanceableObjectSO as TInstantiableObjectSO).transform, buildPos, buildRot);

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
}