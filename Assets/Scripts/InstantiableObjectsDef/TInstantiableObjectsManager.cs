using System;
using System.Collections;
using UnityEngine;

public abstract class TInstantiableObjectsManager: MonoBehaviour {
    public delegate void MouseClickAddIntantiableObjectWithInfo();
    public delegate void AddInstantiableObjectsFromInfo(InstanceInfo bInfo);

    public MouseClickAddIntantiableObjectWithInfo mouseClickAdd;
    public AddInstantiableObjectsFromInfo addFromInfo;

    public TInstantiableObjectSystem.IntantiableTypes managedType;


    /// <summary>
    /// Activates manager functionalities  
    /// </summary>
    public abstract void ActivateManager();
    public abstract void DeactivateManager();
    public abstract void OnKeyPressed(object sender, TInstantiableObjectSystem.OnKeyPressedEventArgs e);
    public abstract void OnMouse0(object sender, EventArgs e);
    public abstract void OnMouse1(object sender, EventArgs e);
    public abstract void OnMouseMid(object sender, EventArgs e);
    public abstract void OnMouseScroll(object sender, TInstantiableObjectSystem.OnMouseScrollEventArgs e);
}