using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerGUI : NetworkBehaviour {
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnGUI() {
        if (!isLocalPlayer)
            return;

        Texture bloodyScreen = AssetLoader.GetTexture("Sprites/bloody_screen");

        GUI.color = new Color(1f, 1f, 1f, 0f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bloodyScreen, ScaleMode.StretchToFill);
        GUI.color = new Color(1f, 1f, 1f);
    }
}