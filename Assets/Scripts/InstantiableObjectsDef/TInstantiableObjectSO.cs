using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TInstantiableObjectSO : ScriptableObject {
    public string nameString;
    public TInstantiableObjectSystem.InstantiableTypes instantiableType;
    public Transform transform;
    public Transform visual;
    public Sprite sprite;
    public int width;
    public int height;
    public enum Dir {
        NotFixed,
        Down,
        Left,
        Up,
        Right,
    }

    public static Dir GetDir(Vector2Int from, Vector2Int to) {
        if (from.x < to.x) {
            return Dir.Right;
        }
        else {
            if (from.x > to.x) {
                return Dir.Left;
            }
            else {
                if (from.y < to.y) {
                    return Dir.Up;
                }
                else {
                    return Dir.Down;
                }
            }
        }
    }

    public static Dir GetNextDir(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public static Vector2Int GetDirForwardVector(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return new Vector2Int(0, -1);
            case Dir.Left: return new Vector2Int(-1, 0);
            case Dir.Up: return new Vector2Int(0, +1);
            case Dir.Right: return new Vector2Int(+1, 0);
        }
    }

    public static int GetRotationAngle(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, width);
            case Dir.Up: return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

}