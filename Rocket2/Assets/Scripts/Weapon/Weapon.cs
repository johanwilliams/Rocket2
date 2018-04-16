using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public abstract class Weapon : NetworkBehaviour {

    public enum Slot { Primary, Seconday };

    public Slot slot;
    public int damage;
    public int range;
    public int speed;
    public int fireRate;

    public ParticleSystem mussleFlash;

    public abstract void Shoot(Player shooter, Vector3 position, Quaternion rotation, Vector3 direction);

    // Called on all clients when we need to do a shoot effect
    [ClientRpc]
    internal void RpcShotEffect() {
        mussleFlash.Play();
        //AudioManager.instance.PlayClipAtPoint("LaserShot", transform.position);
    }

}
