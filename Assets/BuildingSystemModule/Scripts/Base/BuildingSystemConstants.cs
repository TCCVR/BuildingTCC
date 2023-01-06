using UnityEngine;
using System.Collections.Generic;

namespace BuildingSystem {
    public class BuildingSystemConstants :MonoBehaviour {
        public static BuildingSystemConstants Instance { get; private set; }

        public const float UNITSIZE = 1f;
        public const float GRIDHEIGHT = 1f;
        public const int GRIDWIDTH = 1000;
        public const int GRIDDEPTH = 1000;
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