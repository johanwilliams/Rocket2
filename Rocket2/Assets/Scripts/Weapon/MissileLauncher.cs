using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MissileLauncher : Weapon {

    public GameObject missilePrefab;    

    public override void Shoot(Player shooter) {
        base.Shoot(shooter);

        var missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);        
        //missile.GetComponent<Rigidbody2D>().velocity = firePoint.up * speed;
        NetworkServer.Spawn(missile);

        //Destroy(missile, 2.0f);
    }
}
