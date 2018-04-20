using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : LaserWeapon {

    // Simple lasergun that don't need any modifications from the base class
    protected override void Start() {
        base.Start();
        displayName = "a lasergun";
    }

}
