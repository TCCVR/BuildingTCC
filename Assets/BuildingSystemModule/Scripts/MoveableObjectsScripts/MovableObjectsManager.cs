using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuildingSystem {
    public class MovableObjectsManager :TInstantiableObjectsManager, IPCInputSubscriber, ISwitchBuildingSubscriber {
        public static MovableObjectsManager Instance { get; private set; }
        public override bool IsBuildingMode {
            get { return BuildingSystem.Instance.IsBuildingMode; }
        }

        GhostMovableObject ghostMovableObject;

        private float looseObjectEulerY;
        private int angleDiscreetCounter = 0;
        private bool currentManager = false;

        private void Awake() {
            Instance = this;
            ManagedType = BuildingSystemConstants.InstantiableTypes.MoveableObjects;
            MouseClickAdd = mouseClickAddFunc;
            AddFromInfo = addFromInfoFunc;
        }

        public event EventHandler OnSelectedChanged;

        private void Start() {
            TInstantiableObjectsTypeSelectUI initUI = TInstantiableObjectsTypeSelectUI.Instance;
            TInstantiableObjectSystem.Instance.Managers.Add(BuildingSystemConstants.InstantiableTypes.MoveableObjects, this);
            BuildingSystem.Instance.OnKeyPressed += Subs_OnKeyPressed;
            BuildingSystem.Instance.OnMouse0 += Subs_OnMouse0;
            BuildingSystem.Instance.OnMouse1 += Subs_OnMouse1;
            BuildingSystem.Instance.OnMouseMid += Subs_OnMouseMid;
            BuildingSystem.Instance.OnMouseScroll += Subs_OnMouseScroll;
            BuildingSystem.Instance.OnEnableSwitch += Subs_OnBuildingModeEnable;
            BuildingSystem.Instance.OnDisableSwitch += Subs_OnBuildingModeDisable;
            if (!TInstantiableObjectSystem.Instance.CurrentManager) {
                ActivateManager();
                //Debug.Log("current manager name: " + TInstantiableObjectSystem.Instance.CurrentManager.name);
            }
        }


        public override void ActivateManager() {
            if (IsBuildingMode) {
                TInstantiableObjectSystem.Instance.CurrentManager = Instance;
                if (ghostMovableObject is null) {
                    ghostMovableObject = GhostMovableObject.Instance;
                }
                if (currentSO is null) {
                    currentSO = Assets.Instance.movableObjectsTypeSOList[listCounter];
                }
                ghostMovableObject.Activation();
                currentManager = true;
                TInstantiableObjectsTypeSelectUI.Instance.ClearUpdateButtons(Assets.Instance.movableObjectsTypeSOList);
            }
        }

        public override void DeactivateManager() {
            if (IsBuildingMode) {
                ghostMovableObject.Activation(false);
                currentManager = false;
            }
        }

        private void mouseClickAddFunc() {
            Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
            float distanceToPlayer = Mouse3D.GetDistanceToPlayer();
            if ((distanceToPlayer >= BuildingSystemConstants.MAXBUILDINGDISTANCE) || (distanceToPlayer == -1) ) return;

            Transform newPlacedMoveableObjects = Instantiate(currentSO.transform, mouseWorldPosition, transform.rotation * Quaternion.Euler(0, 45 * angleDiscreetCounter, 0));
            newPlacedMoveableObjects.transform.parent = InstancesList.transform;
            MovableObjectsInfo newMoveableObjectsInfo = newPlacedMoveableObjects.gameObject.GetComponent<MovableObjectsInfo>();
            newMoveableObjectsInfo.LoadInfo(currentSO, newPlacedMoveableObjects);
        }


        private void addFromInfoFunc(InstanceInfo bInfo) {
            TInstantiableObjectSO foundSOTypeFromSerialized = Assets.Instance.movableObjectsTypeSOList
                .FirstOrDefault(d => d.nameString == bInfo.SOName);

            if (foundSOTypeFromSerialized is null) {
                Debug.Log("MovableObjectsSO: " + bInfo.SOType + " was NOT serialized in this buildVer.");
                return;
            }
            TInstantiableObjectSO typeSO = foundSOTypeFromSerialized;
            Vector3 worldPosition = bInfo.position.ToVector3();
            MovableObjectsInfo newBuildingInfo = MovableObjectsInfo.Create<TInstantiableObjectSO, MovableObjectsInfo>(bInfo, 
                typeSO, InstancesList.transform) as MovableObjectsInfo;
            return;
        }


        public override void Subs_OnBuildingModeEnable(object sender, EventArgs eventArgs) {
            if (IsBuildingMode) return;
        }

        public override void Subs_OnBuildingModeDisable(object sender, EventArgs eventArgs) {
            if (!IsBuildingMode) return;
        }

        public override void Subs_OnKeyPressed(object sender, OnKeyPressedEventArgs keyPressedArgs) {
            if (IsBuildingMode && currentManager) {
                if (keyPressedArgs.keyPressed == KeyCode.F) {
                    angleDiscreetCounter = 0;
                }
                else if (keyPressedArgs.keyPressed == KeyCode.Tab) {
                    NextSO();
                }

            }
        }
        public override void Subs_OnMouse0(object sender, EventArgs e) {
            if (IsBuildingMode && currentManager) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    mouseClickAddFunc();
                }
            }
        }
        public override void Subs_OnMouse1(object sender, EventArgs e) {
            if (IsBuildingMode && currentManager) {
                angleDiscreetCounter += 1;
                if (angleDiscreetCounter == 8) {
                    angleDiscreetCounter = 0;
                }
                print(angleDiscreetCounter);
            }
        }
        public override void Subs_OnMouseMid(object sender, EventArgs e) {
            if (IsBuildingMode && currentManager) {

            }
        }
        public override void Subs_OnMouseScroll(object sender, OnMouseScrollEventArgs e) {
            if (IsBuildingMode && currentManager) {

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
