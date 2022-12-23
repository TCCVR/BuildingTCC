using UnityEngine;

namespace BuildingSystem {
    public class Mouse3D :MonoBehaviour {
        public static Mouse3D Instance { get; private set; }

        [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();
        [SerializeField] private Transform playerTransform;

        private void Awake() {
            Instance = this;
        }

        private void Update() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask)) {
                transform.position = raycastHit.point;
            }
        }

        /// <summary>
        /// GetMouseWorldPosition get a vector3 of where is rayscast   
        /// </summary>
        public static Vector3 GetMouseWorldPosition() => Instance.GetMouseWorldPosition_Instance();

        public static float GetDistanceToPlayer() => Instance.GetDistanceToPlayer_Instance();
        private float GetDistanceToPlayer_Instance() {
            return Vector3.Distance(playerTransform.position, Instance.GetMouseWorldPosition_Instance());
        }

        private Vector3 GetMouseWorldPosition_Instance() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask)) {
                return raycastHit.point;
            }
            else {
                return Vector3.zero;
            }
        }

    }
}
