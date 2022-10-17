using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TInstantiableObjectSO : ScriptableObject {
    public string nameString;
    public TInstantiableObjectSystem.IntantiableTypes intantiableType;
    public Transform transform;
    public Transform visual;
    public Sprite sprite;
    public int width;
    public int height;
    public enum Dir {
        NotFixed,
        Down,
        Left,
        Up,
        Right,
    }

}