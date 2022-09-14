using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TInstantiableObjectSystem : MonoBehaviour {

    public static TInstantiableObjectSystem Instance { get; private set; }

    public enum IntantiableTypes {
        GridObjects, //Construções
        GridEdgeObjects, //paredes das construções
        MoveableObjects, //objetos interagiveis
    }

    public Dictionary<IntantiableTypes, TInstantiableObjectsManager> Managers;


    private void Awake() {
        Instance = this;
    }
};