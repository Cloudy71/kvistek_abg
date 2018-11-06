using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	private PlayerData _playerData;
	
	// Use this for initialization
	void Start () {
		_playerData = GetComponent<PlayerData>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
			return;
		
		KeyboardMovement();
	}

	private void KeyboardMovement() {
		
	}
}
