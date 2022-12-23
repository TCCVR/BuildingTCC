using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sensor {
    public class WindowGraph :MonoBehaviour {

        private static WindowGraph instance;

        [SerializeField] private Sprite dotSprite;
        private RectTransform graphContainer;
        private RectTransform labelTemplateX;
        private RectTransform labelTemplateY;
        private RectTransform dashContainer;
        private RectTransform dashTemplateX;
        private RectTransform dashTemplateY;
        private List<GameObject> gameObjectList;
        private List<IGraphVisualObject> graphVisualObjectList;
        private List<RectTransform> yLabelList;

        // Cached values
        private List<int> valueList;
        private IGraphVisual graphVisual;
        private int maxVisibleValueAmount;
        private Func<int, string> getAxisLabelX;
        private Func<float, string> getAxisLabelY;
        private float xSize;
        private bool startYScaleAtZero;

        private void Awake() {
            instance = this;
            // Grab base objects references
            graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
            labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
            labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
            dashContainer = graphContainer.Find("dashContainer").GetComponent<RectTransform>();
            dashTemplateX = dashContainer.Find("dashTemplateX").GetComponent<RectTransform>();
            dashTemplateY = dashContainer.Find("dashTemplateY").GetComponent<RectTransform>();

            startYScaleAtZero = true;
            gameObjectList = new List<GameObject>();
            yLabelList = new List<RectTransform>();
            graphVisualObjectList = new List<IGraphVisualObject>();

            IGraphVisual lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, Color.green, new Color(1, 1, 1, .5f));



            // Set up base values
            List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };
            ShowGraph(valueList, lineGraphVisual, -1, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));

        }


        private void ShowGraph(List<int> valueList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null) {
            this.valueList = valueList;
            this.graphVisual = graphVisual;
            this.getAxisLabelX = getAxisLabelX;
            this.getAxisLabelY = getAxisLabelY;

            if (maxVisibleValueAmount <= 0) {
                // Show all if no amount specified
                maxVisibleValueAmount = valueList.Count;
            }
            if (maxVisibleValueAmount > valueList.Count) {
                // Validate the amount to show the maximum
                maxVisibleValueAmount = valueList.Count;
            }

            this.maxVisibleValueAmount = maxVisibleValueAmount;

            // Test for label defaults
            if (getAxisLabelX == null) {
                getAxisLabelX = delegate (int _i) { return _i.ToString(); };
            }
            if (getAxisLabelY == null) {
                getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
            }

            // Clean up previous graph
            foreach (GameObject gameObject in gameObjectList) {
                Destroy(gameObject);
            }
            gameObjectList.Clear();
            yLabelList.Clear();

            foreach (IGraphVisualObject graphVisualObject in graphVisualObjectList) {
                graphVisualObject.CleanUp();
            }
            graphVisualObjectList.Clear();

            graphVisual.CleanUp();

            // Grab the width and height from the container
            float graphWidth = graphContainer.sizeDelta.x;
            float graphHeight = graphContainer.sizeDelta.y;

            float yMinimum, yMaximum;
            CalculateYScale(out yMinimum, out yMaximum);

            // Set the distance between each point on the graph 
            xSize = graphWidth / (maxVisibleValueAmount + 1);

            // Cycle through all visible data points
            int xIndex = 0;
            for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
                float xPosition = xSize + xIndex * xSize;
                float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

                // Add data point visual
                string tooltipText = getAxisLabelY(valueList[i]);
                IGraphVisualObject graphVisualObject = graphVisual.CreateGraphVisualObject(new Vector2(xPosition, yPosition), xSize, tooltipText);
                graphVisualObjectList.Add(graphVisualObject);

                // Duplicate the x label template
                RectTransform labelX = Instantiate(labelTemplateX);
                labelX.SetParent(graphContainer, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(xPosition, -7f);
                labelX.GetComponent<Text>().text = getAxisLabelX(i);
                gameObjectList.Add(labelX.gameObject);

                xIndex++;
            }
        }

        private void CalculateYScale(out float yMinimum, out float yMaximum) {
            // Identify y Min and Max values
            yMaximum = valueList[0];
            yMinimum = valueList[0];

            for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
                int value = valueList[i];
                if (value > yMaximum) {
                    yMaximum = value;
                }
                if (value < yMinimum) {
                    yMinimum = value;
                }
            }

            float yDifference = yMaximum - yMinimum;
            if (yDifference <= 0) {
                yDifference = 5f;
            }
            yMaximum = yMaximum + (yDifference * 0.2f);
            yMinimum = yMinimum - (yDifference * 0.2f);

            if (startYScaleAtZero) {
                yMinimum = 0f; // Start the graph at zero
            }
        }



        /*
         * Interface definition for showing visual for a data point
         * */
        private interface IGraphVisual {

            IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
            void CleanUp();

        }

        /*
         * Represents a single Visual Object in the graph
         * */
        private interface IGraphVisualObject {

            void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
            void CleanUp();

        }

        /*
         * Displays data points as a Line Graph
         * */
        private class LineGraphVisual :IGraphVisual {

            private RectTransform graphContainer;
            private Sprite dotSprite;
            private LineGraphVisualObject lastLineGraphVisualObject;
            private Color dotColor;
            private Color dotConnectionColor;

            public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color dotConnectionColor) {
                this.graphContainer = graphContainer;
                this.dotSprite = dotSprite;
                this.dotColor = dotColor;
                this.dotConnectionColor = dotConnectionColor;
                lastLineGraphVisualObject = null;
            }

            public void CleanUp() {
                lastLineGraphVisualObject = null;
            }


            public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText) {
                GameObject dotGameObject = CreateDot(graphPosition);


                GameObject dotConnectionGameObject = null;
                if (lastLineGraphVisualObject != null) {
                    dotConnectionGameObject = CreateDotConnection(lastLineGraphVisualObject.GetGraphPosition(), dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                }

                LineGraphVisualObject lineGraphVisualObject = new LineGraphVisualObject(dotGameObject, dotConnectionGameObject, lastLineGraphVisualObject);
                lineGraphVisualObject.SetGraphVisualObjectInfo(graphPosition, graphPositionWidth, tooltipText);

                lastLineGraphVisualObject = lineGraphVisualObject;

                return lineGraphVisualObject;
            }

            private GameObject CreateDot(Vector2 anchoredPosition) {
                GameObject gameObject = new GameObject("dot", typeof(Image));
                gameObject.transform.SetParent(graphContainer, false);
                gameObject.GetComponent<Image>().sprite = dotSprite;
                gameObject.GetComponent<Image>().color = dotColor;
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.sizeDelta = new Vector2(11, 11);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                return gameObject;
            }

            private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
                GameObject gameObject = new GameObject("dotConnection", typeof(Image));
                gameObject.transform.SetParent(graphContainer, false);
                gameObject.GetComponent<Image>().color = dotConnectionColor;
                gameObject.GetComponent<Image>().raycastTarget = false;
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                Vector2 dir = (dotPositionB - dotPositionA).normalized;
                float distance = Vector2.Distance(dotPositionA, dotPositionB);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(distance, 3f);
                rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
                rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
                return gameObject;
            }


            public class LineGraphVisualObject :IGraphVisualObject {

                public event EventHandler OnChangedGraphVisualObjectInfo;

                private GameObject dotGameObject;
                private GameObject dotConnectionGameObject;
                private LineGraphVisualObject lastVisualObject;

                public LineGraphVisualObject(GameObject dotGameObject, GameObject dotConnectionGameObject, LineGraphVisualObject lastVisualObject) {
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
                    Destroy(dotGameObject);
                    Destroy(dotConnectionGameObject);
                }

                public Vector2 GetGraphPosition() {
                    RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                    return rectTransform.anchoredPosition;
                }

                private void UpdateDotConnection() {
                    if (dotConnectionGameObject != null) {
                        RectTransform dotConnectionRectTransform = dotConnectionGameObject.GetComponent<RectTransform>();
                        Vector2 dir = (lastVisualObject.GetGraphPosition() - GetGraphPosition()).normalized;
                        float distance = Vector2.Distance(GetGraphPosition(), lastVisualObject.GetGraphPosition());
                        dotConnectionRectTransform.sizeDelta = new Vector2(distance, 3f);
                        dotConnectionRectTransform.anchoredPosition = GetGraphPosition() + dir * distance * .5f;
                        dotConnectionRectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
                    }
                }
            }
        }
    }
}
