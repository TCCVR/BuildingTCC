using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem {
    public class GridXZ<TGridObject> {

        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
        public class OnGridObjectChangedEventArgs :EventArgs {
            public int x;
            public int z;
        }

        private int width;
        private int depth;
        private float cellSize;
        private Vector3 originPosition;
        private TGridObject[,] gridArray;

        public GridXZ(int width, int depth, float cellSize, Vector3 originPosition, Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject) {
            this.width = width;
            this.depth = depth;
            this.cellSize = cellSize;
            this.originPosition = originPosition;

            gridArray = new TGridObject[width, depth];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int z = 0; z < gridArray.GetLength(1); z++) {
                    gridArray[x, z] = createGridObject(this, x, z);
                }
            }

            bool showDebug = false;
            if (showDebug) {
                TextMesh[,] debugTextArray = new TextMesh[width, depth];

                for (int x = 0; x < gridArray.GetLength(0); x++) {
                    for (int z = 0; z < gridArray.GetLength(1); z++) {
                        //debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * .5f, 15, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                        Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                    }
                }
                Debug.DrawLine(GetWorldPosition(0, depth), GetWorldPosition(width, depth), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, depth), Color.white, 100f);

                OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                    debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
                };
            }
        }

        public int GetWidth() {
            return width;
        }

        public int GetHeight() {
            return depth;
        }

        public float GetCellSize() {
            return cellSize;
        }

        public Vector3 GetWorldPosition(int x, int z) {
            return new Vector3(x, 0, z) * cellSize + originPosition;
        }

        public void GetXZ(Vector3 worldPosition, out int x, out int z) {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        }

        public void SetGridObject(int x, int z, TGridObject value) {
            if (x >= 0 && z >= 0 && x < width && z < depth) {
                gridArray[x, z] = value;
                TriggerGridObjectChanged(x, z);
            }
        }

        public void TriggerGridObjectChanged(int x, int z) {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value) {
            GetXZ(worldPosition, out int x, out int z);
            SetGridObject(x, z, value);
        }

        public TGridObject GetGridObject(int x, int z) {
            if (x >= 0 && z >= 0 && x < width && z < depth) {
                return gridArray[x, z];
            }
            else {
                return default(TGridObject);
            }
        }

        public TGridObject GetGridObject(Vector3 worldPosition) {
            int x, z;
            GetXZ(worldPosition, out x, out z);
            return GetGridObject(x, z);
        }

        public Vector2Int ValidateGridPosition(Vector2Int gridPosition) {
            return new Vector2Int(
                Mathf.Clamp(gridPosition.x, 0, width - 1),
                Mathf.Clamp(gridPosition.y, 0, depth - 1)
            );
        }

        public bool IsValidGridPosition(Vector2Int gridPosition) {
            int x = gridPosition.x;
            int z = gridPosition.y;

            if (x >= 0 && z >= 0 && x < width && z < depth) {
                return true;
            }
            else {
                return false;
            }
        }

        public bool IsValidGridPositionWithPadding(Vector2Int gridPosition) {
            Vector2Int padding = new Vector2Int(2, 2);
            int x = gridPosition.x;
            int z = gridPosition.y;

            if (x >= padding.x && z >= padding.y && x < width - padding.x && z < depth - padding.y) {
                return true;
            }
            else {
                return false;
            }
        }
        public static List<Vector2Int> GetGridPositionList(int width, int height, Vector2Int offset, BuildingSystemConstants.Dir dir) {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            switch (dir) {
                default:
                case BuildingSystemConstants.Dir.Down:
                case BuildingSystemConstants.Dir.Up:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
                case BuildingSystemConstants.Dir.Left:
                case BuildingSystemConstants.Dir.Right:
                    for (int x = 0; x < height; x++) {
                        for (int y = 0; y < width; y++) {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
            }
            return gridPositionList;
        }
    }
}

