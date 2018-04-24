using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : ProjectileWeapon {

    protected override void Start() {
        base.Start();
        displayName = "a gun";
    }
}
