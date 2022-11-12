using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TInstantiableObjectSystem : MonoBehaviour {

    public static TInstantiableObjectSystem Instance { get; private set; }

    public enum InstantiableTypes {
        GridObjects, //Construções
        GridEdgeObjects, //paredes das construções
        MoveableObjects, //objetos interagiveis
        SensorObjects,   //sensores
    }

    public Dictionary<InstantiableTypes, TInstantiableObjectsManager> Managers;
    public TInstantiableObjectsManager CurrentManager;

    [SerializeField] public Transform playerTransform;

    public event EventHandler<OnKeyPressedEventArgs> OnKeyPressed;
    public class OnKeyPressedEventArgs {
        public KeyCode keyPressed;
    }

    public event EventHandler OnMouse0;
    public event EventHandler OnMouse1;
    public event EventHandler OnMouseMid;
    public event EventHandler OnActiveGridLevelChanged;
    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    public event EventHandler<OnMouseScrollEventArgs> OnMouseScroll;
    public class OnMouseScrollEventArgs {
        public Vector2 scrollDir;
    }


    private void Awake() {
        Instance = this;
        Managers = new Dictionary<InstantiableTypes, TInstantiableObjectsManager>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            OnMouse0.Invoke(this, EventArgs.Empty);
        }
        else if (Input.GetMouseButtonDown(1)) {
            OnMouse1.Invoke(this, EventArgs.Empty);
        }
        else if (Input.GetMouseButtonDown(2)) {
            OnMouseMid.Invoke(this, EventArgs.Empty);
        }
        else if (Input.mouseScrollDelta != Vector2.zero) {
            OnMouseScroll.Invoke(this, new OnMouseScrollEventArgs { scrollDir = Input.mouseScrollDelta });
        }
        else if (Input.anyKeyDown) {
            if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
                SwitchManagers();
            }
            else {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
                    if (!Constants.Instance.USEDKEYS.Contains(key)) {
                        if (Input.GetKeyDown(key)) {
                            OnKeyPressed(this, new OnKeyPressedEventArgs { keyPressed = key });
                            break;
                        };
                    };
                };
            }
        }
    }

    private void SwitchManagers() {
        switch (CurrentManager.managedType) {
            case InstantiableTypes.GridObjects:
                if (Managers.ContainsKey(InstantiableTypes.MoveableObjects)) {
                    CurrentManager.DeactivateManager();
                    Managers[InstantiableTypes.MoveableObjects].ActivateManager();
                }
                break;

            case InstantiableTypes.MoveableObjects:
                if (Managers.ContainsKey(InstantiableTypes.GridObjects)) {
                    CurrentManager.DeactivateManager();
                    Managers[InstantiableTypes.GridObjects].ActivateManager();
                }                
                break;

        }
        Debug.Log("current manager name: " + TInstantiableObjectSystem.Instance.CurrentManager.name);
        return;
    }

};