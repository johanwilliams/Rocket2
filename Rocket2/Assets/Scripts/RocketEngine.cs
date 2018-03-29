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

    [SerializeField]
    private Transform rocketFlame;
    private bool rocketFlameEnabled = false;

    private Rigidbody2D rb;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        SetRocketFlame(false);
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
    }

    // Rotats the rocket
    private void PerformRotation() {
        if (Mathf.Abs(rotation) > 0f) {
            rb.MoveRotation(rb.rotation - rotation * Time.fixedDeltaTime);
        }
    }

    // Moves the rocket (applies thruster force)
    private void PerformMovement() {
        // Only apply thruster if positive (i.e. no reverse thruster)
        if (thruster > 0f) {
            rb.AddForce(rb.transform.up * thruster * Time.fixedDeltaTime);
        }
    }

    private void UpdateRocketFlame() {
        bool thrustersOn = (thruster > 0f);
        if (thrustersOn != rocketFlameEnabled) {
            rocketFlameEnabled = thrustersOn;
            SetRocketFlame(rocketFlameEnabled);
        }
    }

    private void SetRocketFlame(bool _enabled) {
        foreach (Transform child in rocketFlame) {
            ParticleSystem.EmissionModule em = child.GetComponent<ParticleSystem>().emission;
            em.enabled = _enabled;
        }
    }
}
