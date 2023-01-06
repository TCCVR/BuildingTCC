using UnityEngine;

namespace BuildingSystem {
    public abstract class TSaveManager : MonoBehaviour {
        protected abstract void Save();
        protected abstract void LoadObjects();
    }
}
