
namespace BuildingSystem {
    public class GridObject {

        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        public GridObjectsInfo gridObjectsInfo;

        public GridObject(GridXZ<GridObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            gridObjectsInfo = null;
        }

        public override string ToString() {
            return x + ", " + y + "\n" + gridObjectsInfo;
        }

        public void TriggerGridObjectChanged() {
            grid.TriggerGridObjectChanged(x, y);
        }

        public void SetPlacedObject(GridObjectsInfo placedObject) {
            this.gridObjectsInfo = placedObject;
            TriggerGridObjectChanged();
        }

        public void ClearPlacedObject() {
            gridObjectsInfo = null;
            TriggerGridObjectChanged();
        }

        public GridObjectsInfo GetPlacedObject() {
            return gridObjectsInfo;
        }

        public bool CanBuild() {
            return gridObjectsInfo == null;
        }

    }
}