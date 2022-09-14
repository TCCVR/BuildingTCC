using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class InstanceInfo {
    public TInstantiableObjectSystem.IntantiableTypes SOType;
    public string SOName;
    public string instanceName;
    public myVector3 position;
    public myVector3 rotation;
    public myVector3 scale;
    public TInstantiableObjectSO.Dir dir;
    public int width;
    public int height;
}