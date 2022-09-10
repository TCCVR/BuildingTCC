using System.Collections;
using UnityEngine;


[System.Serializable]
public class InstanceInfo {
    public string SOName;
    public string instanceName;
    public myVector3 position;
    public myVector3 rotation;
    public myVector3 scale;
}

public class TInstanceableObjectInfo : MonoBehaviour{
    public InstanceInfo instanceInfo;
    delegate void LoadFromTypeSO(TInstanceableObjectSO btSO, Transform instancedObjTransform);
}