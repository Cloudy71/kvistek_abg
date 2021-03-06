﻿using System.Collections;
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
    public float DeadSince = 0f;

    [SyncVar]
    public int CurrentWeapon = -1;

    [HideInInspector]
    public float LastDamaged;

    [HideInInspector]
    public string[] DeathAnimations;

    // Use this for initialization
    void Start() {
        DeathAnimations = new[] {
                                    "Standing Death Backward 01", "Standing React Death Forward", "Death From Right",
                                    "Standing Death Forward 01", "Standing React Death Left",
                                    "Death From Front Headshot", "Standing Death Forward 02",
                                    "Flying Back Death", "Death From Back Headshot", "Standing Death Left 01",
                                    "Falling Forward Death", "Death", "Standing Death Left 02",
                                    "Falling Back Death (1)", "Death From The Back", "Standing Death Right 01",
                                    "Falling Back Death", "Standing React Death Backward", "Standing Death Right 02",
                                    "Death From The Front"
                                };
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
        if (IsDead || !isServer)
            return;

        Health -= damage;
        int sideX, sideY;
        sideX = Random.Range(0, 2);
        sideY = Random.Range(0, 2);
        if (sideX == 0) sideX = -1;
        if (sideY == 0) sideY = -1;
        GetComponent<PlayerBehaviour>()
            .AddRecoil(new Vector2(Random.Range(damage / 2f, damage / 2f + 2f) * sideX,
                                   Random.Range(damage / 2f, damage / 2f + 2f) * sideY));
        GetComponent<PlayerGUI>().GoRed();

        LastDamaged = Time.time;

        if (Health <= 0f) {
            IsDead = true;
            DeadSince = Time.time + 1000f;
            TargetDie(connectionToClient, source.transform.forward);
            GetComponent<PlayerBehaviour>().PlayAnimation(DeathAnimations[Random.Range(0, DeathAnimations.Length)]);
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
        GetComponent<Rigidbody>().freezeRotation = true;
        //GetComponent<Rigidbody>().AddForce(sourceForward * 10000f);
    }

    [TargetRpc]
    public void TargetRespawn(NetworkConnection target) {
        GetComponent<Rigidbody>().freezeRotation = true;
        transform.position = Manager.singleton.GetStartPosition().position;
        transform.eulerAngles = Vector3.zero;
    }

    [TargetRpc]
    public void TargetRide(NetworkConnection target, Vector3 pos) {
        Ride(pos);
    }

    public void Ride(Vector3 pos) {
        transform.position = pos + new Vector3(0f, 0.5f, 0f);
    }

    public GameObject GetWeapon() {
        return CurrentWeapon == -1 ? null : transform.GetChild(0).GetChild(CurrentWeapon).gameObject;
    }

    public Weapon GetWeaponData() {
        return CurrentWeapon == -1 ? null : transform.GetChild(0).GetChild(CurrentWeapon).GetComponent<Weapon>();
    }

    [Command]
    public void CmdPickUp(NetworkInstanceId weaponId) {
        GameObject weapon = NetworkServer.FindLocalObject(weaponId);
        weapon.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        weapon.transform.parent = transform.GetChild(0);
        weapon.GetComponent<Weapon>().IsPicked = true;
        CmdEquip(transform.GetChild(0).childCount - 1);
    }

    [Command]
    public void CmdDrop() {
        GameObject weapon = GetWeapon();
        weapon.GetComponent<NetworkIdentity>().RemoveClientAuthority(connectionToClient);
        weapon.transform.parent = null;
        weapon.GetComponent<Weapon>().IsPicked = false;
    }

    [Command]
    public void CmdEquip(int weaponSlot) {
        if (transform.GetChild(0).childCount <= weaponSlot) {
            return;
        }

        Weapon oldWeapon = GetWeaponData();
        if (oldWeapon != null) {
            oldWeapon.IsUsed = false;
        }

        CurrentWeapon = weaponSlot;
        GetWeaponData().IsUsed = true;
    }
}