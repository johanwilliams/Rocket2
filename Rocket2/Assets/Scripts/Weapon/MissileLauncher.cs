using UnityEngine;

public class MissileLauncher : ProjectileWeapon {

    protected override void Start() {
        base.Start();
        displayName = "a missile launcher";
    }
}
