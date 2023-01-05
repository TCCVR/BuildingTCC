using System;
using UnityEngine;

namespace SensorSystem {
    /*
     * Represents a single Visual Object in the graph
     * */
    public interface IGraphVisualObject {
        void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
        void CleanUp();
    }
}
