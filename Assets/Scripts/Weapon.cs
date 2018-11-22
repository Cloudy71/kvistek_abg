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

    private Transform _model;
    private Camera    _camera;

    // Use this for initialization
    void Start() {
        _model = transform.GetChild(0);
        _camera = Camera.main;
        Stats();
    }

    // Update is called once per frame
    void Update() {
        if (IsPicked) {
            transform.localPosition = Vector3.zero;
            _model.GetComponent<Collider>().enabled = false;
            _model.transform.parent = _camera.transform;
            _model.transform.localPosition = new Vector3(0.1f, -0.05f, 0.325f);
            _model.transform.localEulerAngles = Vector3.zero;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else {
            _model.GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
            _model.transform.parent = transform;
            _model.transform.localPosition = Vector3.zero;
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