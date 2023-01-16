using System;
using System.Collections.Generic;
using UnityEngine;

namespace SensorSystem {
    public class SensorSystem :MonoBehaviour, IPCInputHandler {
        public static SensorSystem Instance { get; set; }

        [SerializeField] public List<KeyCode> USEDKEYS;
        public event EventHandler<OnKeyPressedEventArgs> OnKeyPressed;
        public event EventHandler<OnMouseScrollEventArgs> OnMouseScroll;
        public event EventHandler OnMouse0;
        public event EventHandler OnMouse1;
        public event EventHandler OnMouseMid;

        private void Awake() {
            Instance = this;
        }
        void Start() {

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
                            //Debug.Log($"KeyPressed: {key}");
                            OnKeyPressed?.Invoke(this, new OnKeyPressedEventArgs { keyPressed = key });
                            break;
                        };
                    };
                };
            }
        }
    }
}