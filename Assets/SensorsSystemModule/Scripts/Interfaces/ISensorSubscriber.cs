using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;



namespace SensorSystem {
    public interface ISensorSubscriber {

        public void ProcessData(string dataSet);
        public void SubscribeTo(ISensorHandler handler);
        public void UnsubscribeTo(ISensorHandler handler);
        public void Subs_OnSensorConnect(object sender, EventArgs eventArgs);
        public void Subs_OnSensorDisconnect(object sender, EventArgs eventArgs);
        public void Subs_OnOnSensorParsedData(object sender, EventArgs eventArgs);
        public void Subs_OnSensorSentData(object sender, EventArgs eventArgs);
        public void Subs_OnSensorSentLineData(object sender, EventArgs eventArgs);


    }
}
