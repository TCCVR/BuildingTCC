using System.Collections;
using UnityEngine;

namespace BuildingSystem {
    public class RaycastPoint :MonoBehaviour {
        public static RaycastPoint Instance { get; private set; }

        [SerializeField] public LayerMask mouseColliderLayerMask = new LayerMask() { value = 1 };
        public static Vector3 PointPosition {
            get {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                return (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, Instance.mouseColliderLayerMask)) ?
                  raycastHit.point : Vector3.zero;
            }
        }
        public static float DistanceFromCamera {
            get =>
                (BuildingSystem.Instance.PlayerTransform != null)?
                Vector3.Distance(BuildingSystem.Instance.PlayerTransform.position, PointPosition)
                : -1; 
        }


        private void Awake() {
                Instance = this;
        }

        private void Start() {
            StartCoroutine();
        }

        private void OnDestroy() {
            StopCoroutine();
        }
        public void StartCoroutine() {
            StartCoroutine("CoroutineLoop");
        }

        public IEnumerator CoroutineLoop() {
            while (true) {
                UpdateRaycastPoint();
                yield return new WaitForSeconds(.1f);
            }
        }
        public void StopCoroutine() {
            StopCoroutine("CoroutineLoop");
        }

        private void UpdateRaycastPoint() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask)) {
                transform.position = raycastHit.point;
            }
        }

    }
}
