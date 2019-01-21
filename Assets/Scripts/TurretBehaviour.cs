using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TurretBehaviour : NetworkBehaviour {
    [SyncVar]
    public float AttackSpeed;

    [SyncVar]
    public float Damage;

    [SyncVar]
    public float Range;

    [HideInInspector]
    public GameObject Target;

    private float _lastAttack;

    // Start is called before the first frame update
    void Start() {
        _lastAttack = Time.time;
    }

    // Update is called once per frame
    void Update() {
        if (Target != null) {
            transform.GetChild(0).LookAt(Target.transform.position);
            transform.GetChild(0).eulerAngles = new Vector3(0f, transform.GetChild(0).eulerAngles.y + 180f, 0f);
        }

        if (!isServer)
            return;

        if (Target == null) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players) {
                if (Vector3.Distance(player.transform.position, transform.position) <= Range) {
                    Target = player;
                    break;
                }
            }
        }
        else {
            if (Vector3.Distance(Target.transform.position, transform.position) > Range ||
                Target.GetComponent<PlayerData>().IsDead) {
                Target = null;
            }
            else {
                if (Time.time > _lastAttack + 1f / AttackSpeed) {
                    _lastAttack = Time.time;
                    GameObject bullet = Instantiate(GameManager.GAMEMANAGER.Bullet);
                    bullet.GetComponent<Bullet>().Damage = Damage;
                    bullet.GetComponent<Bullet>().DamageHeadshot = Damage;
                    bullet.GetComponent<Bullet>().Speed = 190f;
                    bullet.transform.position = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).position;
                    bullet.transform.LookAt(Target.transform.position + new Vector3(0f, 1f, 0f));
                    NetworkServer.Spawn(bullet);
                }
            }
        }
    }
}