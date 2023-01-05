using UnityEngine;
using UnityEngine.UI;

namespace SensorSystem {
    /*
     * Displays data points as a Bar Chart
     * */
    public class BarChartVisual :IGraphVisual {

        private RectTransform graphContainer;
        private Color barColor;
        private float barWidthMultiplier;

        public BarChartVisual(RectTransform graphContainer, Color barColor, float barWidthMultiplier) {
            this.graphContainer = graphContainer;
            this.barColor = barColor;
            this.barWidthMultiplier = barWidthMultiplier;
        }

        public void CleanUp() {
        }

        public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText) {
            GameObject barGameObject = CreateBar(graphPosition, graphPositionWidth);

            BarChartVisualObject barChartVisualObject = new BarChartVisualObject(barGameObject, barWidthMultiplier);
            barChartVisualObject.SetGraphVisualObjectInfo(graphPosition, graphPositionWidth, tooltipText);

            return barChartVisualObject;
        }

        private GameObject CreateBar(Vector2 graphPosition, float barWidth) {
            GameObject gameObject = new GameObject("bar", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = barColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
            rectTransform.sizeDelta = new Vector2(barWidth * barWidthMultiplier, graphPosition.y);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(.5f, 0f);

            // Add Button_UI Component which captures UI Mouse Events
            //Button_UI barButtonUI = gameObject.AddComponent<Button_UI>();

            return gameObject;
        }


        public class BarChartVisualObject :IGraphVisualObject {

            private GameObject barGameObject;
            private float barWidthMultiplier;

            public BarChartVisualObject(GameObject barGameObject, float barWidthMultiplier) {
                this.barGameObject = barGameObject;
                this.barWidthMultiplier = barWidthMultiplier;
            }

            public void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText) {
                RectTransform rectTransform = barGameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
                rectTransform.sizeDelta = new Vector2(graphPositionWidth * barWidthMultiplier, graphPosition.y);

                //Button_UI barButtonUI = barGameObject.GetComponent<Button_UI>();

                // Show Tooltip on Mouse Over
                //barButtonUI.MouseOverOnceFunc = () => {
                //    ShowTooltip_Static(tooltipText, graphPosition);
                //};

                // Hide Tooltip on Mouse Out
                //barButtonUI.MouseOutOnceFunc = () => {
                //    HideTooltip_Static();
                //};
            }

            public void CleanUp() {
                Object.Destroy(barGameObject);
            }


        }

    }

}
