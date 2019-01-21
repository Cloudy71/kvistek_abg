using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerGUI : NetworkBehaviour {
    [HideInInspector]
    public bool ShowPickup = false;

    private PlayerData _playerData;

    private float _redScreen;
    private float _blackScreen;

    // Use this for initialization
    void Start() {
        _playerData = GetComponent<PlayerData>();
        _redScreen = 0f;
        _blackScreen = 0f;
    }

    // Update is called once per frame
    void Update() {
    }

    private void FixedUpdate() {
        if (_redScreen > 0f) {
            _redScreen -= 0.01f;
            if (_redScreen < 0f) _redScreen = 0f;
        }

        if (_blackScreen > 0f) {
            _blackScreen -= 0.0025f;
            if (_blackScreen < 0f) _blackScreen = 0f;
        }
    }

    public void GoRed() {
        TargetGoRed(connectionToClient);
    }

    [TargetRpc]
    public void TargetGoRed(NetworkConnection target) {
        _redScreen = 0.5f;
    }

    public void GoBlack() {
        TargetGoBlack(connectionToClient);
    }

    [TargetRpc]
    public void TargetGoBlack(NetworkConnection target) {
        _blackScreen = 1f;
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

        GUI.color = new Color(1f, 1f, 1f, _redScreen);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), AssetLoader.GetColor(Color.red));
        GUI.color = new Color(1f, 1f, 1f, _blackScreen);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), AssetLoader.GetColor(Color.black));
        GUI.color = new Color(1f, 1f, 1f);

        if (ShowPickup) {
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2f - 160f, Screen.height / 2f + 64f, 320f, 24f),
                      "Press <E> to pick up weapon.");
        }
    }
}