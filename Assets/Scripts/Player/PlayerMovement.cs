using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {
    private PlayerData _playerData;

    // Use this for initialization
    void Start() {
        _playerData = GetComponent<PlayerData>();
    }

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer)
            return;

        KeyboardMovement();
    }

    private void KeyboardMovement() {
        Vector3 velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) {
            velocity += transform.forward * _playerData.Speed;
        }

        if (Input.GetKey(KeyCode.S)) {
            velocity -= transform.forward * _playerData.Speed;
        }

        if (Input.GetKey(KeyCode.D)) {
            velocity += transform.right * _playerData.Speed;
        }

        if (Input.GetKey(KeyCode.A)) {
            velocity -= transform.right * _playerData.Speed;
        }

        velocity.y = GetComponent<Rigidbody>().velocity.y;

        if (Input.GetKeyDown(KeyCode.Space)) {
            velocity.y += 5f;
        }

        GetComponent<Rigidbody>().velocity = velocity;
    }
}