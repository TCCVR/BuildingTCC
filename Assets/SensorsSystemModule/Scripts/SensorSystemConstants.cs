using System;
using UnityEngine;

namespace SensorSystem {
    [Serializable]
    public class HeartData {
        public int HeartBPM { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class SerialEventArg :EventArgs {
        public string[] data { get; set; }
        public string rawData { get; set; }
    }
    public class SimpleSerialEventArg :EventArgs {
        public string data { get; set; }
    }
    public enum SensorConnectionStatus {
        Connected,
        Disconnected,
        Running,
        NotRunning,
        CoroutineStopped
    }
    public enum ReadWriteMethod {
        Line,
        Char
    }
    public class SensorSystemConstants :MonoBehaviour {

        public static SensorSystemConstants Instance { get; set; }

        void Awake() {
            Instance = this;
        }
    }
}