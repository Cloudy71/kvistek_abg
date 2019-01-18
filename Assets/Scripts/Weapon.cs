using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Animations;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using Random = System.Random;

public abstract class Weapon : NetworkBehaviour {
    public enum WeaponSound {
        Shot = 0
    }

    [HideInInspector]
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

    [HideInInspector]
    public bool IsUsed;

    [HideInInspector]
    public Vector2 RecoilX;

    [HideInInspector]
    public Vector2 RecoilY;

    [HideInInspector]
    public float RecoilXDiff;

    [HideInInspector]
    public float RecoilYDiff;

    [HideInInspector]
    public float RecoilBackTime;

    [HideInInspector]
    public float MoveDistraction;

    [HideInInspector]
    public float SprintDistraction;
    // TODO: Use distractions

    [HideInInspector]
    public Vector3 WeaponPosition;

    public AudioClip ShotSound;

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
            if (IsUsed) {
                _model.transform.localPosition = WeaponPosition;
                _model.transform.localEulerAngles = Vector3.zero;
            }
            else {
                _model.transform.localPosition = Vector3.zero;
                _model.transform.localEulerAngles = Vector3.zero;
            }

            if (hasAuthority) {
                foreach (MeshRenderer meshRenderer in _model.GetComponentsInChildren<MeshRenderer>()) {
                    meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                }
            }
            else {
                foreach (MeshRenderer meshRenderer in _model.GetComponentsInChildren<MeshRenderer>()) {
                    meshRenderer.shadowCastingMode = ShadowCastingMode.On;
                }
            }

            GetComponent<Rigidbody>().isKinematic = true;
        }
        else {
            _model.GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
            _model.transform.parent = transform;
            _model.transform.localPosition = Vector3.zero;
        }
    }

    public abstract void Stats();

    public virtual bool CanShot() {
        return Time.time >= LastShot + Cooldown && Ammo > 0;
    }

    public virtual GameObject Shot(Vector3 point) {
        if (!CanShot() || !isServer)
            return null;

        GameObject bullet = Instantiate(GameManager.GAMEMANAGER.Bullet);
        bullet.GetComponent<Bullet>().Damage = DamageBody;
        bullet.GetComponent<Bullet>().DamageHeadshot = DamageHeadshot;
        bullet.GetComponent<Bullet>().Speed = BulletSpeed;
        bullet.transform.position = _model.GetChild(0).position;
        bullet.transform.LookAt(point);

        //Ammo--;
        LastShot = Time.time;

        if (IsPicked && IsUsed) {
            Random rnd = new Random();
            transform.parent.parent.GetComponent<PlayerBehaviour>()
                     .AddRecoil(new Vector2((float) rnd.NextDouble() * (RecoilX.y - RecoilX.x) + RecoilX.x,
                                            (float) rnd.NextDouble() * (RecoilY.y - RecoilY.x) + RecoilY.x));
        }

        RpcPlaySound(WeaponSound.Shot);

        return bullet;
    }

    [ClientRpc]
    public void RpcPlaySound(WeaponSound snd) {
        AudioClip sound = null;

        switch (snd) {
            case WeaponSound.Shot:
                sound = ShotSound;
                break;
        }

        if (sound == null)
            return;

        GetComponent<AudioSource>().PlayOneShot(sound);
    }
}