using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BuildingSystem {
    [CreateAssetMenu]
    public class TInstantiableObjectSO :ScriptableObject {
        public string nameString;
        public BuildingSystemConstants.InstantiableTypes instantiableType;
        public Transform transform;
        public Transform visual;
        public Sprite sprite;
        public int width;
        public int height;
        public static BuildingSystemConstants.Dir GetDir(Vector2Int from, Vector2Int to) {
            if (from.x < to.x) {
                return BuildingSystemConstants.Dir.Right;
            }
            else {
                if (from.x > to.x) {
                    return BuildingSystemConstants.Dir.Left;
                }
                else {
                    if (from.y < to.y) {
                        return BuildingSystemConstants.Dir.Up;
                    }
                    else {
                        return BuildingSystemConstants.Dir.Down;
                    }
                }
            }
        }

        public static BuildingSystemConstants.Dir GetNextDir(BuildingSystemConstants.Dir dir) {
            switch (dir) {
                default:
                case BuildingSystemConstants.Dir.Down: return BuildingSystemConstants.Dir.Left;
                case BuildingSystemConstants.Dir.Left: return BuildingSystemConstants.Dir.Up;
                case BuildingSystemConstants.Dir.Up: return BuildingSystemConstants.Dir.Right;
                case BuildingSystemConstants.Dir.Right: return BuildingSystemConstants.Dir.Down;
            }
        }

        public static Vector2Int GetDirForwardVector(BuildingSystemConstants.Dir dir) {
            switch (dir) {
                default:
                case BuildingSystemConstants.Dir.Down: return new Vector2Int(0, -1);
                case BuildingSystemConstants.Dir.Left: return new Vector2Int(-1, 0);
                case BuildingSystemConstants.Dir.Up: return new Vector2Int(0, +1);
                case BuildingSystemConstants.Dir.Right: return new Vector2Int(+1, 0);
            }
        }

        public static int GetRotationAngle(BuildingSystemConstants.Dir dir) {
            switch (dir) {
                default:
                case BuildingSystemConstants.Dir.Down: return 0;
                case BuildingSystemConstants.Dir.Left: return 90;
                case BuildingSystemConstants.Dir.Up: return 180;
                case BuildingSystemConstants.Dir.Right: return 270;
            }
        }

        public Vector2Int GetRotationOffset(BuildingSystemConstants.Dir dir) {
            switch (dir) {
                default:
                case BuildingSystemConstants.Dir.Up: return new Vector2Int(width, height);
                case BuildingSystemConstants.Dir.Down: return new Vector2Int(0, 0);
                case BuildingSystemConstants.Dir.Left: return new Vector2Int(0, width);
                case BuildingSystemConstants.Dir.Right: return new Vector2Int(height, 0);
            }
        }

    }
}