using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {
    [SyncVar]
    public float Damage;

    [SyncVar]
    public float DamageHeadshot;

    [SyncVar]
    public float Speed;

    [SyncVar]
    public uint SourceId;

    private GameObject _spawnObject;

    // Use this for initialization
    void Start() {
        _spawnObject = GameObject.Find("SPAWN");
    }

    // Update is called once per frame
    void Update() {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed, transform);
        if (transform.position.x < 0f                                 || transform.position.x > 500f ||
            transform.position.y < 0f                                 ||
            transform.position.y >= _spawnObject.transform.position.y || transform.position.z < 0f ||
            transform.position.z > 500f) {
            Destroy(gameObject);
        }

        RaycastHit[] raycastHits =
            Physics.RaycastAll(transform.position, transform.forward, Speed * Time.deltaTime / 2f);

        GameObject nearest = null;
        float dist = float.MaxValue;
        foreach (RaycastHit raycastHit in raycastHits) {
            if (raycastHit.transform.gameObject.Equals(gameObject)) {
                continue;
            }

            if (raycastHit.distance <= dist) {
                dist = raycastHit.distance;
                nearest = raycastHit.transform.gameObject;
            }
        }

        if (nearest != null) {
            if (nearest.tag.Equals("Player")) {
                nearest.GetComponent<PlayerData>().TakeDamage(Damage, gameObject);
            }

            Destroy(gameObject);
        }
    }
}