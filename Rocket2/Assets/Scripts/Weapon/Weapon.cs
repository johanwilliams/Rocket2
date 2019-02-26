using System;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Weapon : NetworkBehaviour {

    public enum Slot { Primary, Seconday };

    public Slot slot;
    public string displayName;
    [SerializeField] protected int speed;   //TODO: Move to a projectile class of the gameobject we are shooting?
    public float fireRate;
    [SerializeField] protected int energyCost;
    [SerializeField] protected float recoil;

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
        Debug.Log(shooter.name + " fires a weapon!");

        UpdateShooter(shooter);                
        CmdDoShotEffect();        
    }    

    // Calls the server to notify all clients of the shot
    [Command]
    protected virtual void CmdDoShotEffect() {
        RpcDoShotEffect();
    }

    // Rpc call to render the shot effect
    [ClientRpc]
    protected virtual void RpcDoShotEffect() {
        DoShotEffect();
    }

    // Play the muzzle flash and play the fire sound
    protected virtual void DoShotEffect() {
        if (mussleFlash != null)
            mussleFlash.Play();

        if (fireSound != null)
            fireSound.Play();
    }

    // Check if shooting is allowed
    protected virtual void UpdateShooter(Player shooter) {
        // Check if we can shoot (fireRate and energy).
        checkTime();
        checkEnergy(shooter);

        // We can (otherwise exception is thrown) so update last shot time and energy
        lastShotTime = Time.time;
        shooter.energy.ConsumeEnergy(energyCost);

        // Add recoil to the shooter
        if (recoil > 0f)
            shooter.AddForce(-transform.up * recoil);
    }

    private void checkTime() {
        float timeMargin = 0.02f;
        if (fireRate != 0f && ((Time.time - lastShotTime + timeMargin) < (1f / fireRate)))
            throw new ShootException("Firerate of weapon does not permit to shoot yet");
    }   

    private void checkEnergy(Player shooter) {
        if (shooter.energy.GetEnergy() < energyCost)
            throw new ShootException("Not enough energy to shoot weapon");
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