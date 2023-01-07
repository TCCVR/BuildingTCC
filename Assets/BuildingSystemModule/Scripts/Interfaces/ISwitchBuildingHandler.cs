using System;

namespace BuildingSystem {
    interface ISwitchBuildingHandler{
        public bool IsBuildingMode { get; }
        public void SetBuildingMode(bool enable);
        public event EventHandler OnEnableSwitch;
        public event EventHandler OnDisableSwitch;
    }
}
