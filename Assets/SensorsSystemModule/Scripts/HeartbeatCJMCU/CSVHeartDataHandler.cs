using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;


namespace SensorSystem {
    public class CSVHeartDataHandler : IHandleSensorData<HeartData> {
        private static string PATHTOSAVEDATA;
        private static string CSVFILENAME;
        [SerializeField] private int cacheSizeLimit = 100;
        public int CacheSizeLimit {
            get => cacheSizeLimit;
            set => cacheSizeLimit = value; 
        }

        public List<HeartData> CachedData { get; set; } = new List<HeartData>();

        public CSVHeartDataHandler() {
            PATHTOSAVEDATA = Path.Combine(Application.dataPath, "SensorData");
            if (!Directory.Exists(PATHTOSAVEDATA)) {
                Directory.CreateDirectory(PATHTOSAVEDATA);
            }
            GenerateNewSaveName();
        }

        public void InsertDataIntoCache(HeartData data) {
            CachedData.Add(data);
            if (CachedData.Count >= cacheSizeLimit) {
                SaveCached();
            }
        }
        public void GenerateNewSaveName() {
             CSVFILENAME = $"HeartData-{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
        }

        public void SaveCached() {
            try {
                StringBuilder saveString = new StringBuilder();
                foreach (var data in CachedData) {
                    saveString.Append($"{data.HeartBPM},{data.Timestamp.ToString()}{Environment.NewLine}");
                }
                File.AppendAllText(Path.Combine(PATHTOSAVEDATA, CSVFILENAME), saveString.ToString());
                CachedData.Clear();
            }
            catch (Exception ex) {
                Debug.Log($"SensorData CSV Save error: {ex.Message}");
            }
        }

        public void CloseFile() {
            if (CachedData.Count > 0) SaveCached();
            GenerateNewSaveName();
        }

    }
}