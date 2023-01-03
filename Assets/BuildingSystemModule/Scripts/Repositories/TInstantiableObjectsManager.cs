using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem {
    public abstract class TInstantiableObjectsManager :MonoBehaviour, IPCInputSubscriber, ISwitchBuildingSubscriber {
        public delegate void MouseClickAddIntantiableObjectWithInfo();
        public delegate void AddInstantiableObjectsFromInfo(InstanceInfo bInfo);

        public MouseClickAddIntantiableObjectWithInfo MouseClickAdd;
        public AddInstantiableObjectsFromInfo AddFromInfo;

        public Constants.InstantiableTypes ManagedType { get; protected set; }
        public abstract bool IsBuildingMode { get; }


        [SerializeField] public GameObject InstancesList;
        protected TInstantiableObjectSO currentSO;
        public int listCounter { get; protected set; } = 0;


        /// <summary>
        /// Activates manager functionalities  
        /// </summary>
        public abstract void ActivateManager();
        public abstract void DeactivateManager();

        public abstract void Subs_OnBuildingModeEnable(object sender, EventArgs eventArgs);

        public abstract void Subs_OnBuildingModeDisable(object sender, EventArgs eventArgs);

        public abstract void Subs_OnKeyPressed(object sender, OnKeyPressedEventArgs keyPressedArgs);
        public abstract void Subs_OnMouse0(object sender, EventArgs e);
        public abstract void Subs_OnMouse1(object sender, EventArgs e);
        public abstract void Subs_OnMouseMid(object sender, EventArgs e);
        public abstract void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs e);

    }
}