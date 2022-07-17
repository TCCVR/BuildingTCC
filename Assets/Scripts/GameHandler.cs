
using System.Collections;
using UnityEngine;
public class GameHandler :MonoBehaviour {

    [SerializeField] private GameObject PlayerCreatedScenario;
    private void Awake() {
        SaveSystem.Init();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            Load();
        }
    }

    private void Save() {
    }

    private void Load() {
    }


    private class SaveObject {
    }
}