using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerBehaviour : NetworkBehaviour {
    private Camera     _camera;
    private PlayerData _playerData;

    private GameObject _spawnObject;

    private bool _mouseMove;

    // Use this for initialization
    void Start() {
        _camera = Camera.main;
        _playerData = GetComponent<PlayerData>();
        _mouseMove = true;
        _spawnObject = GameObject.Find("SPAWN");
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

        _camera.transform.parent = transform;
        _camera.transform.localPosition = new Vector3(0f, .8f, 0f);

        if (!_playerData.IsDead) {
            MouseMovement();
            EquipChecking();
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

        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y                  + mx, 0f);
        _camera.transform.localEulerAngles = new Vector3(_camera.transform.eulerAngles.x - my, 0f, 0f);
    }

    private void EquipChecking() {
        RaycastHit[] hits = Physics.RaycastAll(_camera.transform.position, _camera.transform.forward, 3f);
        foreach (RaycastHit raycastHit in hits) {
            //TODO: Pickup
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
}