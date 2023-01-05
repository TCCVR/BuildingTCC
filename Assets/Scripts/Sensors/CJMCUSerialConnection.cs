using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Threading;
using TMPro;

namespace SensorSystem {
        
    public class CJMCUSerialConnection :MonoBehaviour, ISensorHandler<HeartData, SimpleSerialEventArg>,
                                    ISerialConfiguration {

        public static CJMCUSerialConnection Instance;
        public SensorConnectionStatus Status { get; set; }

        private bool isRunning = false;
        public bool IsRunning {
            get { return isRunning; }
            set { isRunning = value; }
        }

        private SerialPort serialPort;
        public SerialPort SerialPort { get; set; }
        [Header("SerialPort")]
        private string comPort = "COM3";
        public string ComPort {
            get { return comPort; }
            set { comPort = value; }
        }
        private int baudRate = 9600;
        public int BaudRate {
            get { return baudRate; }
            set { baudRate = value; }
        }
        private Parity parity = Parity.None;
        public Parity Parity {
            get { return parity; }
            set { parity = value; }
        }
        private StopBits stopBits = StopBits.None;
        public StopBits StopBits {
            get { return stopBits; }
            set { stopBits = value; }
        }
        private int dataBits = 8;
        public int DataBits {
            get { return dataBits; }
            set { dataBits = value; }
        }
        private bool dtrEnable = false;
        public bool DtrEnable {
            get { return dtrEnable; }
            set { dtrEnable = value; }
        }
        private bool rtsEnable = false;
        public bool RtsEnable {
            get { return rtsEnable; }
            set { rtsEnable = value; }
        }
        private int readTimeout = 10;
        public int ReadTimeout {
            get { return readTimeout; }
            set { readTimeout = value; }
        }
        private int writeTimeout = 10;
        public int WriteTimeout {
            get { return writeTimeout; }
            set { writeTimeout = value; }
        }


        private string rawData = "Ready";
        public string RawData {
            get { return rawData; }
            set { rawData = value; }
        }

        private string[] chunkData;
        public string[] ChunkData {
            get { return chunkData; }
            set { chunkData = value; }
        }

        public HeartData SingleDataSet { get; set; } = new HeartData();

        public event EventHandler OnSensorParsedData;
        public event EventHandler OnSensorConnect;
        public event EventHandler OnSensorDisconnect;
        public event EventHandler<SimpleSerialEventArg> OnSensorSentData;
        public event EventHandler<SimpleSerialEventArg> OnSensorSentLineData;

        [Header("Data Read")]
        private ReadWriteMethod readWriteMethod = ReadWriteMethod.Char;
        public ReadWriteMethod ReadWriteDataMethod {
            get { return readWriteMethod; }
            set { readWriteMethod = value; }
        }

        private string delimiter = Environment.NewLine;
        public string Delimiter {
            get { return delimiter; }
            set { delimiter = value; }
        }
        private char separator = ',';
        public char Separator {
            get { return separator; }
            set { separator = value; }
        }

        void Awake() {
            Instance = this;
        }

        void Start() {
            OnSensorConnect +=
                UnitySerialPort_SerialPortOpenEvent;

            OnSensorDisconnect +=
                UnitySerialPort_SerialPortCloseEvent;

            OnSensorSentData +=
                UnitySerialPort_SerialPortSentDataEvent;

            OnSensorSentLineData +=
                UnitySerialPort_SerialPortSentLineDataEvent;

            OnSensorParsedData +=
                UnitySerialPort_SerialDataParseEvent;
            OpenSerialPort();
            HeartbeatGraph.Instance.SubscribeTo(this);
        }

        void OnDestroy() {
            if (OnSensorParsedData != null)
                OnSensorParsedData -= UnitySerialPort_SerialDataParseEvent;
            if (OnSensorConnect != null)
                OnSensorConnect -= UnitySerialPort_SerialPortOpenEvent;
            if (OnSensorDisconnect != null)
                OnSensorDisconnect -= UnitySerialPort_SerialPortCloseEvent;
            if (OnSensorSentData != null)
                OnSensorSentData -= UnitySerialPort_SerialPortSentDataEvent;
            if (OnSensorSentLineData != null)
                OnSensorSentLineData -= UnitySerialPort_SerialPortSentLineDataEvent;
        }

        void Update() {
            if (serialPort == null || serialPort.IsOpen == false) { return; }
        }

        void OnApplicationQuit() {
            CloseSerialPort();
            Thread.Sleep(100);
            StopCoroutine();
            Thread.Sleep(100);
        }


        void UnitySerialPort_SerialDataParseEvent(object o, EventArgs e) {
            Debug.Log($"Data Recieved via port: {SingleDataSet.HeartBPM}");
        }

        void UnitySerialPort_SerialPortOpenEvent(object o, EventArgs e) {
            Status = SensorConnectionStatus.Connected;
            Debug.Log(Status);
        }

        void UnitySerialPort_SerialPortCloseEvent(object o, EventArgs e) {
            Status = SensorConnectionStatus.Disconnected;
            Debug.Log(Status);
        }

        void UnitySerialPort_SerialPortSentDataEvent(object o, SimpleSerialEventArg e) {
            Status = SensorConnectionStatus.Running;
            Debug.Log(Status);
        }

        void UnitySerialPort_SerialPortSentLineDataEvent(object o, SimpleSerialEventArg e) {
            Status = SensorConnectionStatus.Running;
            Debug.Log(Status);
        }


        public void OpenSerialPort() {
            try {
                serialPort = new SerialPort(ComPort, BaudRate, Parity, DataBits, StopBits);
                serialPort.ReadTimeout = ReadTimeout;
                serialPort.WriteTimeout = WriteTimeout;
                serialPort.DtrEnable = DtrEnable;
                serialPort.RtsEnable = RtsEnable;
                serialPort.Open();
                if (isRunning) {
                    StopCoroutine();
                }
                StartCoroutine();
                Status = SensorConnectionStatus.Connected;
                Debug.Log(Status);
            }
            catch (Exception ex) {
                Debug.Log("Error 1: " + ex.Message.ToString());
            }
            OnSensorConnect?.Invoke(this, EventArgs.Empty);
        }

        public void CloseSerialPort() {
            StopCoroutine();
            Status = SensorConnectionStatus.Disconnected;
            Debug.Log(Status);
            OnSensorDisconnect?.Invoke(this, EventArgs.Empty);
        }

        public void StartCoroutine() {
            isRunning = true;
            StartCoroutine("CoroutineLoop");
        }

        public IEnumerator CoroutineLoop() {
            while (isRunning) {
                SerialCheckData();
                //yield return null;
                yield return new WaitForSeconds(.20f);
            }
            Status = SensorConnectionStatus.CoroutineStopped;
            Debug.Log(Status);
        }

        public void StopCoroutine() {
            isRunning = false;
            Thread.Sleep(100);
            try {
                StopCoroutine("CoroutineLoop");
            }
            catch (Exception ex) {
                Debug.Log("Error 2A: " + ex.Message.ToString());
            }
            if (serialPort != null) { serialPort = null; }
            Debug.Log("Ended Serial Loop Coroutine!");
        }

        private void SerialCheckData() {
            try {
                if (serialPort.IsOpen) {
                    string rData = string.Empty;
                    switch (readWriteMethod) {
                        case ReadWriteMethod.Line:
                            rData = serialPort.ReadLine();
                            break;
                        case ReadWriteMethod.Char:
                            rData = serialPort.ReadTo(Delimiter);
                            break;
                    }
                    if (rData != null && rData != "") {
                        RawData = rData;
                        ChunkData = RawData.Split(Separator);
                        if (int.TryParse(rawData, out int BPM)) {
                            SingleDataSet.HeartBPM = BPM;
                            SingleDataSet.Timestamp = DateTime.Now;
                        }
                        OnSensorParsedData?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (TimeoutException) {
            }
            catch (Exception ex) {
                if (serialPort.IsOpen) {
                    Debug.Log("Error 4: " + ex.Message.ToString());
                }
                else {
                    Debug.Log("Error 5: Port Closed Exception!");
                }
            }
        }

        public void SendData(string data) {
            if (serialPort != null) {
                if (ReadWriteDataMethod == ReadWriteMethod.Line) {
                    serialPort.WriteLine(data);
                    try {
                        //OnSensorSentLineData?.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception ex) {
                        // Failed to open com port or start serial thread
                        Debug.Log("Error OnSensorSentLineData: " + ex.Message.ToString());
                    }
                }
                else if (ReadWriteDataMethod == ReadWriteMethod.Char) {
                    serialPort.Write(data);
                    try {
                        //OnSensorSentData?.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception ex) {
                        // Failed to open com port or start serial thread
                        Debug.Log("Error OnSensorSentData: " + ex.Message.ToString());
                    }

                }
            }
        }

    }

}