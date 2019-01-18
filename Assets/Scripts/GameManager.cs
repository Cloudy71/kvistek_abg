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

    public GameObject   Rider;
    public GameObject   Bullet;
    public GameObject[] RandomWeapons;
    public GameObject   BulletHole;

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

    public void RidePlayer(GameObject player) {
        Vector3 pos = new Vector3( /*Random.Range(0f, 500f), 450f, Random.Range(0f, 500f)*/32f, 50f, 104f);
        GameObject rider = Instantiate(Rider, pos, Quaternion.identity);
        NetworkServer.Spawn(rider);
        player.GetComponent<PlayerData>().TargetRide(player.GetComponent<PlayerData>().connectionToClient, pos);
    }

    public void StartMatch() {
        if (MatchStarted)
            return;

        MatchStarted = true;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            RidePlayer(player);
        }
    }
}