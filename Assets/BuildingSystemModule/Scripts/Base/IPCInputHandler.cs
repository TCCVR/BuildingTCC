using System;
namespace BuildingSystem {
    interface IPCInputHandler {
        public event EventHandler<OnKeyPressedEventArgs> OnKeyPressed;
        public event EventHandler<OnMouseScrollEventArgs> OnMouseScroll;
        public event EventHandler OnMouse0;
        public event EventHandler OnMouse1;
        public event EventHandler OnMouseMid;
    }
}