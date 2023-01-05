using System;
using System.Collections.Generic;
using System.Linq;


namespace SensorSystem {
    internal class IntGraphValues {
        private List<int> list;
        public int this[int index] {
            get {
                if (index >= list.Count) return Average;
                return list[index];
            }
            set {
                if (index >= maxSize) InsertValue(value);
                else if (index < 0) return;
                else list[index] = value;
            }
        }
        private int maxSize;
        public int MaxSize {
            get => maxSize;
        }
        public int MaxValue {
            get {
                if (list.Count == 0) return 0;
                return list.Max();
            }
        }
        public int MinValue {
            get {
                if (list.Count == 0) return 0;
                return list.Min();
            }
        }
        public int Average {
            get {
                if (list.Count == 0) return 0;
                return (int)Math.Truncate(list.Average());
            }
        }
        public IntGraphValues(int size) {
            maxSize = size;
            list = new List<int>();
        }

        public int Count {
            get => list.Count;
        }

        public int? InsertValue(int value) {
            if (list.Count >= maxSize) {
                int result = list[0];
                list.RemoveAt(0);
                list.Add(value);
                return result;
            }
            else {
                list.Add(value);
                return null;
            }
        }

        public void Resize(int newSize) {
            if (newSize > maxSize) {
                int difference = newSize - maxSize;
                for (int i = 0; i < difference; i++) list.RemoveAt(0);
            }
            maxSize = newSize;
        }
    }
}
