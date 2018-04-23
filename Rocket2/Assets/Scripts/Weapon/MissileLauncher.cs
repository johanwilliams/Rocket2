using UnityEngine;

public class MissileLauncher : ProjectileWeapon {

    // Simple lasergun that don't need any modifications from the base class
    protected override void Start() {
        base.Start();
        displayName = "a missile launcher";
    }
}
