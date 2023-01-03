using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BuildingSystem {
    [CreateAssetMenu]
    public class TInstantiableObjectSO :ScriptableObject {
        public string nameString;
        public Constants.InstantiableTypes instantiableType;
        public Transform transform;
        public Transform visual;
        public Sprite sprite;
        public int width;
        public int height;
        public static Constants.Dir GetDir(Vector2Int from, Vector2Int to) {
            if (from.x < to.x) {
                return Constants.Dir.Right;
            }
            else {
                if (from.x > to.x) {
                    return Constants.Dir.Left;
                }
                else {
                    if (from.y < to.y) {
                        return Constants.Dir.Up;
                    }
                    else {
                        return Constants.Dir.Down;
                    }
                }
            }
        }

        public static Constants.Dir GetNextDir(Constants.Dir dir) {
            switch (dir) {
                default:
                case Constants.Dir.Down: return Constants.Dir.Left;
                case Constants.Dir.Left: return Constants.Dir.Up;
                case Constants.Dir.Up: return Constants.Dir.Right;
                case Constants.Dir.Right: return Constants.Dir.Down;
            }
        }

        public static Vector2Int GetDirForwardVector(Constants.Dir dir) {
            switch (dir) {
                default:
                case Constants.Dir.Down: return new Vector2Int(0, -1);
                case Constants.Dir.Left: return new Vector2Int(-1, 0);
                case Constants.Dir.Up: return new Vector2Int(0, +1);
                case Constants.Dir.Right: return new Vector2Int(+1, 0);
            }
        }

        public static int GetRotationAngle(Constants.Dir dir) {
            switch (dir) {
                default:
                case Constants.Dir.Down: return 0;
                case Constants.Dir.Left: return 90;
                case Constants.Dir.Up: return 180;
                case Constants.Dir.Right: return 270;
            }
        }

        public Vector2Int GetRotationOffset(Constants.Dir dir) {
            switch (dir) {
                default:
                case Constants.Dir.Up: return new Vector2Int(width, height);
                case Constants.Dir.Down: return new Vector2Int(0, 0);
                case Constants.Dir.Left: return new Vector2Int(0, width);
                case Constants.Dir.Right: return new Vector2Int(height, 0);
            }
        }

    }
}