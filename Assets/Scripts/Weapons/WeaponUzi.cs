using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUzi : Weapon {
	// Use this for initialization

	public override void Stats() {
		Name = "Uzi";
		MaxAmmo = 30;
		Ammo = MaxAmmo;
		MaxMagazine = MaxAmmo * 4;
		Magazine = MaxMagazine;
		IsPrimary = true;
		DamageBody = 20f;
		DamageHeadshot = 60f;
		BulletSpeed = 200f;
		Cooldown = .1f;

		RecoilX = new Vector2(-1f, 1f);
		RecoilY = new Vector2(-2f, -2f);
		RecoilXDiff = 1f;
		RecoilYDiff = 1f;
		RecoilBackTime = 1f;
		MoveDistraction = 1f;
		SprintDistraction = 2f;
		
		WeaponPosition = new Vector3(0.1f, -0.15f, 0.325f);
	}
}