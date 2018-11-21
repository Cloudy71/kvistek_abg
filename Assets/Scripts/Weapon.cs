using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour {
    //[HideInInspector]
    public string Name;

    [HideInInspector]
    public int MaxAmmo;

    [HideInInspector]
    [SyncVar]
    public int Ammo;

    [HideInInspector]
    public int MaxMagazine;

    [HideInInspector]
    public bool IsPrimary;

    [HideInInspector]
    public float DamageBody;

    [HideInInspector]
    public float DamageHeadshot;

    [HideInInspector]
    public float BulletSpeed;

    [HideInInspector]
    public float Cooldown;

    [HideInInspector]
    [SyncVar]
    protected float LastShot;

    [HideInInspector]
    [SyncVar]
    public int Magazine;

    [HideInInspector]
    [SyncVar]
    public bool IsPicked;

    private Transform model;

    // Use this for initialization
    void Start() {
        model = transform.GetChild(0);
        Stats();
    }

    // Update is called once per frame
    void Update() {
        if (IsPicked) {
            transform.localPosition = Vector3.zero;
            model.GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else {
            model.GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public virtual void Stats() {
    }

    public virtual bool CanShot() {
        return Time.time >= LastShot + Cooldown && Ammo > 0;
    }

    public virtual GameObject Shot(Vector3 position, Vector3 point) {
        if (!CanShot())
            return null;

        GameObject bullet = Instantiate(GameManager.GAMEMANAGER.Bullet);
        bullet.GetComponent<Bullet>().Damage = DamageBody;
        bullet.GetComponent<Bullet>().DamageHeadshot = DamageHeadshot;
        bullet.GetComponent<Bullet>().Speed = BulletSpeed;
        bullet.transform.position = position;
        bullet.transform.LookAt(point);

        Ammo--;

        return bullet;
    }
}