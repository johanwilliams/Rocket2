using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : Weapon {

    public GameObject bulletPrefab;    

    public override void Shoot(Player shooter) {
        base.Shoot(shooter);

        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        NetworkServer.Spawn(bullet);

    }
}
