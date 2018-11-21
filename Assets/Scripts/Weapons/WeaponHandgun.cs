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
        Cooldown = 0.1f;
    }
}