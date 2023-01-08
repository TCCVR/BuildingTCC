using System;
using UnityEngine;

namespace SensorSystem {
    public class HeartbeatGraphDot {

        public event EventHandler OnChangedGraphVisualObjectInfo;

        private GameObject dotGameObject;
        private GameObject dotConnectionGameObject;
        private HeartbeatGraphDot lastVisualObject;

        public HeartbeatGraphDot(GameObject dotGameObject, GameObject dotConnectionGameObject, HeartbeatGraphDot lastVisualObject) {
            this.dotGameObject = dotGameObject;
            this.dotConnectionGameObject = dotConnectionGameObject;
            this.lastVisualObject = lastVisualObject;

            if (lastVisualObject != null) {
                lastVisualObject.OnChangedGraphVisualObjectInfo += LastVisualObject_OnChangedGraphVisualObjectInfo;
            }
        }

        private void LastVisualObject_OnChangedGraphVisualObjectInfo(object sender, EventArgs e) {
            UpdateDotConnection();
        }

        public void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText) {
            RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = graphPosition;
            UpdateDotConnection();
            if (OnChangedGraphVisualObjectInfo != null) OnChangedGraphVisualObjectInfo(this, EventArgs.Empty);
        }

        public void CleanUp() {
            UnityEngine.Object.Destroy(dotGameObject);
            UnityEngine.Object.Destroy(dotConnectionGameObject);
        }

        public Vector2 GetGraphPosition() {
            RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
            return rectTransform.anchoredPosition;
        }

        public static float GetAngleFromVectorFloat(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }
        private void UpdateDotConnection() {
            if (dotConnectionGameObject != null) {
                RectTransform dotConnectionRectTransform = dotConnectionGameObject.GetComponent<RectTransform>();
                Vector2 dir = (lastVisualObject.GetGraphPosition() - GetGraphPosition()).normalized;
                float distance = Vector2.Distance(GetGraphPosition(), lastVisualObject.GetGraphPosition());
                dotConnectionRectTransform.sizeDelta = new Vector2(distance, 3f);
                dotConnectionRectTransform.anchoredPosition = GetGraphPosition() + dir * distance * .5f;
                dotConnectionRectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
            }
        }
    }
    
}
