using System.Collections;
using UnityEngine;

public abstract class TInstantiableObjectsManager :MonoBehaviour {
    public delegate void MouseClickAddIntantiableObjectWithInfo();
    public delegate void AddInstantiableObjectsFromInfo(InstanceInfo bInfo);

    public MouseClickAddIntantiableObjectWithInfo mouseClickAdd;
    public AddInstantiableObjectsFromInfo addFromInfo;

    public TInstantiableObjectSystem.IntantiableTypes managedType;
}