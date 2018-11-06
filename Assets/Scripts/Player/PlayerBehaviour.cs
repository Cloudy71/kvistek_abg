using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerBehaviour : NetworkBehaviour {
    private Camera     _camera;
    private PlayerData _playerData;

    private bool _mouseMove;

    // Use this for initialization
    void Start() {
        _camera = Camera.main;
        _playerData = GetComponent<PlayerData>();
        _mouseMove = true;
    }

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer)
            return;

        _camera.transform.parent = transform;
        _camera.transform.localPosition = new Vector3(0f, 1.8f, 0f);

        MouseMovement();
    }

    private void MouseMovement() {
        if (Input.GetKeyDown(KeyCode.M))
            _mouseMove = !_mouseMove;
        if (!_mouseMove)
            return;

        float mx, my;
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y                  + mx, 0f);
        _camera.transform.localEulerAngles = new Vector3(_camera.transform.eulerAngles.x - my, 0f, 0f);
    }
}