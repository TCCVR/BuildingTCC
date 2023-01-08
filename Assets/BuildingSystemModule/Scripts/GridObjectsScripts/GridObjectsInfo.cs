using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem {
    public class GridObjectsInfo :TInstantiableObjectInfo {


        [SerializeField] private GridEdgeObjectsPosition upGridEdgePosition;
        [SerializeField] private GridEdgeObjectsPosition downGridEdgePosition;
        [SerializeField] private GridEdgeObjectsPosition leftGridEdgePosition;
        [SerializeField] private GridEdgeObjectsPosition rightGridEdgePosition;

        private GridObjectsInfo upEdgeObject;
        private GridObjectsInfo downEdgeObject;
        private GridObjectsInfo leftEdgeObject;
        private GridObjectsInfo rightEdgeObject;


        public static GridObjectsInfo Create(Vector3 worldPosition, Constants.Dir dir, TInstantiableObjectSO gridObjectSO, GameObject parent) {
            GridObjectsInfo placedObject;
            Transform placedObjectTransform = Instantiate(gridObjectSO.transform, worldPosition, Quaternion.Euler(0, TInstantiableObjectSO.GetRotationAngle(dir), 0));
            placedObjectTransform.parent = parent.transform;
            placedObject = placedObjectTransform.GetComponent<GridObjectsInfo>();
            placedObject.LoadInfo(gridObjectSO, placedObjectTransform);
            placedObject.instanceInfo.dir = dir;
            return placedObject;
        }


        public static GridObjectsInfo Create(Vector3 worldPosition, float absAngle, TInstantiableObjectSO gridObjectSO, GameObject parent) {
            GridObjectsInfo placedObject;
            Transform placedObjectTransform = Instantiate(gridObjectSO.transform, worldPosition, Quaternion.Euler(0, absAngle, 0));
            placedObjectTransform.parent = parent.transform;
            placedObject = placedObjectTransform.GetComponent<GridObjectsInfo>();
            placedObject.LoadInfo(gridObjectSO, placedObjectTransform);
            return placedObject;
        }


        public override void LoadInfo<T>(T btSO, Transform instancedObjTransform) {
            if (this.instanceInfo is null)
                this.instanceInfo = new InstanceInfo();

            TInstantiableObjectSO btSO2 = btSO as TInstantiableObjectSO;
            this.instanceInfo.SOType = btSO2.instantiableType;
            this.instanceInfo.SOName = btSO2.nameString;
            this.instanceInfo.instanceName = instancedObjTransform.name;
            this.instanceInfo.width = btSO2.width;
            this.instanceInfo.depth = btSO2.depth;
            this.instanceInfo.height = btSO2.height;

            this.instanceInfo.position = new MyVector3 { };
            this.instanceInfo.position.x = instancedObjTransform.position.x;
            this.instanceInfo.position.y = instancedObjTransform.position.y;
            this.instanceInfo.position.z = instancedObjTransform.position.z;

            this.instanceInfo.rotation = new MyVector3 { };
            this.instanceInfo.rotation.x = instancedObjTransform.rotation.x;
            this.instanceInfo.rotation.y = instancedObjTransform.rotation.y;
            this.instanceInfo.rotation.z = instancedObjTransform.rotation.z;
            this.instanceInfo.rotation.w = instancedObjTransform.rotation.w;

            this.instanceInfo.scale = new MyVector3 { x = 1, y = 1, z = 1 };
            this.instanceInfo.scale.x = instancedObjTransform.lossyScale.x;
            this.instanceInfo.scale.y = instancedObjTransform.lossyScale.y;
            this.instanceInfo.scale.z = instancedObjTransform.lossyScale.z;
            return;
        }


        public List<Vector2Int> GetGridPositionList() {
            return GridLevel.CoordinatesListOf(instanceInfo.width, instanceInfo.depth, new Vector2Int(Mathf.FloorToInt(instanceInfo.position.x / 2), Mathf.FloorToInt(instanceInfo.position.z / 2)), instanceInfo.dir);
        }


        public void PlaceEdge(Constants.Dir edge, TInstantiableObjectSO gridObjectSO, GameObject parent) {
            GridObjectsInfo currentFloorEdgePlacedObject = GetFloorEdgePlacedObject(edge);
            if (currentFloorEdgePlacedObject != null) {
                Destroy(currentFloorEdgePlacedObject.gameObject);
            }
            GridEdgeObjectsPosition gridEdgePosition = GetGridEdgePosition(edge);
            GridObjectsInfo floorEdgePlacedObject = Create(gridEdgePosition.transform.position, 
                gridEdgePosition.transform.rotation.eulerAngles.y, gridObjectSO, parent);
            floorEdgePlacedObject.instanceInfo.dir = edge;
            SetGridEdgePlacedObject(edge, floorEdgePlacedObject);
        }

        private GridEdgeObjectsPosition GetGridEdgePosition(Constants.Dir edge) {
            switch (edge) {
                default:
                case Constants.Dir.Up: return upGridEdgePosition;
                case Constants.Dir.Down: return downGridEdgePosition;
                case Constants.Dir.Left: return leftGridEdgePosition;
                case Constants.Dir.Right: return rightGridEdgePosition;
            }
        }

        private void SetGridEdgePlacedObject(Constants.Dir edge, GridObjectsInfo floorEdgePlacedObject) {
            switch (edge) {
                default:
                case Constants.Dir.Up:
                    upEdgeObject = floorEdgePlacedObject;
                    break;
                case Constants.Dir.Down:
                    downEdgeObject = floorEdgePlacedObject;
                    break;
                case Constants.Dir.Left:
                    leftEdgeObject = floorEdgePlacedObject;
                    break;
                case Constants.Dir.Right:
                    rightEdgeObject = floorEdgePlacedObject;
                    break;
            }
        }

        private GridObjectsInfo GetFloorEdgePlacedObject(Constants.Dir edge) {
            switch (edge) {
                default:
                case Constants.Dir.Up:
                    return upEdgeObject;
                case Constants.Dir.Down:
                    return downEdgeObject;
                case Constants.Dir.Left:
                    return leftEdgeObject;
                case Constants.Dir.Right:
                    return rightEdgeObject;
            }
        }

    }
}

