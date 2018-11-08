﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerData : NetworkBehaviour {
    [SyncVar]
    public bool IsReady = false;

    [SyncVar]
    public float Health = 100f;

    [SyncVar]
    public float HealthMax = 100f;

    [SyncVar]
    public float DamageAdder = 0f;

    [SyncVar]
    public float Speed = 3.5f;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}