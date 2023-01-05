/* 
------------------- Code Monkey -------------------

Thank you for downloading this package
I hope you find it useful in your projects
If you have any questions let me know
Cheers!

           unitycodemonkey.com
--------------------------------------------------
*/

    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

namespace SensorSystem {
    public class Window_Graph :MonoBehaviour {

        private static Window_Graph instance;

        [SerializeField] private Sprite dotSprite;
        private RectTransform graphContainer;
        private RectTransform labelTemplateX;
        private RectTransform labelTemplateY;
        private RectTransform dashContainer;
        private RectTransform dashTemplateX;
        private RectTransform dashTemplateY;
        private List<GameObject> gameObjectList;
        private List<IGraphVisualObject> graphVisualObjectList;
        private GameObject tooltipGameObject;
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
            tooltipGameObject = graphContainer.Find("tooltip").gameObject;

            startYScaleAtZero = true;
            gameObjectList = new List<GameObject>();
            yLabelList = new List<RectTransform>();
            graphVisualObjectList = new List<IGraphVisualObject>();

            IGraphVisual lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, Color.green, new Color(1, 1, 1, .5f));
            IGraphVisual barChartVisual = new BarChartVisual(graphContainer, Color.white, .8f);

            //// Set up buttons
            //transform.Find("barChartBtn").GetComponent<Button_UI>().ClickFunc = () => {
            //    SetGraphVisual(barChartVisual);
            //};
            //transform.Find("lineGraphBtn").GetComponent<Button_UI>().ClickFunc = () => {
            //    SetGraphVisual(lineGraphVisual);
            //};

            //transform.Find("decreaseVisibleAmountBtn").GetComponent<Button_UI>().ClickFunc = () => {
            //    DecreaseVisibleAmount();
            //};
            //transform.Find("increaseVisibleAmountBtn").GetComponent<Button_UI>().ClickFunc = () => {
            //    IncreaseVisibleAmount();
            //};

            //transform.Find("dollarBtn").GetComponent<Button_UI>().ClickFunc = () => {
            //    SetGetAxisLabelY((float _f) => "$" + Mathf.RoundToInt(_f));
            //};
            //transform.Find("euroBtn").GetComponent<Button_UI>().ClickFunc = () => {
            //    SetGetAxisLabelY((float _f) => "€" + Mathf.RoundToInt(_f / 1.18f));
            //};

            HideTooltip();

