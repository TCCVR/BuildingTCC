using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableObjectsManager : TInstantiableObjectsManager {
    public static MovableObjectsManager Instance { get; private set; }

    GhostMovableObject ghostMovableObject;

    private List<MovableObjectsSO> movableObjectsTypeSOList;
    private MovableObjectsSO activeMovableObjectsType;
    [SerializeField] public GameObject MOInstancesList;

    private float looseObjectEulerY;
    private int angleDiscreetCounter = 0;
    private int listCounter = 0;
    private bool currentManager = false;

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    private void Awake() {
        Instance = this;
        managedType = Constants.InstantiableTypes.MoveableObjects;
        MouseClickAdd = mouseClickAddFunc;
        AddFromInfo = addFromInfoFunc;
        //MovableObjectsTypeSelectUI initUI = MovableObjectsTypeSelectUI.Instance;
    }



    private void Start() {
        movableObjectsTypeSOList = GameAssets.Instance.movableObjectsTypeSOList;
        MovableObjectsTypeSelectUI initUI = MovableObjectsTypeSelectUI.Instance;
        TInstantiableObjectSystem.Instance.Managers.Add(Constants.InstantiableTypes.MoveableObjects, this);
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


    public override void ActivateManager() {
        TInstantiableObjectSystem.Instance.CurrentManager = Instance;
        if (ghostMovableObject is null) {
            ghostMovableObject = GhostMovableObject.Instance;
        }
        if (activeMovableObjectsType is null) {
            activeMovableObjectsType = movableObjectsTypeSOList[listCounter];
        }
        ghostMovableObject.Activation();
        currentManager = true;
    }
    public override void DeactivateManager() {
        ghostMovableObject.Activation(false);
        currentManager = false;
    }

    private void mouseClickAddFunc() {
        Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
        float maxBuildDistance = 10f;

        if (Mouse3D.GetDistanceToPlayer() >= maxBuildDistance) {
            return;
        }

        Transform newPlacedMoveableObjects = Instantiate(activeMovableObjectsType.transform, mouseWorldPosition, transform.rotation * Quaternion.Euler(0, 45 * angleDiscreetCounter, 0));
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


    public override void OnKeyPressed(object sender, TInstantiableObjectSystem.OnKeyPressedEventArgs e) { 
        if (currentManager) {
            if (e.keyPressed == KeyCode.F) {
                angleDiscreetCounter = 0;
            }
            else if (e.keyPressed == KeyCode.Tab) {
                NextSO();
            }

        }
    }
    public override void OnMouse0(object sender, EventArgs e) {
        if (currentManager) {
            if(!EventSystem.current.IsPointerOverGameObject()) {
                mouseClickAddFunc();
            }
        }
    }
    public override void OnMouse1(object sender, EventArgs e) {
        if (currentManager) {
            angleDiscreetCounter += 1;
            if (angleDiscreetCounter == 8) {
                angleDiscreetCounter = 0;
            }
            print(angleDiscreetCounter);
        }
    }
    public override void OnMouseMid(object sender, EventArgs e) {
        if (currentManager) {

        }
    }
    public override void OnMouseScroll(object sender, TInstantiableObjectSystem.OnMouseScrollEventArgs e) {
        if (currentManager) {

        }
    }

    public float GetMovableObjectEulerY() {
        return looseObjectEulerY;
    }

    public TInstantiableObjectSO GetInstanceableObjectSO() {
        return activeMovableObjectsType;
    }


    public void NextSO() {
        if (activeMovableObjectsType is null) {
            activeMovableObjectsType = movableObjectsTypeSOList[listCounter];
        }
        if (listCounter < movableObjectsTypeSOList.Count - 1) {
            listCounter++;
        }
        else {
            listCounter = 0;
        }
        activeMovableObjectsType = movableObjectsTypeSOList[listCounter];
        OnSelectedChanged.Invoke(this, EventArgs.Empty);
    }
}
