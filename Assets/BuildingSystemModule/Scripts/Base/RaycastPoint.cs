using System;
using System.Collections;
using UnityEngine;

namespace BuildingSystem {
    public class RaycastPoint :MonoBehaviour, ISwitchBuildingSubscriber {
        public static RaycastPoint Instance { get; private set; }
        public bool IsBuildingMode {
            get {
                if (BuildingSystem.Instance != null)
                    return BuildingSystem.Instance.IsBuildingMode;
                else return false;
            }
        }

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
            StartCoroutine("InitAfterBuildingModeOn");
        }

        private void OnDestroy() {
            StopCoroutine();
        }

        private IEnumerable InitAfterBuildingModeOn() {
            yield return new WaitUntil(() => BuildingSystem.Instance.IsBuildingMode);
            StartCoroutine();
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

        public void Subs_OnBuildingModeEnable(object sender, EventArgs eventArgs) {

        }

        public void Subs_OnBuildingModeDisable(object sender, EventArgs eventArgs) {

        }
    }
}
