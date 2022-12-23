using System;
namespace BuildingSystem {
    interface IPCInputSubscriber {
        public void Subs_OnKeyPressed(object sender, OnKeyPressedEventArgs e);
        public void Subs_OnMouse0(object sender, EventArgs e);
        public void Subs_OnMouse1(object sender, EventArgs e);
        public void Subs_OnMouseMid(object sender, EventArgs e);
        public void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs e);
    }
}
