using UnityEngine;
using Newtonsoft.Json;


[System.Serializable]
public class MyVector3 {
    public float x;
    public float y;
    public float z;
    public float w;
}

[System.Serializable]
public class InstanceInfo {
    public TInstantiableObjectSystem.InstantiableTypes SOType;
    public string SOName;
    public string instanceName;
    public MyVector3 position;
    public MyVector3 rotation;
    public MyVector3 scale;
    public GridObjectsSO.Dir dir;
    public int width;
    public int height;
}