using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDelete : MonoBehaviour {
    public float TimeToDelete;

    private float _start;

    // Use this for initialization
    void Start() {
        _start = Time.time;
    }

    // Update is called once per frame
    void Update() {
        if (Time.time >= _start + TimeToDelete) {
            Destroy(gameObject);
        }
    }
}