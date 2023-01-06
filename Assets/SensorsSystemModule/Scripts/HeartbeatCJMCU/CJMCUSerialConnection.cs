using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;


namespace SensorSystem {
        
    public class CJMCUSerialConnection :SerialConnection, ISensorHandler,
                                    IPCInputSubscriber {

        public static CJMCUSerialConnection Instance { get; private set; }

        public override event EventHandler OnSensorParsedData;
        public override event EventHandler OnSensorConnect;
        public override event EventHandler OnSensorDisconnect;
        public override event EventHandler OnSensorSentData;


        void Awake() {
            Instance = this;
        }

        void Start() {
            HeartbeatGraph.Instance.SubscribeTo(this);
            SensorManager.Sensors.Add(this);
            SensorSystem.Instance.OnKeyPressed += Subs_OnKeyPressed;
        }

        void OnDestroy() {
            HeartbeatGraph.Instance.UnsubscribeTo(this);
        }

        void OnApplicationQuit() {
            CloseSerialPort();
            Thread.Sleep(100);
            StopCoroutine();
            Thread.Sleep(100);
        }

        public override void OpenSerialPort() {
            try {
                SerialPort = new SerialPort(ComPort, BaudRate, Parity, DataBits, StopBits);
                SerialPort.ReadTimeout = ReadTimeout;
                SerialPort.WriteTimeout = WriteTimeout;
                SerialPort.DtrEnable = DtrEnable;
                SerialPort.RtsEnable = RtsEnable;
                SerialPort.Open();
                if (IsRunning) {
                    StopCoroutine();
                }
                StartCoroutine();
                Status = SensorConnectionStatus.Connected;
            }
            catch (Exception ex) {
                Debug.Log("Error 1: " + ex.Message.ToString());
            }
            OnSensorConnect?.Invoke(this, EventArgs.Empty);
        }

        public override void CloseSerialPort() {
            StopCoroutine();
            Status = SensorConnectionStatus.Disconnected;
            OnSensorDisconnect?.Invoke(this, EventArgs.Empty);
        }
        protected override void SerialCheckData() {
            try {
                if (SerialPort.IsOpen) {
                    string rData = string.Empty;
                    switch (ReadWriteDataMethodChoice) {
                        case ReadWriteMethod.Line:
                            rData = SerialPort.ReadLine();
                            break;
                        case ReadWriteMethod.Char:
                            rData = SerialPort.ReadTo(Delimiter);
                            break;
                    }
                    if (rData != null && rData != "") {
                        RawData = rData;
                        ChunkData = RawData.Split(Separator);
                        OnSensorParsedData?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (TimeoutException) {
            }
            catch (Exception ex) {
                if (SerialPort.IsOpen) {
                    Debug.Log("Error 4: " + ex.Message.ToString());
                }
                else {
                    Debug.Log("Error 5: Port Closed Exception!");
                }
            }
        }

        public override void SendData(string data) {
            if (SerialPort != null) {
                if (ReadWriteDataMethodChoice == ReadWriteMethod.Line) {
                    SerialPort.WriteLine(data);
                    try {
                        OnSensorSentData?.Invoke(this, new SimpleSerialEventArg() { data = data });
                    }
                    catch (Exception ex) {
                        // Failed to open com port or start serial thread
                        Debug.Log("Error OnSensorSentLineData: " + ex.Message.ToString());
                    }
                }
                else if (ReadWriteDataMethodChoice == ReadWriteMethod.Char) {
                    SerialPort.Write(data);
                    try {
                        OnSensorSentData?.Invoke(this, new SimpleSerialEventArg() { data = data });
                    }
                    catch (Exception ex) {
                        // Failed to open com port or start serial thread
                        Debug.Log("Error OnSensorSentData: " + ex.Message.ToString());
                    }

                }
            }
        }

        public void Subs_OnKeyPressed(object sender, OnKeyPressedEventArgs keyPressedArgs) {
            if (keyPressedArgs.keyPressed == KeyCode.F4) {
                if (!IsRunning) OpenSerialPort();
                else CloseSerialPort();
            }
        }
        public void Subs_OnMouse0(object sender, EventArgs eventArgs) { }

        public void Subs_OnMouse1(object sender, EventArgs eventArgs) { }

        public void Subs_OnMouseMid(object sender, EventArgs eventArgs) { }

        public void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs eventArgs) { }




    }

}