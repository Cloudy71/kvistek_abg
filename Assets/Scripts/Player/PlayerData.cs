using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerData : NetworkBehaviour {
    [SyncVar]
    public bool IsReady = false;

    [SyncVar]
    public bool IsDead = false;

    [SyncVar]
    public float Health = 100f;

    [SyncVar]
    public float HealthMax = 100f;

    [SyncVar]
    public float DamageAdder = 0f;

    [SyncVar]
    public float Speed = 3.5f;

    [SyncVar]
    public float SprintSpeed = 5f;

    [SyncVar]
    public int Ammo = 12;

    [SyncVar]
    public float DeadSince = 0f;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (IsDead && isServer) {
            if (Time.time >= DeadSince + 5f) {
                Respawn();
            }
        }
    }

    public void TakeDamage(float damage, GameObject source = null) {
        if (IsDead)
            return;

        CmdTakeDamage(damage);

        if (Health <= 0f) {
            TargetDie(connectionToClient, source.transform.forward);
        }
    }

    [Command]
    public void CmdTakeDamage(float damage) {
        Health -= damage;
        if (Health <= 0f) {
            CmdDie();
        }
    }

    [Command]
    public void CmdDie() {
        if (!isServer)
            return;
        IsDead = true;
        DeadSince = Time.time;
    }

    public void Respawn() {
        IsDead = false;
        Health = 100f;

        TargetRespawn(connectionToClient);
    }

    [TargetRpc]
    public void TargetDie(NetworkConnection target, Vector3 sourceForward) {
        GetComponent<Rigidbody>().freezeRotation = false;
        GetComponent<Rigidbody>().AddForce(sourceForward * 10000f);
    }

    [TargetRpc]
    public void TargetRespawn(NetworkConnection target) {
        GetComponent<Rigidbody>().freezeRotation = true;
        transform.position = Manager.singleton.GetStartPosition().position;
        transform.eulerAngles = Vector3.zero;
    }

    [TargetRpc]
    public void TargetRide(NetworkConnection target) {
        Ride();
    }

    public void Ride() {
        Vector3 pos = new Vector3(Random.Range(0f, 500f), 450f, Random.Range(0f, 500f));
        GameObject rider = Instantiate(GameManager.GAMEMANAGER.Rider, pos, Quaternion.identity);
        transform.position = pos + new Vector3(0f, 0.5f, 0f);
    }
}