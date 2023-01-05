using System;
using UnityEngine;

namespace SensorSystem {
    /*
     * Interface definition for showing visual for a data point
     * */
    public interface IGraphVisual {
        IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
        void CleanUp();
    }
}
