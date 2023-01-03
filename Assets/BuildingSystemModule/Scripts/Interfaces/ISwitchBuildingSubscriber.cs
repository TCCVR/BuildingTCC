using System;


namespace BuildingSystem {
    interface ISwitchBuildingSubscriber {
        public bool IsBuildingMode { get; }
        public void Subs_OnBuildingModeEnable(object sender, EventArgs eventArgs);
        public void Subs_OnBuildingModeDisable(object sender, EventArgs eventArgs);
    }
}
