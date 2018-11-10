using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rider : MonoBehaviour {
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (transform.position.y < 100f && GetComponent<Rigidbody>().velocity.y > -2f) {
            Destroy(gameObject);
        }
    }
}