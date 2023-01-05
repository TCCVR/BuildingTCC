using System;
using System.Collections.Generic;
using System.Linq;


namespace SensorSystem {
    internal class IntGraphValues {
        private List<int> list;
        public int this[int index] {
            get {
                if (index >= list.Count) return average;
                return list[index];
            }
            set {
                list[index] = value;
            }
        }
        private int maxSize;
        public int MaxSize {
            get => maxSize;
        }
        private int maxValue = default;
        public int MaxValue {
            get => maxValue;
        }
        private int minValue = default;
        public int MinValue {
            get => minValue;
        }
        private int average = default;
        public int Average {
            get => average;
        }
        public IntGraphValues(int size) {
            maxSize = size;
            list = new List<int>();
        }

        public int Count {
            get => list.Count;
        }

        public int? InsertValue(int value) {
            if (list.Count == 0) {
                maxValue = value;
                minValue = value;
            }
            else if (value > maxValue) maxValue = value;
            else if (value < minValue) minValue = value;
            if (list.Count >= maxSize) {
                int result = list[0];
                list.RemoveAt(0);
                list.Add(value);
                average = (int)Math.Truncate(((decimal)list.Sum())/list.Count);
                return result;
            }
            else {
                list.Add(value);
                average = (int)Math.Truncate(((decimal)list.Sum()) / list.Count);
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
