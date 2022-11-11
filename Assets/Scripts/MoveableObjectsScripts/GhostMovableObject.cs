using UnityEngine;


public class GhostMovableObject :TGhostObject {
    public static GhostMovableObject Instance { get; private set; }

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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit)) {
            transform.position = Vector3.Lerp(transform.position, raycastHit.point, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, MovableObjectsManager.Instance.GetMovableObjectEulerY(), 0), Time.deltaTime * 25f);
        }
    }
}