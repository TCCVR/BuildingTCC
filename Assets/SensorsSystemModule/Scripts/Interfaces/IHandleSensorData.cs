using System.Collections.Generic;


namespace SensorSystem {
    public interface IHandleSensorData<TDataSet> {
        public int CacheSizeLimit { get; set; }
        public List<TDataSet> CachedData { get; set; }

        /// <summary>
        /// Unity doesnt work easily with threading so, initially, 
        /// data insertion is not meant to be threadsafe.
        /// </summary>
        /// <param name="data"></param>
        public void InsertDataIntoCache(TDataSet data);

        public void SaveCached();

    }
}
