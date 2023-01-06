using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Threading;



namespace SensorSystem {
    public abstract class SerialConnection :MonoBehaviour, ISensorHandler {

        public SensorConnectionStatus Status { get; set; }

        private bool isRunning = false;
        public bool IsRunning {
            get { return isRunning; }
            set { isRunning = value; }
        }

        protected string rawData = string.Empty;
        public string RawData {
            get { return rawData; }
            set { rawData = value; }
        }

        protected string[] chunkData = Array.Empty<string>();
        public string[] ChunkData {
            get { return chunkData; }
            set { chunkData = value; }
        }

        private SerialPort serialPort;
        public SerialPort SerialPort { get; set; }
        [Header("SerialPort")]
        [SerializeField]
        private string comPort = "COM3";
        /// <summary>
        /// Default "COM5"
        /// </summary>
        public string ComPort {
            get { return comPort; }
            set { comPort = value; }
        }

        [SerializeField]
        private int baudRate = 9600;
        /// <summary>
        /// Current baud rate and set of default
        /// 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200
        /// </summary>
        public int BaudRate {
            get { return baudRate; }
            set { baudRate = value; }
        }
        [SerializeField]
        private Parity parity = Parity.None;
        /// <summary>
        /// Default Parity.None
        /// </summary>
        public Parity Parity {
            get { return parity; }
            set { parity = value; }
        }
        [SerializeField]
        private StopBits stopBits = StopBits.None;
        /// <summary>
        /// Default StopBits.None;
        /// </summary>
        public StopBits StopBits {
            get { return stopBits; }
            set { stopBits = value; }
        }
        [SerializeField]
        private int dataBits = 8;
        /// <summary>
        /// Default 8
        /// </summary>
        public int DataBits {
            get { return dataBits; }
            set { dataBits = value; }
        }
        [SerializeField]
        private bool dtrEnable = false;
        /// <summary>
        /// The state of the Data Terminal Ready(DTR) signal during serial communication.
        /// </summary>
        public bool DtrEnable {
            get { return dtrEnable; }
            set { dtrEnable = value; }
        }
        [SerializeField]
        private bool rtsEnable = false;
        /// <summary>
        /// Whether or not the Request to Send(RTS) signal is enabled during serial communication.
        /// </summary>
        public bool RtsEnable {
            get { return rtsEnable; }
            set { rtsEnable = value; }
        }
        [SerializeField]
        private int readTimeout = 10;
        /// <summary>
        /// Default 10
        /// </summary>
        public int ReadTimeout {
            get { return readTimeout; }
            set { readTimeout = value; }
        }
        [SerializeField]
        private int writeTimeout = 10;
        /// <summary>
        /// Default 10
        /// </summary>
        public int WriteTimeout {
            get { return writeTimeout; }
            set { writeTimeout = value; }
        }
        [Header("Data Read")]
        [SerializeField]
        private ReadWriteMethod readWriteMethod = ReadWriteMethod.Char;
        /// <summary>
        /// Default Char
        /// </summary>
        public ReadWriteMethod ReadWriteDataMethodChoice {
            get { return readWriteMethod; }
            set { readWriteMethod = value; }
        }

        [SerializeField]
        private string delimiter = Environment.NewLine;
        public string Delimiter {
            get { return delimiter; }
            set { delimiter = value; }
        }
        [SerializeField]
        private char separator = ',';
        public char Separator {
            get { return separator; }
            set { separator = value; }
        }

        public abstract event EventHandler OnSensorParsedData;
        public abstract event EventHandler OnSensorConnect;
        public abstract event EventHandler OnSensorDisconnect;
        public abstract event EventHandler OnSensorSentData;

        public abstract void OpenSerialPort();

        public abstract void CloseSerialPort();
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
            //Debug.Log(Status);
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
            //Debug.Log("Ended Serial Loop Coroutine!");
        }
        protected abstract void SerialCheckData();
        public abstract void SendData(string data);
    }
    //public override void OpenSerialPort() {
    //    try {
    //        SerialPort = new SerialPort(ComPort, BaudRate, Parity, DataBits, StopBits);
    //        SerialPort.ReadTimeout = ReadTimeout;
    //        SerialPort.WriteTimeout = WriteTimeout;
    //        SerialPort.DtrEnable = DtrEnable;
    //        SerialPort.RtsEnable = RtsEnable;
    //        SerialPort.Open();
    //        if (IsRunning) {
    //            StopCoroutine();
    //        }
    //        StartCoroutine();
    //        Status = SensorConnectionStatus.Connected;
    //        //Debug.Log(Status);
    //    }
    //    catch (Exception ex) {
    //        Debug.Log("Error 1: " + ex.Message.ToString());
    //    }
    //    OnSensorConnect?.Invoke(this, EventArgs.Empty);
    //}
    //public override void CloseSerialPort() {
    //    StopCoroutine();
    //    Status = SensorConnectionStatus.Disconnected;
    //    //Debug.Log(Status);
    //    OnSensorDisconnect?.Invoke(this, EventArgs.Empty);
    //}

    //protected override void SerialCheckData() {
    //    try {
    //        if (SerialPort.IsOpen) {
    //            string rData = string.Empty;
    //            switch (ReadWriteDataMethodChoice) {
    //                case ReadWriteMethod.Line:
    //                    rData = SerialPort.ReadLine();
    //                    break;
    //                case ReadWriteMethod.Char:
    //                    rData = SerialPort.ReadTo(Delimiter);
    //                    break;
    //            }
    //            if (rData != null && rData != "") {
    //                RawData = rData;
    //                ChunkData = RawData.Split(Separator);
    //                OnSensorParsedData?.Invoke(this, EventArgs.Empty);
    //            }
    //        }
    //    }
    //    catch (TimeoutException) {
    //    }
    //    catch (Exception ex) {
    //        if (SerialPort.IsOpen) {
    //            Debug.Log("Error 4: " + ex.Message.ToString());
    //        }
    //        else {
    //            Debug.Log("Error 5: Port Closed Exception!");
    //        }
    //    }
    //}
    //public override void SendData(string data) {
    //    if (SerialPort != null) {
    //        if (ReadWriteDataMethodChoice == ReadWriteMethod.Line) {
    //            SerialPort.WriteLine(data);
    //            try {
    //                OnSensorSentData?.Invoke(this, new SimpleSerialEventArg() { data = data });
    //            }
    //            catch (Exception ex) {
    //                // Failed to open com port or start serial thread
    //                Debug.Log("Error OnSensorSentLineData: " + ex.Message.ToString());
    //            }
    //        }
    //        else if (ReadWriteDataMethodChoice == ReadWriteMethod.Char) {
    //            SerialPort.Write(data);
    //            try {
    //                OnSensorSentData?.Invoke(this, new SimpleSerialEventArg() { data = data });
    //            }
    //            catch (Exception ex) {
    //                // Failed to open com port or start serial thread
    //                Debug.Log("Error OnSensorSentData: " + ex.Message.ToString());
    //            }
    //        }
    //    }
    //}
}
