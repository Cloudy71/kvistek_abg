using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using Random = System.Random;

public class PlayerBehaviour : NetworkBehaviour {
    [HideInInspector]
    public Vector2 Recoil;

    [HideInInspector]
    public Vector2 DesiredRecoil;

    [HideInInspector]
    public Vector2 ShotRecoilStep;

    [HideInInspector]
    public Vector2 HitRecoilStep;

    public  AudioClip[] Steps;
    private int         _lastSoundId;

    private Camera     _camera;
    private PlayerData _playerData;
    private PlayerGUI  _playerGui;

    private GameObject _spawnObject;

    private bool    _mouseMove;
    private float   _cameraX;
    private float   _recoilStart = -1f;
    private Vector2 _recoilBegin = Vector2.zero;

    // Use this for initialization
    void Start() {
        _camera = Camera.main;
        _playerData = GetComponent<PlayerData>();
        _playerGui = GetComponent<PlayerGUI>();
        _mouseMove = true;
        _spawnObject = GameObject.Find("SPAWN");

        ShotRecoilStep = new Vector2(.025f, .4f);
    }

    // Update is called once per frame
    void Update() {
        if (transform.position.y < -20f) {
            if (isServer) {
                _playerData.Respawn();
            }
        }

        if (!isLocalPlayer)
            return;

        GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        _camera.transform.parent = transform;
        _camera.transform.localPosition = new Vector3(0f, .8f, 0f);
        _camera.transform.localEulerAngles = new Vector3(_cameraX + Recoil.y, Recoil.x, 0f);

        if (!_playerData.IsDead) {
            MouseMovement();
            EquipChecking();
        }
    }

    private void FixedUpdate() {
        // TODO: Make recoil but in moving in time not points.
        if (!_recoilStart.Equals(-1f)) {
            Recoil.x = _recoilBegin.x +
                       (Time.time - _recoilStart) * (DesiredRecoil.x - _recoilBegin.x) /
                       (DesiredRecoil.Equals(Vector2.zero) ? ShotRecoilStep.y : ShotRecoilStep.x);
            Recoil.y = _recoilBegin.y +
                       (Time.time - _recoilStart) * (DesiredRecoil.y - _recoilBegin.y) /
                       (DesiredRecoil.Equals(Vector2.zero) ? ShotRecoilStep.y : ShotRecoilStep.x);

            if (Time.time > _recoilStart + ShotRecoilStep.x) {
                _recoilStart = -1f;
            }
        }
        else if (!Recoil.Equals(Vector2.zero)) {
            SetRecoil(Vector2.zero);
        }
    }

    private void MouseMovement() {
        if (Input.GetKeyDown(KeyCode.M))
            _mouseMove = !_mouseMove;

        Cursor.lockState = _mouseMove ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !_mouseMove;

        if (!_mouseMove)
            return;

        float mx, my;
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y + mx, 0f);
        _cameraX -= my;
    }

    private void EquipChecking() {
        RaycastHit[] hits = Physics.RaycastAll(_camera.transform.position, _camera.transform.forward, 3f);
        GameObject nearest = null;
        float dist = float.MaxValue;
        int type = -1;
        foreach (RaycastHit raycastHit in hits) {
            if (raycastHit.transform.tag.Equals("Weapon")) {
                if (raycastHit.distance < dist) {
                    nearest = raycastHit.transform.gameObject;
                    dist = raycastHit.distance;
                    type = 0;
                }
            }
        }

        _playerGui.ShowPickup = false;
        if (nearest != null) {
            if (type == 0) {
                if (!nearest.GetComponent<Weapon>().IsPicked) {
                    _playerGui.ShowPickup = true;
                    if (Input.GetKeyDown(KeyCode.E)) {
                        _playerData.CmdPickUp(nearest.GetComponent<NetworkIdentity>().netId);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name.Equals("ReadyPlane")) {
            if (!GameManager.GAMEMANAGER.MatchStarted) {
                _playerData.IsReady = true;

                if (isServer) {
                    GameManager.GAMEMANAGER.PlayersReady++;
                }
            }
            else {
                CmdRide();
            }
        }
    }

    [Command]
    public void CmdRide() {
        GameManager.GAMEMANAGER.RidePlayer(gameObject);
    }

    private void OnTriggerExit(Collider other) {
        if (other.name.Equals("ReadyPlane") && !GameManager.GAMEMANAGER.MatchStarted) {
            _playerData.IsReady = false;

            if (isServer) {
                GameManager.GAMEMANAGER.PlayersReady--;
            }
        }
    }

    public void SetRecoil(Vector2 recoil) {
        _setRecoil(recoil);
    }

    private void _setRecoil(Vector2 recoil) {
        _recoilStart = Time.time;
        _recoilBegin = Recoil;
        DesiredRecoil = recoil;
    }

    public void AddRecoil(Vector2 recoil) {
        Vector2 rec = Recoil + recoil;
        TargetAddRecoil(connectionToClient, rec);
    }

    [TargetRpc]
    public void TargetAddRecoil(NetworkConnection target, Vector2 recoil) {
        _setRecoil(recoil);
    }

    [Command]
    public void CmdMakeStep() {
        Random rnd = new Random();
        int snd;

        do {
            snd = rnd.Next(0, Steps.Length);
        } while (snd == _lastSoundId);

        _lastSoundId = snd;

        RpcMakeStep(snd);
    }

    [ClientRpc]
    public void RpcMakeStep(int snd) {
        GetComponent<AudioSource>().PlayOneShot(Steps[snd]);
    }
}