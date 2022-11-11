using UnityEngine;
using System.Collections.Generic;

public class Constants: MonoBehaviour{

    public static Constants Instance { get; private set; }
    public const float CELLSIZE = 1f;
    public const float GRIDVERTICALSIZE = 2.5f;
    public const int GRIDWIDTH = 1000;
    public const int GRIDHEIGHT = 1000;
    public const int GRIDVERTICALCOUNT = 4;
    public const float MAXBUILDINGDISTANCE = 10f;

    public List<KeyCode> USEDKEYS = new List<KeyCode>();

    private void Awake() {
        Instance = this;
        //USEDKEYS.Add(KeyCode.W);
        //USEDKEYS.Add(KeyCode.A);
        //USEDKEYS.Add(KeyCode.S);
        //USEDKEYS.Add(KeyCode.D);
        //USEDKEYS.Add(KeyCode.Space);
    }
}