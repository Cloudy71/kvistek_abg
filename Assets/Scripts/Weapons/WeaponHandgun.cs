using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandgun : Weapon {
    // Use this for initialization

    public override void Stats() {
        Name = "Handgun";
        MaxAmmo = 12;
        Ammo = MaxAmmo;
        MaxMagazine = MaxAmmo * 4;
        Magazine = MaxMagazine;
        IsPrimary = false;
        DamageBody = 30f;
        DamageHeadshot = 70f;
        BulletSpeed = 190f;
        Cooldown = .3f;

        RecoilX = new Vector2(-1f, 1f);
        RecoilY = new Vector2(-1f, -1f);
        RecoilXDiff = 1f;
        RecoilYDiff = 1f;
        RecoilBackTime = 1f;
        MoveDistraction = 1f;
        SprintDistraction = 2f;

        WeaponPosition = new Vector3(0.1f, -0.05f, 0.325f);
    }
}