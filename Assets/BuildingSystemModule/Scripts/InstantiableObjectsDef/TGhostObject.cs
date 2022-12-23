using UnityEngine;

namespace BuildingSystem {
    public abstract class TGhostObject :MonoBehaviour {
        protected Transform visual;
        protected bool active;


        private void LateUpdate() { //LateUpdate
            if (active) {
                GhostLateUpdate();
            }
        }

        public void Activation(bool activation = true) {
            active = activation;
            RefreshVisual();
        }

        protected void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
            RefreshVisual();
        }
        protected void SetLayerRecursive(GameObject targetGameObject, int layer) {
            targetGameObject.layer = layer;
            foreach (Transform child in targetGameObject.transform) {
                SetLayerRecursive(child.gameObject, layer);
            }
        }


        protected abstract void RefreshVisual();
        protected abstract void GhostLateUpdate();

    }
}