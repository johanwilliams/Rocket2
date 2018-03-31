using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketEngine : MonoBehaviour {

    [SerializeField]
    private float rotationSpeed = 200f;
    private float rotation = 0f;

    [SerializeField]
    private float thrusterForce = 1000f;
    private float thruster = 0f;
    private bool thrustersOn;

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
    private bool rocketFlameEnabled = false;

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
            SetRocketFlame(false);
        }
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
        UpdateRocketFlame();
    }

    // Run every physics iteration
    private void FixedUpdate() {
        PerformRotation();
        PerformMovement();
        ReFuel();
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
        if (thruster > 0f && fuelAmout > 0f) {            
            // Burn fuel
            fuelAmout -= fuelBurnSpeed * Time.fixedDeltaTime;

            if (fuelAmout >= 0.01f) {
                thrustersOn = true;
                rb.AddForce(rb.transform.up * thruster * Time.fixedDeltaTime);
            }            
        } else {            
            thrustersOn = false;
        }
        fuelAmout = Mathf.Clamp(fuelAmout, 0f, 1f);
    }

    private void ReFuel() {
        if (rb.velocity == Vector2.zero) {
            fuelAmout += fuelRegenSpeed * Time.fixedDeltaTime;
        }
    }

        private void UpdateRocketFlame() {        
        if (rocketFlameIns != null && thrustersOn != rocketFlameEnabled) {            
            rocketFlameEnabled = thrustersOn;
            Debug.Log("Setting rocket flame: " + rocketFlameEnabled);
            SetRocketFlame(rocketFlameEnabled);
        }
    }

    private void SetRocketFlame(bool _enabled) {
        foreach (Transform child in rocketFlameIns.transform) {
            ParticleSystem.EmissionModule em = child.GetComponent<ParticleSystem>().emission;
            em.enabled = _enabled;
        }
    }
}
