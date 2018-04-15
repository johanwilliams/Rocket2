using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour {

    [SerializeField] private float maxEnergy = 100f;
    private float energy;
    [SerializeField] private float regenSpeed = 4f;

    private void Start() {
        Reset();
    }

    private void Update() {
        RegenEnergy();
    }

    public float GetEnergyPct() {
        return energy / maxEnergy;
    }

    public float GetEnergy() {
        return energy;
    }

    public void Reset() {
        energy = maxEnergy;
    }

    public void ConsumeEnergy(float _energy) {
        energy -= _energy;
        energy = Mathf.Clamp(energy, 0f, maxEnergy);
    }

    void RegenEnergy() {
        energy += regenSpeed * Time.deltaTime;
        energy = Mathf.Clamp(energy, 0f, maxEnergy);
    }

}
