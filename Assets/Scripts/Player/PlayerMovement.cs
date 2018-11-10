using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {
    private PlayerData _playerData;
    private Camera     _camera;
    private GameObject _spawnObject;

    // Use this for initialization
    void Start() {
        _playerData = GetComponent<PlayerData>();
        _camera = Camera.main;
        _spawnObject = GameObject.Find("SPAWN");
    }

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer)
            return;

        if (!_playerData.IsDead) {
            KeyboardMovement();
            if (transform.position.y < _spawnObject.transform.position.y)
                MouseAttack();
        }
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
            if (transform.position.y > _spawnObject.transform.position.y) {
                velocity.y = 0f;
            }
        }

        GetComponent<Rigidbody>().velocity = velocity;
    }

    private void MouseAttack() {
        Debug.DrawRay(_camera.transform.position, _camera.transform.forward * 10f, Color.blue);
        if (Input.GetMouseButtonDown(0) && _playerData.Ammo > 0) {
            CmdShotBullet(_camera.transform.position, _camera.transform.forward);
        }
    }

    [Command]
    public void CmdShotBullet(Vector3 position, Vector3 dir) {
        RaycastHit[] raycastHits =
            Physics.RaycastAll(position, dir, 512f);

        Vector3 point = Vector3.zero;
        float dist = float.MaxValue;
        foreach (RaycastHit raycastHit in raycastHits) {
            if (raycastHit.transform.gameObject.Equals(gameObject)) {
                continue;
            }

            if (raycastHit.distance <= dist) {
                dist = raycastHit.distance;
                point = raycastHit.point;
            }
        }

        GameObject bullet = Instantiate(GameManager.GAMEMANAGER.Bullet,
                                        position - new Vector3(0f, 0.2f, 0f),
                                        Quaternion.identity);
        if (!point.Equals(Vector3.zero)) {
            bullet.transform.LookAt(point);
        }
        else {
            bullet.transform.rotation = _camera.transform.rotation;
        }

        bullet.GetComponent<Bullet>().Damage = 45f;
        bullet.GetComponent<Bullet>().Speed = 190f;
        NetworkServer.Spawn(bullet);
    }
}