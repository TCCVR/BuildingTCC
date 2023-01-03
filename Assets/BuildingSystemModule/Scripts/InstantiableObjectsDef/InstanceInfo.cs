using System;
using UnityEngine;


namespace BuildingSystem {
    [Serializable]
    public class MyVector3 {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector3 ToVector3() {
            return new Vector3(x, y, z);
        }
        public Quaternion ToQuaternion() {
            return new Quaternion(x, y, z, w);
        }
    }


    [Serializable]
    public class SaveObject {
        public playerCreatedScenario playerCreatedScenario {
            set;
            get;
        }
    }

    [Serializable]
    public class playerCreatedScenario {
        public MoveableObjects moveableObjects {
            set;
            get;
        }
        public GridObjects gridObjects {
            set;
            get;
        }

    }

    [Serializable]
    public class MoveableObjects {
        public InstanceInfo[] placedMoveableInfo {
            set;
            get;
        }
    }

    [Serializable]
    public class GridObjects {
        public InstanceInfo[] placedGridInfo {
            set;
            get;
        }
    }

    [Serializable]
    public class InstanceInfo {
        public Constants.InstantiableTypes SOType;
        public string SOName;
        public string instanceName;
        public MyVector3 position;
        public MyVector3 rotation;
        public MyVector3 scale;
        public Constants.Dir dir;
        public int width;
        public int height;
    }
}