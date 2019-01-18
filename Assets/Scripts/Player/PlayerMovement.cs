using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {
    private PlayerData      _playerData;
    private PlayerBehaviour _playerBehaviour;
    private Camera          _camera;
    private GameObject      _spawnObject;

    private float _stepCooldownMove;
    private float _stepCooldownSprint;
    private float _stepLast;

    // Use this for initialization
    void Start() {
        _playerData = GetComponent<PlayerData>();
        _playerBehaviour = GetComponent<PlayerBehaviour>();
        _camera = Camera.main;
        _spawnObject = GameObject.Find("SPAWN");
        _stepCooldownMove = 0.5f;
        _stepCooldownSprint = 0.35f;
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
        float speed = Input.GetKey(KeyCode.LeftShift) ? _playerData.SprintSpeed : _playerData.Speed;

        if (Input.GetKey(KeyCode.W)) {
            velocity += transform.forward * speed;
        }

        if (Input.GetKey(KeyCode.S)) {
            velocity -= transform.forward * speed;
        }

        if (Input.GetKey(KeyCode.D)) {
            velocity += transform.right * speed;
        }

        if (Input.GetKey(KeyCode.A)) {
            velocity -= transform.right * speed;
        }

        velocity.y = GetComponent<Rigidbody>().velocity.y;

        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, 1.1f)) {
            velocity.y += 5f;
            if (transform.position.y > _spawnObject.transform.position.y) {
                velocity.y = 0f;
            }
        }

        GetComponent<Rigidbody>().velocity = velocity;

        if (!velocity.x.Equals(0f) || !velocity.z.Equals(0f)) {
            bool sprint = Input.GetKey(KeyCode.LeftShift);
            if (Time.time >= _stepLast + (sprint ? _stepCooldownSprint : _stepCooldownMove)) {
                RaycastHit[] hits = Physics.RaycastAll(transform.position, -transform.up, 1.1f);
                bool col = false;
                foreach (RaycastHit raycastHit in hits) {
                    if (raycastHit.transform.name.Equals(name))
                        continue;

                    col = true;
                    break;
                }

                if (col) {
                    _stepLast = Time.time;
                    _playerBehaviour.CmdMakeStep();
                }
            }
        }
    }

    private void MouseAttack() {
        if (_playerData.CurrentWeapon != -1 && _playerData.GetWeaponData().CanShot()) {
            if (Input.GetMouseButtonDown(0) && !_playerData.GetWeaponData().IsPrimary) {
                CmdShotBullet(_camera.transform.position, _camera.transform.forward);
            }

            if (Input.GetMouseButton(0) && _playerData.GetWeaponData().IsPrimary) {
                CmdShotBullet(_camera.transform.position, _camera.transform.forward);
            }
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

        GameObject bullet = _playerData.GetWeaponData().Shot(point);
        if (point.Equals(Vector3.zero)) {
            bullet.transform.rotation = _camera.transform.rotation;
        }

        bullet.GetComponent<Bullet>().SourceId = netId.Value;

        NetworkServer.Spawn(bullet);
    }
}