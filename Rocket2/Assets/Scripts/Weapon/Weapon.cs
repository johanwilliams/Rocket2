using System;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Weapon : NetworkBehaviour {

    public enum Slot { Primary, Seconday };

    public Slot slot;
    public int damage;
    public int range;
    public int speed;
    public int fireRate;

    public Transform firePoint;

    public ParticleSystem mussleFlash;
    private AudioSource fireSound;

    public virtual void Shoot(Player shooter) {
        //TODO: Check energy. If not enough through exception
        //TODO: Check firerate. If not enough through exception
        shooter.weaponManager.CmdOnWeaponShot(slot);
    }

    protected virtual void Start() {
        fireSound = GetComponent<AudioSource>();
        if (fireSound == null)
            Debug.LogWarning("No AudioSource component added as a fire sound on weapon " + this.GetType().Name);
    }

    // Called on all clients when we need to do a shoot effect
    public virtual void OnShoot() {
        if (mussleFlash != null)
            mussleFlash.Play();

        if (fireSound != null)
            fireSound.Play();
    }

    public virtual void OnShootAndHit(Vector3 hitPosition, Vector3 hitNormal) {
        OnShoot();
        //TODO: Implement the hit effect
    }
}
