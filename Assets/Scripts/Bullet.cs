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
            transform.position.y < -20f                               ||
            transform.position.y >= _spawnObject.transform.position.y || transform.position.z < 0f ||
            transform.position.z > 500f) {
            Destroy(gameObject);
        }

        RaycastHit[] raycastHits =
            Physics.RaycastAll(transform.position - transform.forward * Speed * Time.deltaTime, transform.forward,
                               Speed * Time.deltaTime);

        if (raycastHits.Length > 0) {
            RaycastHit nearest = raycastHits[0];
            float dist = float.MaxValue;
            foreach (RaycastHit raycastHit in raycastHits) {
                if (raycastHit.transform.gameObject.Equals(gameObject)) {
                    continue;
                }

                if (raycastHit.distance <= dist && !(raycastHit.transform.tag.Equals("Player") &&
                                                     raycastHit.transform.GetComponent<PlayerData>().netId.Value ==
                                                     SourceId)) {
                    dist = raycastHit.distance;
                    nearest = raycastHit;
                }
            }

            if (!dist.Equals(float.MaxValue)) {
                if (nearest.transform.tag.Equals("Player")) {
                    if (isServer) {
                        nearest.transform.GetComponent<PlayerData>().TakeDamage(Damage, gameObject);
                    }
                }
                else if (nearest.transform.tag.Equals("Breakable")) {
                    if (isServer) {
                        nearest.transform.GetComponent<Entity>().TakeDamage(Damage);
                    }
                }
                else {
                    GameObject bulletHole =
                        Instantiate(GameManager.GAMEMANAGER.BulletHole, nearest.point,
                                    Quaternion.FromToRotation(Vector3.up, nearest.normal));
                    bulletHole.transform.Translate(bulletHole.transform.up * 0.025f);
                }

                Destroy(gameObject);
            }
        }
    }
}