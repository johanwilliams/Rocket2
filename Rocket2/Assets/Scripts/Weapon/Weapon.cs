using System;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Weapon : NetworkBehaviour {

    public enum Slot { Primary, Seconday };

    public Slot slot;
    public int damage;
    public int range;
    public int speed;   //TODO: Move to a projectile class of the gameobject we are shooting?
    public int fireRate;
    public int energyCost;

    public Transform firePoint;

    public ParticleSystem mussleFlash;
    private AudioSource fireSound;

    protected float lastShotTime;

    protected virtual void Start() {
        fireSound = GetComponent<AudioSource>();
        if (fireSound == null)
            Debug.LogWarning("No AudioSource component added as a fire sound on weapon " + this.GetType().Name);
    }

    // Checks if shooting is allowd, updates the shot tiem and calls the on weapon shot 
    public virtual void Shoot(Player shooter) {
        if (!isShootingAllowed(shooter))
            return;

        lastShotTime = Time.time;        
        OnWeaponShot(shooter);
        shooter.energy.ConsumeEnergy(energyCost);
    }    

    // Calls the shooter game manager (so it can update the server of the shot)
    protected virtual void OnWeaponShot(Player shooter) {
        shooter.rocketWeapons.CmdOnWeaponShot(slot);
    }

    // Play the muzzle flash and play the fire sound
    public virtual void DoShotEffects() {
        if (mussleFlash != null)
            mussleFlash.Play();

        if (fireSound != null)
            fireSound.Play();
    }

    public virtual void OnShootAndHit(Vector3 hitPosition, Vector3 hitNormal) {
        DoShotEffects();
        //Implement the hit effect in overriding classes
    }

    // Check if shooting is allowed
    public virtual bool isShootingAllowed(Player shooter) {
        return checkTime() && checkEnergy(shooter);
    }

    private bool checkTime() {
        if (fireRate == 0f)
            return true;
        else 
            return (Time.time - lastShotTime) >= (1f / fireRate);
    }

    private bool checkEnergy(Player shooter) {
        Debug.Log(shooter.energy.GetEnergy() + ":" + energyCost);
        return shooter.energy.GetEnergy() >= energyCost;
    }

    private void cSSheckEnergy(Player shooter) {
        if (shooter.energy.GetEnergy() < energyCost)
            throw new ShootException("Not enough energy to shoot");
    }
}

[Serializable]
internal class ShootException : Exception {
    public ShootException() {
    }

    public ShootException(string message) : base(message) {
    }

    public ShootException(string message, Exception innerException) : base(message, innerException) {
    }

    protected ShootException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}