using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TInstanceableObjectMono :MonoBehaviour {

    public static TInstanceableObjectMono Create(Vector3 worldPosition, Vector2Int origin, TInstanceableObjectSO.Dir dir, TInstanceableObjectSO instanceableObjectSO) {
        Transform placedObjectTransform = Instantiate(instanceableObjectSO.transform, worldPosition, Quaternion.Euler(0, instanceableObjectSO.GetRotationAngle(dir), 0));

        TInstanceableObjectMono placedObject = placedObjectTransform.GetComponent<TInstanceableObjectMono>();
        placedObject.Setup(instanceableObjectSO, origin, dir);

        return placedObject;
    }




    private TInstanceableObjectSO instanceableObjectSO;
    private Vector2Int origin;
    private TInstanceableObjectSO.Dir dir;

    private void Setup(TInstanceableObjectSO iInstanceableObjectSO, Vector2Int origin, TInstanceableObjectSO.Dir dir) {
        this.instanceableObjectSO = iInstanceableObjectSO;
        this.origin = origin;
        this.dir = dir;
    }

    public List<Vector2Int> GetGridPositionList() {
        return instanceableObjectSO.GetGridPositionList(origin, dir);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public override string ToString() {
        return instanceableObjectSO.nameString;
    }

}