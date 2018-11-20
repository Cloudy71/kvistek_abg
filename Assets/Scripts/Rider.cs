using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Rider : NetworkBehaviour {
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (transform.position.y < 20f && GetComponent<Rigidbody>().velocity.y > -2f) {
            Destroy(gameObject);
        }
    }
}