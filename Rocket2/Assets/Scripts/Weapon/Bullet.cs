using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : Weapon {

    public GameObject bulletPrefab;    

    private void OnCollisionEnter2D(Collision2D collision) {
        var hit = collision.gameObject;
        if (hit.layer != LayerMask.NameToLayer("PlayerLocal")) { 
            Debug.Log("Bullet hit " + hit.name);
            Destroy(gameObject);
        }
    }
    
    public override void Shoot(Player shooter, Vector3 position, Quaternion rotation, Vector3 direction) {
        var bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * speed;
        
        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }


}
