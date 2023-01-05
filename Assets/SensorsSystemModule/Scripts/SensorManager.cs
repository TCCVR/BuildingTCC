using UnityEngine;
using System.Collections.Generic;


namespace SensorSystem {
    public class SensorManager :MonoBehaviour {
        public static SensorManager Instance { get; set; }
        public static List<ISensorHandler> Sensors = new List<ISensorHandler>();

        private void Awake() {
            Instance = this;
        }

        void Start() {

        }

        void Update() {

        }
    }
}