using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets :MonoBehaviour {
    public static GameAssets Instance { get; private set; }

    [SerializeField] public List<GridObjectsSO> gridObjectsTypeSOList;
    [SerializeField] public List<MovableObjectsSO> movableObjectsTypeSOList;


    void Start() {
        Instance = this;
    }




}