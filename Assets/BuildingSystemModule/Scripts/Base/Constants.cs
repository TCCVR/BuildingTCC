using UnityEngine;
using System.Collections.Generic;

namespace BuildingSystem {
    public class Constants :MonoBehaviour {

        //public static Constants Instance { get; private set; }
        public const float CELLSIZE = 1f;
        public const float GRIDVERTICALSIZE = 2.5f;
        public const int GRIDWIDTH = 1000;
        public const int GRIDHEIGHT = 1000;
        public const int GRIDVERTICALCOUNT = 4;
        public const float MAXBUILDINGDISTANCE = 10f;
        public static List<KeyCode> USEDKEYS = new List<KeyCode>();
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
            //Instance = this;
            USEDKEYS.Add(KeyCode.W);
            USEDKEYS.Add(KeyCode.A);
            USEDKEYS.Add(KeyCode.S);
            USEDKEYS.Add(KeyCode.D);
            USEDKEYS.Add(KeyCode.Space);
        }
    }
}