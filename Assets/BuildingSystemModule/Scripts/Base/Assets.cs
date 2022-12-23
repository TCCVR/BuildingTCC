using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem {
    public class Assets :MonoBehaviour {
        public static Assets Instance { get; private set; }

        [SerializeField] public List<TInstantiableObjectSO> gridObjectsTypeSOList;
        [SerializeField] public List<TInstantiableObjectSO> movableObjectsTypeSOList;


        void Awake() {
            Instance = this;
        }
    } 
}