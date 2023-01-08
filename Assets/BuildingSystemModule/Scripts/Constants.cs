using UnityEngine;

namespace BuildingSystem {
    public class Constants :MonoBehaviour {
        public static Constants Instance { get; private set; }

        public const float UNITSIZE = 1f;
        public const int GRIDWIDTH = 1000;
        public const int GRIDDEPTH = 1000;
        public const float GROUNDLEVELOFFSET = 0f;
        public const int QNTYGRIDLEVELS = 10;
        public const float MAXBUILDINGDISTANCE = 10f;
        public enum InstantiableTypes {
            GridObjects, //Construções
            GridEdgeObjects, //paredes das construções
            MoveableObjects, //objetos interagiveis
            SensorObjects,   //sensores
        }
        public enum Dir {
            NotFixed,
            Down,
            Left,
            Up,
            Right,
        }


        private void Awake() {
            Instance = this;
        }
    }
}