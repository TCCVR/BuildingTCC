using UnityEngine;

namespace BuildingSystem {
    public class GameHandler :MonoBehaviour {

        public static GameHandler Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }

    }
}


