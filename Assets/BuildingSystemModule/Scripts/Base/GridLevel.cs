using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem {
    public class GridLevel {

        public class OnGridObjectChangedEventArgs :EventArgs {
            public int x;
            public int z;
        }
        public GridObjectsInfo this[int x, int z] {
            get => (x >= 0 && z >= 0 && x< Constants.GRIDWIDTH 
                        && z< Constants.GRIDDEPTH) ? gridArray[x, z] : default;
            set {
                if (x >= 0 && z >= 0 && x < Constants.GRIDWIDTH 
                    && z < Constants.GRIDDEPTH) gridArray[x, z] = value; 
            }
        }

        private static Vector3 groundCastOriginPosition;
        private Vector3 originPosition;
        private GridObjectsInfo[,] gridArray;

        public GridLevel(int level, Vector3 groundedOriginPosition = default) {
            groundCastOriginPosition = (groundedOriginPosition == default)? 
                        new Vector3(0, Constants.GROUNDLEVELOFFSET, 0) : groundedOriginPosition;
            originPosition = groundCastOriginPosition + new Vector3(0, level * Constants.UNITSIZE, 0);   
            gridArray = new GridObjectsInfo[Constants.GRIDWIDTH, Constants.GRIDDEPTH];
            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int z = 0; z < gridArray.GetLength(1); z++) {
                    gridArray[x, z] = default;
                }
            }
        }

        public Vector3 GetWorldPosition(Vector2Int coordinates) {
            return new Vector3(coordinates.x, 0, coordinates.y) * Constants.UNITSIZE + originPosition;
        }

        public static Vector2Int PlaneCoordinatesOf(Vector3 worldPosition) {
            int x = Mathf.RoundToInt((worldPosition - groundCastOriginPosition).x / Constants.UNITSIZE);
            int z = Mathf.RoundToInt((worldPosition - groundCastOriginPosition).z / Constants.UNITSIZE);
            x = Mathf.Clamp(x, 0, Constants.GRIDWIDTH - 1);
            z = Mathf.Clamp(z, 0, Constants.GRIDDEPTH - 1);
            return new Vector2Int(x, z);
        }



        public static List<Vector2Int> CoordinatesListOf(int width, int depth, Vector2Int offset, Constants.Dir dir) {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            switch (dir) {
                default:
                case Constants.Dir.Left:
                case Constants.Dir.Up:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < depth; y++) {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
                case Constants.Dir.Down:
                case Constants.Dir.Right:
                    for (int x = 0; x < depth; x++) {
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

