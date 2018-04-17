using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public GameObject bulletPrefab;    

    private void OnCollisionEnter2D(Collision2D collision) {
        var hit = collision.gameObject;
        if (hit.layer != LayerMask.NameToLayer("PlayerLocal")) { 
            Debug.Log("Bullet hit " + hit.name);
            Destroy(gameObject);
        }
    }   
}
