using UnityEngine;
using System;
namespace SensorSystem {
    public class OnKeyPressedEventArgs: EventArgs {
        public KeyCode keyPressed;
    }
    public class OnMouseScrollEventArgs: EventArgs {
        public Vector2 scrollDir;
    }
}