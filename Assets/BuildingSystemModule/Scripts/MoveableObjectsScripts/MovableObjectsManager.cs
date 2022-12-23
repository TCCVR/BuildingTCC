using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuildingSystem {
    public class MovableObjectsManager :TInstantiableObjectsManager, IPCInputSubscriber {
        public static MovableObjectsManager Instance { get; private set; }

        GhostMovableObject ghostMovableObject;

        private float looseObjectEulerY;
        private int angleDiscreetCounter = 0;
        private bool currentManager = false;

        public event EventHandler OnSelectedChanged;

        private void Awake() {
            Instance = this;
            managedType = Constants.InstantiableTypes.MoveableObjects;
            MouseClickAdd = mouseClickAddFunc;
            AddFromInfo = addFromInfoFunc;
            //MovableObjectsTypeSelectUI initUI = MovableObjectsTypeSelectUI.Instance;
        }



        private void Start() {
            TInstantiableObjectsTypeSelectUI initUI = TInstantiableObjectsTypeSelectUI.Instance;
            TInstantiableObjectSystem.Instance.Managers.Add(Constants.InstantiableTypes.MoveableObjects, this);
            TInstantiableObjectSystem.Instance.OnKeyPressed += Subs_OnKeyPressed;
            TInstantiableObjectSystem.Instance.OnMouse0 += Subs_OnMouse0;
            TInstantiableObjectSystem.Instance.OnMouse1 += Subs_OnMouse1;
            TInstantiableObjectSystem.Instance.OnMouseMid += Subs_OnMouseMid;
            TInstantiableObjectSystem.Instance.OnMouseScroll += Subs_OnMouseScroll;
            if (!TInstantiableObjectSystem.Instance.CurrentManager) {
                ActivateManager();
                Debug.Log("current manager name: " + TInstantiableObjectSystem.Instance.CurrentManager.name);
            }
        }


        public override void ActivateManager() {
            TInstantiableObjectSystem.Instance.CurrentManager = Instance;
            if (ghostMovableObject is null) {
                ghostMovableObject = GhostMovableObject.Instance;
            }
            if (currentSO is null) {
                currentSO = Assets.Instance.movableObjectsTypeSOList[listCounter];
            }
            ghostMovableObject.Activation();
            currentManager = true;
            //TInstantiableObjectsTypeSelectUI.Instance.ClearUpdateButtons(Assets.Instance.movableObjectsTypeSOList);
        }
        public override void DeactivateManager() {
            ghostMovableObject.Activation(false);
            currentManager = false;
        }

        private void mouseClickAddFunc() {
            Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
            float maxBuildDistance = 10f;

            if (Mouse3D.GetDistanceToPlayer() >= maxBuildDistance) {
                return;
            }

            Transform newPlacedMoveableObjects = Instantiate(currentSO.transform, mouseWorldPosition, transform.rotation * Quaternion.Euler(0, 45 * angleDiscreetCounter, 0));
            newPlacedMoveableObjects.transform.parent = InstancesList.transform;
            MovableObjectsInfo newMoveableObjectsInfo = newPlacedMoveableObjects.gameObject.GetComponent<MovableObjectsInfo>();
            newMoveableObjectsInfo.LoadInfo(currentSO, newPlacedMoveableObjects);
        }


        private void addFromInfoFunc(InstanceInfo bInfo) {
            List<TInstantiableObjectSO> foundSOTypeFromSerialized;
            TInstantiableObjectSO typeSO;

            foundSOTypeFromSerialized = Assets.Instance.movableObjectsTypeSOList
                .Where(d => d.nameString == bInfo.instanceName).Select(d => (TInstantiableObjectSO)d)
                    .ToList();

            if (foundSOTypeFromSerialized.Count<TInstantiableObjectSO>() == 0) {
                Debug.Log("MovableObjectsSO: " + bInfo.SOType + " was NOT serialized in this buildVer.");
                return;
            }
            typeSO = foundSOTypeFromSerialized[0];
            MovableObjectsInfo newBuildingInfo = MovableObjectsInfo.Create<TInstantiableObjectSO, MovableObjectsInfo>(bInfo, typeSO, InstancesList.transform) as MovableObjectsInfo;
            return;
        }


        public override void Subs_OnKeyPressed(object sender, OnKeyPressedEventArgs e) {
            if (currentManager) {
                if (e.keyPressed == KeyCode.F) {
                    angleDiscreetCounter = 0;
                }
                else if (e.keyPressed == KeyCode.Tab) {
                    NextSO();
                }

            }
        }
        public override void Subs_OnMouse0(object sender, EventArgs e) {
            if (currentManager) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    mouseClickAddFunc();
                }
            }
        }
        public override void Subs_OnMouse1(object sender, EventArgs e) {
            if (currentManager) {
                angleDiscreetCounter += 1;
                if (angleDiscreetCounter == 8) {
                    angleDiscreetCounter = 0;
                }
                print(angleDiscreetCounter);
            }
        }
        public override void Subs_OnMouseMid(object sender, EventArgs e) {
            if (currentManager) {

            }
        }
        public override void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs e) {
            if (currentManager) {

            }
        }

        public float GetMovableObjectEulerY() {
            return looseObjectEulerY;
        }

        public TInstantiableObjectSO GetInstanceableObjectSO() {
            return currentSO;
        }


        public void NextSO() {
            if (currentSO is null) {
                currentSO = Assets.Instance.movableObjectsTypeSOList[listCounter];
            }
            if (listCounter < Assets.Instance.movableObjectsTypeSOList.Count - 1) {
                listCounter++;
            }
            else {
                listCounter = 0;
            }
            currentSO = Assets.Instance.movableObjectsTypeSOList[listCounter];
            OnSelectedChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
