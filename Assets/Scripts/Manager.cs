using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Manager : NetworkManager {
    public static bool  MatchStarted;
    public static float TimeToStart    = 30f;
    public static float TimeSinceStart = -1f;

    // Use this for initialization
    void Start() {
        MatchStarted = false;
    }

    // Update is called once per frame
    void Update() {
    }

    public override void OnClientConnect(NetworkConnection conn) {
        if (MatchStarted) {
            conn.Disconnect();
            return;
        }

        base.OnClientConnect(conn);
        
    }
}