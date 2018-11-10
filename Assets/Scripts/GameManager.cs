using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {
    public static GameManager GAMEMANAGER;

    [SyncVar]
    public int Players;

    [SyncVar]
    public int PlayersReady;

    [SyncVar]
    public bool MatchStarted;

    [SyncVar]
    public float TimeToStart = 30f;

    [SyncVar]
    public float TimeSinceStart = -1f;

    public GameObject Rider;
    public GameObject Bullet;

    private Text _textReady;

    // Use this for initialization
    void Start() {
        GAMEMANAGER = this;

        _textReady = GameObject.Find("LobbyCanvas").transform.GetChild(0).GetComponent<Text>();

        if (!isServer)
            return;

        MatchStarted = false;
        if (Manager.InitWithOnePlayer)
            Players++;
    }

    // Update is called once per frame
    void Update() {
        LobbyCanvasUpdate();

        if (!isServer) // Server space
            return;

        CheckIfPlayersAreReady();
    }

    private void LobbyCanvasUpdate() {
        _textReady.text = "Ready: " + PlayersReady + " / " + Players;
    }

    private void CheckIfPlayersAreReady() {
        if (MatchStarted)
            return;

        if (PlayersReady == Players && Players != 0) {
            StartMatch();
        }
    }

    public void StartMatch() {
        if (MatchStarted)
            return;

        MatchStarted = true;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            player.GetComponent<PlayerData>().TargetRide(player.GetComponent<PlayerData>().connectionToClient);
        }
    }
}