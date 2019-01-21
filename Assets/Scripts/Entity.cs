using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Entity : NetworkBehaviour {
    [SyncVar]
    public float Health = 100f;

    [SyncVar]
    public float DamageReduction;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (!isServer) return;

        if (Health < 0f) {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage) {
        if (!isServer) return;

        Health -= damage * (1f - DamageReduction);
    }
}