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
            get => (x >= 0 && z >= 0 && x< BuildingSystemConstants.GRIDWIDTH 
                        && z< BuildingSystemConstants.GRIDDEPTH) ? gridArray[x, z] : default;
            set {
                if (x >= 0 && z >= 0 && x < BuildingSystemConstants.GRIDWIDTH 
                    && z < BuildingSystemConstants.GRIDDEPTH) gridArray[x, z] = value; 
            }
        }

        private static Vector3 groundCastOriginPosition;
        private Vector3 originPosition;
        private GridObjectsInfo[,] gridArray;

        public GridLevel(int level, Vector3 groundedOriginPosition = default) {
            groundCastOriginPosition = (groundedOriginPosition == default)? 
                        new Vector3(0, BuildingSystemConstants.GROUNDLEVELOFFSET, 0) : groundedOriginPosition;
            originPosition = groundCastOriginPosition + new Vector3(0, level * BuildingSystemConstants.UNITSIZE, 0);   
            gridArray = new GridObjectsInfo[BuildingSystemConstants.GRIDWIDTH, BuildingSystemConstants.GRIDDEPTH];
            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int z = 0; z < gridArray.GetLength(1); z++) {
                    gridArray[x, z] = default;
                }
            }
        }

        public Vector3 GetWorldPosition(Vector2Int coordinates) {
            return new Vector3(coordinates.x, 0, coordinates.y) * BuildingSystemConstants.UNITSIZE + originPosition;
        }

        public static Vector2Int PlaneCoordinatesOf(Vector3 worldPosition) {
            int x = Mathf.FloorToInt((worldPosition - groundCastOriginPosition).x / BuildingSystemConstants.UNITSIZE);
            int z = Mathf.FloorToInt((worldPosition - groundCastOriginPosition).z / BuildingSystemConstants.UNITSIZE);
            x = Mathf.Clamp(x, 0, BuildingSystemConstants.GRIDWIDTH - 1);
            z = Mathf.Clamp(z, 0, BuildingSystemConstants.GRIDDEPTH - 1);
            return new Vector2Int(x, z);
        }



        public static List<Vector2Int> CoordinatesListOf(int width, int depth, Vector2Int offset, BuildingSystemConstants.Dir dir) {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            switch (dir) {
                default:
                case BuildingSystemConstants.Dir.Left:
                case BuildingSystemConstants.Dir.Up:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < depth; y++) {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
                case BuildingSystemConstants.Dir.Down:
                case BuildingSystemConstants.Dir.Right:
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

