using UnityEngine;
using System.Collections.Generic;


namespace SensorSystem {
    public class SensorManager :MonoBehaviour {
        public static SensorManager Instance { get; set; }
        public static List<ISensorHandler> Sensors = new List<ISensorHandler>();
        public static List<ISensorSubscriber> SensorServices = new List<ISensorSubscriber>();

        private void Awake() {
            Instance = this;
        }

    }
}