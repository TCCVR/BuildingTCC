using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SensorSystem {
    public class HeartbeatGraph :MonoBehaviour, ISensorSubscriber {

        public static HeartbeatGraph Instance { get; private set; }

        [SerializeField] private int MaxVisibleValueAmount = 50;
        [SerializeField] private bool StartYScaleAtZero = false;
        [SerializeField] private Sprite dotSprite;
        private RectTransform graphContainer;
        private RectTransform labelTemplateY;
        private RectTransform dashContainer;
        private RectTransform dashTemplateY;
        private List<GameObject> gameObjectList;
        private List<HeartbeatGraphDot> graphVisualObjectList;
        private List<RectTransform> yLabelList;

        // Cached values
        private IntGraphValues valueList;
        private HeartbeatLine graphVisual;
        private Func<float, string> getAxisLabelY;
        private float xSize;

        private IHandleSensorData<HeartData> keepSensorData;

        private void Awake() {
            Instance = this; 
            valueList = new IntGraphValues(MaxVisibleValueAmount);
            graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
            labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
            dashContainer = graphContainer.Find("dashContainer").GetComponent<RectTransform>();
            dashTemplateY = dashContainer.Find("dashTemplateY").GetComponent<RectTransform>();

            gameObjectList = new List<GameObject>();
            yLabelList = new List<RectTransform>();
            graphVisualObjectList = new List<HeartbeatGraphDot>();

            HeartbeatLine lineGraphVisual = new HeartbeatLine(graphContainer, dotSprite, Color.grey, new Color(1, 1, 1, .5f));
            ShowGraph(valueList, lineGraphVisual, (int _i) => "Day " + (_i + 1), (float _f) => "" + Mathf.RoundToInt(_f));

        }

        private void Start() {
            keepSensorData = new CSVHeartDataHandler();
            SensorManager.SensorServices.Add(this);
            SubscribeTo(CJMCUSerialConnection.Instance);
        }

        private void OnDestroy() {
            (keepSensorData as CSVHeartDataHandler).CloseFile();
            UnsubscribeTo(CJMCUSerialConnection.Instance);
        }

        private void ShowGraph(IntGraphValues valueList, HeartbeatLine graphVisual, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null) {
            this.valueList = valueList;
            this.graphVisual = graphVisual;
            this.getAxisLabelY = getAxisLabelY;

            if (getAxisLabelX == null) {
                getAxisLabelX = delegate (int _i) { return _i.ToString(); };
            }
            if (getAxisLabelY == null) {
                getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
            }

            foreach (GameObject gameObject in gameObjectList) {
                Destroy(gameObject);
            }
            gameObjectList.Clear();
            yLabelList.Clear();

            foreach (HeartbeatGraphDot graphVisualObject in graphVisualObjectList) {
                graphVisualObject.CleanUp();
            }
            graphVisualObjectList.Clear();

            graphVisual.CleanUp();
            float graphWidth = graphContainer.sizeDelta.x;
            float graphHeight = graphContainer.sizeDelta.y;

            float yMinimum, yMaximum;
            CalculateYScale(out yMinimum, out yMaximum);
            xSize = graphWidth / (MaxVisibleValueAmount + 1);
            int xIndex = 0;
            for (int i = 0; i < valueList.MaxSize; i++) {
                float xPosition = xSize + xIndex * xSize;
                float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
                string tooltipText = getAxisLabelY(valueList[i]);
                HeartbeatGraphDot graphVisualObject = graphVisual.CreateGraphVisualObject(new Vector2(xPosition, yPosition), xSize, tooltipText);
                graphVisualObjectList.Add(graphVisualObject);

                xIndex++;
            }
            int separatorCount = 5;
            for (int i = 0; i <= separatorCount; i++) {
                RectTransform labelY = Instantiate(labelTemplateY);
                labelY.SetParent(graphContainer, false);
                labelY.gameObject.SetActive(true);
                float normalizedValue = i * 1f / separatorCount;
                labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight);
                labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
                yLabelList.Add(labelY);
                gameObjectList.Add(labelY.gameObject);
                RectTransform dashY = Instantiate(dashTemplateY);
                dashY.SetParent(dashContainer, false);
                dashY.gameObject.SetActive(true);
                dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
                gameObjectList.Add(dashY.gameObject);
            }
        }

        private void UpdateValue(int value) {
            float graphWidth = graphContainer.sizeDelta.x;
            float graphHeight = graphContainer.sizeDelta.y;
            valueList.InsertValue(value);
            float yMinimum, yMaximum;
            CalculateYScale(out yMinimum, out yMaximum);
            int xIndex = 0;
            for (int i = 0; i < valueList.Count; i++) {
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

        private void CalculateYScale(out float yMinimum, out float yMaximum) {
            // Identify y Min and Max values
            yMaximum = valueList.MaxValue;
            yMinimum = valueList.MinValue;

            float yDifference = yMaximum - yMinimum;
            if (yDifference <= 0) {
                yDifference = 5f;
            }
            yMaximum = yMaximum + (yDifference * 0.2f);
            yMinimum = yMinimum - (yDifference * 0.2f);

            if (StartYScaleAtZero) {
                yMinimum = 0f; // Start the graph at zero
            }
        }

        public void ProcessData(string dataSet) {
            HeartData heartData = new HeartData() {
                HeartBPM = int.Parse(dataSet),
                Timestamp = DateTime.Now
            };
            UpdateValue(heartData.HeartBPM);
            (keepSensorData as CSVHeartDataHandler).InsertDataIntoCache(heartData);
        }

        public void SubscribeTo(ISensorHandler handler) {
            handler.OnSensorParsedData += Subs_OnOnSensorParsedData;
        }

        public void UnsubscribeTo(ISensorHandler handler) {
            handler.OnSensorParsedData -= Subs_OnOnSensorParsedData;
        }

        public void Subs_OnSensorConnect(object sender, EventArgs eventArgs) {
            (keepSensorData as CSVHeartDataHandler).CloseFile();
        }

        public void Subs_OnSensorDisconnect(object sender, EventArgs eventArgs) {
            (keepSensorData as CSVHeartDataHandler).CloseFile();
        }

        public void Subs_OnOnSensorParsedData(object sender, EventArgs eventArgs) {
            string data = CJMCUSerialConnection.Instance.RawData;
            ProcessData(data);
        }

        public void Subs_OnSensorSentData(object sender, EventArgs eventArgs) {
            //Debug.Log($"{(eventArgs as SimpleSerialEventArg).data}");
        }

        public void Subs_OnSensorSentLineData(object sender, EventArgs eventArgs) {
            //Debug.Log($"{(eventArgs as SimpleSerialEventArg).data}");
        }

    }
}
