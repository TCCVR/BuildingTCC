
using UnityEngine;

public class GhostGridEdgeObject: TGhostObject {

    public static GhostGridEdgeObject Instance { get; private set; }

    private void Awake() {
        Instance = this;
        Instance.Activation(false);
    }
    private void Start() {
        GridObjectsManager.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    protected override void RefreshVisual() {
        if (active) {
            if (visual != null) {
                Destroy(visual.gameObject);
                visual = null;
            }
            TInstantiableObjectSO instanceableObjectSO = GridObjectsManager.Instance.GetInstanceableObjectSO();
            if (instanceableObjectSO != null) {
                visual = Instantiate(instanceableObjectSO.visual, Vector3.zero, Quaternion.identity);
                visual.parent = transform;
                visual.localPosition = Vector3.zero;
                visual.localEulerAngles = Vector3.zero;
                SetLayerRecursive(visual.gameObject, 14);
            }
        }
        else {
            if (visual != null) {
                Destroy(visual.gameObject);
                visual = null;
            }
        }
    }

    protected override void GhostLateUpdate() {
        Vector3 targetPosition = GridObjectsManager.Instance.GetMouseWorldSnappedPosition();
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
        transform.rotation = Quaternion.Lerp(transform.rotation, GridObjectsManager.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
    }

}