using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;



namespace SensorSystem {
    public interface ISensorHandler {
        public SensorConnectionStatus Status { get; set; }
        public bool IsRunning { get; set; }
        public string RawData { get; set; }

        public event EventHandler OnSensorConnect;
        public event EventHandler OnSensorDisconnect;
        public event EventHandler OnSensorParsedData;
        public event EventHandler OnSensorSentData;
        public IEnumerator CoroutineLoop();
        public void StartCoroutine();
        public void StopCoroutine();
        public void SendData(string data);

    }
}
