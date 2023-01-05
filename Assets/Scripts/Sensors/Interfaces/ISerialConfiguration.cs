using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;

namespace SensorSystem {
    interface ISerialConfiguration {
        public SerialPort SerialPort { get; set; }
        /// <summary>
        /// Default "COM5"
        /// </summary>
        public string ComPort { get; set; }

        /// <summary>
        /// Current baud rate and set of default
        /// 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200
        /// </summary>
        public int BaudRate { get; set; }
        /// <summary>
        /// Default Parity.None
        /// </summary>
        public Parity Parity { get; set; }
        /// <summary>
        /// Default StopBits.None;
        /// </summary>
        public StopBits StopBits { get; set; }
        /// <summary>
        /// Default 8
        /// </summary>
        public int DataBits { get; set; }
        /// <summary>
        /// The state of the Data Terminal Ready(DTR) signal during serial communication.
        /// </summary>
        public bool DtrEnable { get; set; }
        /// <summary>
        /// Whether or not the Request to Send(RTS) signal is enabled during serial communication.
        /// </summary>
        public bool RtsEnable { get; set; }
        /// <summary>
        /// Default 10
        /// </summary>
        public int ReadTimeout { get; set; }
        /// <summary>
        /// Default 10
        /// </summary>
        public int WriteTimeout { get; set; }
        public ReadWriteMethod ReadWriteDataMethod { get; set; }

        public string Delimiter { get; set; }
        public char Separator { get; set; }

        public void OpenSerialPort();
        public void CloseSerialPort();
    }
}
