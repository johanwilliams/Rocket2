using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision) {
        var hit = collision.gameObject;
        if (hit.layer != LayerMask.NameToLayer("PlayerLocal")) { 
            Debug.Log("Bullet hit " + hit.name);
            Destroy(gameObject);
        }
    }
}
