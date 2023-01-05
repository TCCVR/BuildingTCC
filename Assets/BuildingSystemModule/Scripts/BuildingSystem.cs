using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

namespace BuildingSystem {
    class BuildingSystem :MonoBehaviour, ISwitchBuildingHandler, IPCInputHandler {
        public static BuildingSystem Instance { get; private set; }

        public bool IsBuildingMode {
            get { return isBuildingMode; }
        }

        [SerializeField] private bool isBuildingMode;
        [SerializeField] public List<KeyCode> USEDKEYS;
        [SerializeField] private Transform playerTransform;
        [SerializeField] public KeyCode SaveKey;
        [SerializeField] public KeyCode BuildingModeLoadKey;
        [SerializeField] public KeyCode GridlessLoadKey;
        public Transform PlayerTransform {
            get { return playerTransform; }
        }

        public event EventHandler<OnKeyPressedEventArgs> OnKeyPressed;
        public event EventHandler<OnMouseScrollEventArgs> OnMouseScroll;
        public event EventHandler OnMouse0;
        public event EventHandler OnMouse1;
        public event EventHandler OnMouseMid;
        public event EventHandler OnEnableSwitch;
        public event EventHandler OnDisableSwitch;

        public void Awake() {
            Instance = this;
        }

        public void Start() {
            if (isBuildingMode) OnEnableSwitch?.Invoke(this, EventArgs.Empty);
            else if (!isBuildingMode) OnDisableSwitch?.Invoke(this, EventArgs.Empty);
        }

        public void Update() {
            if (Input.GetMouseButtonDown(0)) {
                OnMouse0?.Invoke(this, EventArgs.Empty);
            }
            else if (Input.GetMouseButtonDown(1)) {
                OnMouse1?.Invoke(this, EventArgs.Empty);
            }
            else if (Input.GetMouseButtonDown(2)) {
                OnMouseMid?.Invoke(this, EventArgs.Empty);
            }
            else if (Input.mouseScrollDelta != Vector2.zero) {
                OnMouseScroll?.Invoke(this, new OnMouseScrollEventArgs { scrollDir = Input.mouseScrollDelta });
            }
            else if (Input.anyKeyDown) {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
                    if (USEDKEYS.Contains(key)) {
                        if (Input.GetKeyDown(key)) {
                            OnKeyPressed?.Invoke(this, new OnKeyPressedEventArgs { keyPressed = key });
                            break;
                        };
                    };
                };
            }
        }


        public void SetBuildingMode(bool enable) {
            if (enable && (isBuildingMode != true)) {
                isBuildingMode = true;
                OnEnableSwitch?.Invoke(this, EventArgs.Empty);
            }
            else if (!enable && (isBuildingMode != false)) {
                isBuildingMode = false;
                OnDisableSwitch?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
