using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableObjectsManager : TInstantiableObjectsManager {
    public static MovableObjectsManager Instance { get; private set; }

    GhostMovableObject ghostMovableObject;

    [SerializeField] public List<MovableObjectsSO> movableObjectsTypeSOList;
    [SerializeField] private MovableObjectsSO activeMovableObjectsType;
    [SerializeField] public GameObject MOInstancesList;

    private float looseObjectEulerY;
    private int counter;
    private bool currentManager = false;

    private void Awake() {
        Instance = this;
        managedType = TInstantiableObjectSystem.IntantiableTypes.MoveableObjects;
        activeMovableObjectsType = movableObjectsTypeSOList[0];
        mouseClickAdd = mouseClickAddFunc;
        addFromInfo = addFromInfoFunc;
    }



    private void Start() {
        MovableObjectsTypeSelectUI initUI = MovableObjectsTypeSelectUI.Instance;
        TInstantiableObjectSystem.Instance.Managers.Add(TInstantiableObjectSystem.IntantiableTypes.MoveableObjects, this);
        TInstantiableObjectSystem.Instance.OnKeyPressed += OnKeyPressed;
        TInstantiableObjectSystem.Instance.OnMouse0 += OnMouse0;
        TInstantiableObjectSystem.Instance.OnMouse1 += OnMouse1;
        TInstantiableObjectSystem.Instance.OnMouseMid += OnMouseMid;
        TInstantiableObjectSystem.Instance.OnMouseScroll += OnMouseScroll;
        if (!TInstantiableObjectSystem.Instance.CurrentManager) {
            ActivateManager();
            Debug.Log("current manager name: " + TInstantiableObjectSystem.Instance.CurrentManager.name);
        }
    }

    private void Update() {

 
    }

    /// <summary>
    /// Changes active SO type
    /// </summary>
    /// <param name="moveableObjectsTypeSO">SO type to activate</param>
    /// <returns></returns>
    public void SetActiveBuildingType (MovableObjectsSO moveableObjectsTypeSO) {
        activeMovableObjectsType = moveableObjectsTypeSO;
    }

    public MovableObjectsSO GetActiveMOType() {
        return activeMovableObjectsType;
    }

    private void mouseClickAddFunc() {
        Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
        float maxBuildDistance = 10f;

        if (Mouse3D.GetDistanceToPlayer() >= maxBuildDistance) {
            return;
        }

        Transform newPlacedMoveableObjects = Instantiate(activeMovableObjectsType.transform, mouseWorldPosition, transform.rotation * Quaternion.Euler(0, 45 * counter, 0));
        newPlacedMoveableObjects.transform.parent = MOInstancesList.transform;
        MovableObjectsInfo newMoveableObjectsInfo = newPlacedMoveableObjects.gameObject.GetComponent<MovableObjectsInfo>();
        newMoveableObjectsInfo.LoadInfo(activeMovableObjectsType, newPlacedMoveableObjects);
    }

    public MovableObjectsSO[] FindSOTypeFromName(string name) {
        return movableObjectsTypeSOList
            .Where(d => d.nameString == name)
                .ToArray();
    }

    private void addFromInfoFunc(InstanceInfo bInfo) {
        MovableObjectsSO[] foundSOTypeFromSerialized;
        MovableObjectsSO typeSO;

        foundSOTypeFromSerialized = movableObjectsTypeSOList
            .Where(d => d.nameString == bInfo.instanceName)
                .ToArray();

        if (foundSOTypeFromSerialized.Count<MovableObjectsSO>() == 0) {
            Debug.Log("MovableObjectsSO: " + bInfo.SOType + " was NOT serialized in this buildVer.");
            return;
        }
        typeSO = foundSOTypeFromSerialized[0];
        MovableObjectsInfo newBuildingInfo = MovableObjectsInfo.Create<MovableObjectsSO, MovableObjectsInfo>(bInfo, typeSO, MOInstancesList.transform) as MovableObjectsInfo;
        return;
    }

    public float GetMovableObjectEulerY() {
        return looseObjectEulerY;
    }

    public override void ActivateManager(){
        TInstantiableObjectSystem.Instance.CurrentManager = Instance;
        if (ghostMovableObject is null) {
            ghostMovableObject = GhostMovableObject.Instance;
        }
        ghostMovableObject.Activation();
        currentManager = true;
    }
    public override void DeactivateManager() {
        ghostMovableObject.Activation(false);
        currentManager = false;
    }

    public override void OnKeyPressed(object sender, TInstantiableObjectSystem.OnKeyPressedEventArgs e) { 
        if (currentManager) {
            Debug.Log("MoveableObjectManager KeyPressed!: " + e.keyPressed); 
            if (e.keyPressed == KeyCode.F) {
                counter = 0;
                print("Counter Reset");
            }

        }
    }
    public override void OnMouse0(object sender, EventArgs e) {
        if (currentManager) {
            Debug.Log("MoveableObjectManager OnMouse0!"); 
            if(!EventSystem.current.IsPointerOverGameObject()) {
                mouseClickAddFunc();
            }
        }
    }
    public override void OnMouse1(object sender, EventArgs e) {
        if (currentManager) {
            Debug.Log("MoveableObjectManager OnMouse1!"); 
            counter += 1;
            if (counter == 8) {
                counter = 0;
            }
            print(counter);
        }
    }
    public override void OnMouseMid(object sender, EventArgs e) {
        if (currentManager) {
            Debug.Log("MoveableObjectManager OnMouseMid!");

        }
    }
    public override void OnMouseScroll(object sender, TInstantiableObjectSystem.OnMouseScrollEventArgs e) {
        if (currentManager) {
            Debug.Log("MoveableObjectManager OnMouseScroll!");

        }
    }
}
