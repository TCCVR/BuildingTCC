using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;



namespace SensorSystem {
    interface ISensorSubscriber<TDataSet, TSimpleEventArgs> {

        public void ProcessData(TDataSet dataSet);
        public void SubscribeTo(ISensorHandler<TDataSet, TSimpleEventArgs> handler);
        public void Subs_OnSensorConnect(object sender, EventArgs eventArgs);
        public void Subs_OnSensorDisconnect(object sender, EventArgs eventArgs);
        public void Subs_OnOnSensorParsedData(object sender, EventArgs eventArgs);
        public void Subs_OnSensorSentData(object sender, TSimpleEventArgs eventArgs);
        public void Subs_OnSensorSentLineData(object sender, TSimpleEventArgs eventArgs);


    }
}
