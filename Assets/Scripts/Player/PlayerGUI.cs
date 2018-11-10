using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerGUI : NetworkBehaviour {
    private PlayerData _playerData;

    // Use this for initialization
    void Start() {
        _playerData = GetComponent<PlayerData>();
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnGUI() {
        if (!isLocalPlayer)
            return;

        Texture bloodyScreen = AssetLoader.GetTexture("Sprites/bloody_screen");
        Texture crosshair = AssetLoader.GetTexture("Sprites/crosshair");

        GUI.color = new Color(1f, 1f, 1f, (100f - _playerData.Health) / 100f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bloodyScreen, ScaleMode.StretchToFill);
        GUI.color = new Color(1f, 1f, 1f);
        GUI.DrawTexture(new Rect(Screen.width / 2f - 400f, Screen.height / 2f - 300f, 800f, 600f), crosshair);
    }
}