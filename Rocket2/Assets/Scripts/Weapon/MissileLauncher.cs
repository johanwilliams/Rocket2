using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MissileLauncher : Weapon {

    public HomingMissile missilePrefab;    

    public override void Shoot(Player shooter) {
        base.Shoot(shooter);

        HomingMissile missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        NetworkServer.Spawn(missile.gameObject);

    }
}