            // Set up base values
            List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };
            ShowGraph(valueList, barChartVisual, -1, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));

            /*
            // Automatically modify graph values and visual
            bool useBarChart = true;
            FunctionPeriodic.Create(() => {
                for (int i = 0; i < valueList.Count; i++) {
                    valueList[i] = Mathf.RoundToInt(valueList[i] * UnityEngine.Random.Range(0.8f, 1.2f));
                    if (valueList[i] < 0) valueList[i] = 0;
                }
                if (useBarChart) {
                    ShowGraph(valueList, barChartVisual, -1, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));
                } else {
                    ShowGraph(valueList, lineGraphVisual, -1, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));
                }
                useBarChart = !useBarChart;
            }, .5f);
            //*/

            int index = 0;
            //FunctionPeriodic.Create(() => {
            //    index = (index + 1) % valueList.Count;
            //}, .1f);
            //FunctionPeriodic.Create(() => {
            //    //int index = UnityEngine.Random.Range(0, valueList.Count);
            UpdateValue(index, valueList[index] + UnityEngine.Random.Range(1, 3));
            //}, .02f);
        }

        public static void ShowTooltip_Static(string tooltipText, Vector2 anchoredPosition) {
            instance.ShowTooltip(tooltipText, anchoredPosition);
        }

        private void ShowTooltip(string tooltipText, Vector2 anchoredPosition) {
            // Show Tooltip GameObject
            tooltipGameObject.SetActive(true);

            tooltipGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

            Text tooltipUIText = tooltipGameObject.transform.Find("text").GetComponent<Text>();
            tooltipUIText.text = tooltipText;

            float textPaddingSize = 4f;
            Vector2 backgroundSize = new Vector2(
                tooltipUIText.preferredWidth + textPaddingSize * 2f,
                tooltipUIText.preferredHeight + textPaddingSize * 2f
            );

            tooltipGameObject.transform.Find("background").GetComponent<RectTransform>().sizeDelta = backgroundSize;

            // UI Visibility Sorting based on Hierarchy, SetAsLastSibling in order to show up on top
            tooltipGameObject.transform.SetAsLastSibling();
        }

        public static void HideTooltip_Static() {
            instance.HideTooltip();
        }

        private void HideTooltip() {
            tooltipGameObject.SetActive(false);
        }

        private void SetGetAxisLabelX(Func<int, string> getAxisLabelX) {
            ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, getAxisLabelX, this.getAxisLabelY);
        }

        private void SetGetAxisLabelY(Func<float, string> getAxisLabelY) {
            ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, getAxisLabelY);
        }

        private void IncreaseVisibleAmount() {
            ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount + 1, this.getAxisLabelX, this.getAxisLabelY);
        }

        private void DecreaseVisibleAmount() {
            ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount - 1, this.getAxisLabelX, this.getAxisLabelY);
        }

        private void SetGraphVisual(IGraphVisual graphVisual) {
            ShowGraph(this.valueList, graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, this.getAxisLabelY);
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

                // Duplicate the x dash template
                RectTransform dashX = Instantiate(dashTemplateX);
                dashX.SetParent(dashContainer, false);
                dashX.gameObject.SetActive(true);
                dashX.anchoredPosition = new Vector2(xPosition, -3f);
                gameObjectList.Add(dashX.gameObject);

                xIndex++;
            }

            // Set up separators on the y axis
            int separatorCount = 10;
            for (int i = 0; i <= separatorCount; i++) {
                // Duplicate the label template
                RectTransform labelY = Instantiate(labelTemplateY);
                labelY.SetParent(graphContainer, false);
                labelY.gameObject.SetActive(true);
                float normalizedValue = i * 1f / separatorCount;
                labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight);
                labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
                yLabelList.Add(labelY);
                gameObjectList.Add(labelY.gameObject);

                // Duplicate the dash template
                RectTransform dashY = Instantiate(dashTemplateY);
                dashY.SetParent(dashContainer, false);
                dashY.gameObject.SetActive(true);
                dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
                gameObjectList.Add(dashY.gameObject);
            }
        }

        private void UpdateValue(int index, int value) {
            float yMinimumBefore, yMaximumBefore;
            CalculateYScale(out yMinimumBefore, out yMaximumBefore);

            valueList[index] = value;

            float graphWidth = graphContainer.sizeDelta.x;
            float graphHeight = graphContainer.sizeDelta.y;

            float yMinimum, yMaximum;
            CalculateYScale(out yMinimum, out yMaximum);

            bool yScaleChanged = yMinimumBefore != yMinimum || yMaximumBefore != yMaximum;

            if (!yScaleChanged) {
                // Y Scale did not change, update only this value
                float xPosition = xSize + index * xSize;
                float yPosition = ((value - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

                // Add data point visual
                string tooltipText = getAxisLabelY(value);
                graphVisualObjectList[index].SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), xSize, tooltipText);
            }
            else {
                // Y scale changed, update whole graph and y axis labels
                // Cycle through all visible data points
                int xIndex = 0;
                for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
                    float xPosition = xSize + xIndex * xSize;
                    float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

                    // Add data point visual
                    string tooltipText = getAxisLabelY(valueList[i]);
                    graphVisualObjectList[xIndex].SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), xSize, tooltipText);

                    xIndex++;
                }

                for (int i = 0; i < yLabelList.Count; i++) {
                    float normalizedValue = i * 1f / yLabelList.Count;
                    yLabelList[i].GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
                }
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




    }

}