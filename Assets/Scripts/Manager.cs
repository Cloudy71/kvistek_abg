using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Manager : NetworkManager {
    public static bool InitWithOnePlayer = false;
    
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public override void OnStartServer() {
        base.OnStartServer();
    }

    public override void OnServerConnect(NetworkConnection conn) {
        if (conn.connectionId == 0 && GameManager.GAMEMANAGER == null) {
            base.OnServerConnect(conn);
            InitWithOnePlayer = true;
            return;
        }

        if (GameManager.GAMEMANAGER.MatchStarted) {
            conn.Disconnect();
            return;
        }

        base.OnServerConnect(conn);
        GameManager.GAMEMANAGER.Players++;
    }
}