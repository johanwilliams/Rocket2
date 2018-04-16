using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon {

    public GameObject bulletPrefab;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Shoot(Player shooter, Vector3 position, Quaternion rotation, Vector3 direction) {
        throw new System.NotImplementedException();
    }
}
