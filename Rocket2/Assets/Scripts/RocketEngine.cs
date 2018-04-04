using System;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketEngine : NetworkBehaviour {

    [SerializeField]
    private float rotationSpeed = 200f;
    private float rotation = 0f;

    [SerializeField]
    private float thrusterForce = 1000f;
    private float thruster = 0f;

    //[SyncVar(hook = "OnThrusterChange")]
    public bool isThrustersOn = false;
    /*private bool _isThrustersOn = false;
    public bool isThrustersOn {
        get { return _isThrustersOn; }
        protected set { _isThrustersOn = value; }
    }*/

    private float fuelAmout = 1f;
    [SerializeField]
    private float fuelBurnSpeed = 0.05f;
    [SerializeField]
    private float fuelRegenSpeed = 0.01f;

    public float GetFuelAmount() {
        return fuelAmout;
    }
    

    [SerializeField]
    public Transform thrusterSlot;

    [SerializeField]
    private GameObject rocketFlamePrefab;
    private GameObject rocketFlameIns;

    private Rigidbody2D rb;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        InitRocketFlame();
    }

    // Instantiates a rocket flame from selected prefab at the rocket engine position
    private void InitRocketFlame() {        
        if (rocketFlamePrefab == null) {
            Debug.LogWarning("No rocket flame prefab configured");
        }
        else {
            rocketFlameIns = Instantiate(rocketFlamePrefab, thrusterSlot.position, thrusterSlot.rotation);
            rocketFlameIns.transform.SetParent(thrusterSlot);
        }
    }

    // Resets the engine (typically when respawning)
    public void Reset() {
        fuelAmout = 1f;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    // Sets the rocket engine rotation by taking a roation input (between -1 and 1) and adds the rotation speed.
    public void ApplyRotation(float _rotation) {
        rotation = _rotation * rotationSpeed;
    }

    // Sets the rocket engine thruster by taking a thruster input (between -1 and 1) and adds the thruster force.
    public void ApplyThruster(float _thruster) {
        thruster = _thruster * thrusterForce;
    }

    // Run every graphics update
    private void Update() {        
        UpdateFuel();                
    }

    // Run every physics iteration
    private void FixedUpdate() {
        if (!isLocalPlayer)
            return;

        PerformRotation();
        PerformMovement();
        
    }

    // Rotats the rocket
    private void PerformRotation() {
        if (Mathf.Abs(rotation) > 0f) {
            rb.MoveRotation(rb.rotation - rotation * Time.fixedDeltaTime);
        }
    }

    // Moves the rocket (applies thruster force and burns/regens fuel)
    private void PerformMovement() {
        // Only apply thruster if positive (i.e. no reverse thruster)
        if (thruster > 0f && fuelAmout > 0.01f) {                                                        
            rb.AddForce(rb.transform.up * thruster * Time.fixedDeltaTime);                       
            if (!isThrustersOn)
                CmdSetThruster(true);
        } else {
            if (isThrustersOn)
                CmdSetThruster(false);
        }        
    }

    // Notify the server that we have changed our thruster (so it can notify all clients which can render the rocket flame etc properly)
    [Command]
    private void CmdSetThruster(bool _isThrustersOn) {
        RpcSetThruster(_isThrustersOn);
    }

    // Notfy all clients of the change in thruster so they can redner rocket flame etc properly
    [ClientRpc]
    private void RpcSetThruster(bool _isThrustersOn) {
        isThrustersOn = _isThrustersOn;
        SetRocketFlame();
    }

    // Update our current fuel amount. Burn fuel if the thrusters are on (and we have fuel left). Refill fuel if we are standing on the ground.
    private void UpdateFuel() {
        if (isThrustersOn && fuelAmout >= 0.01f) {
            // Burn fuel
            fuelAmout -= fuelBurnSpeed * Time.deltaTime;
        } else if (rb.velocity == Vector2.zero){
            fuelAmout += fuelRegenSpeed * Time.deltaTime;
        }
        fuelAmout = Mathf.Clamp(fuelAmout, 0f, 1f);
    }

    // Update the rocket flame to match the state of our thruster. Thruster off disables emission, thrusters on enables the emission
    private void SetRocketFlame() {        
        if (rocketFlameIns != null) { 
            foreach (Transform child in rocketFlameIns.transform) {
                ParticleSystem.EmissionModule em = child.GetComponent<ParticleSystem>().emission;
                em.enabled = isThrustersOn;
            }
        }
    }
}
