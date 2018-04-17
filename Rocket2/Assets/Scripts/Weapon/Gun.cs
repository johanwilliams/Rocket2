using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : Weapon {

    public GameObject bulletPrefab;    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Shoot(Player shooter) {
        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = firePoint.up * speed;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }
}
