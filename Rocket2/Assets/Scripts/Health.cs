using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField] private int maxHealth = 100;
    [SerializeField] [SyncVar(hook = "OnHealthChanged")] private int health;

    public delegate void DiedAction(string source);
    public static event DiedAction OnDeath;

    private bool eventTriggered;

    private void Start() {
        Reset();
    }

    private void Update() {
        if (health <= 0) {
            if (eventTriggered)
                return;
            if (OnDeath != null)
                OnDeath("Test");
            eventTriggered = true;
        }
    }

    // Returns the current health in percentage (often used in UI for helthbars)
    public float GetHealthPct() {
        return health / (float) maxHealth;
    }

    // Returns the current health
    public float GetHealth() {
        return health;
    }

    public void Reset() {
        health = maxHealth;
    }

    internal void TakeDamage(int damage, string source) {
        if (health <= 0)
            return;
        
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        Debug.Log(transform.name + " took " + damage + " from " + source + " and now has " + health + " HP in health");
    }

    public void OnHealthChanged(int _health) {
        health = _health;
    }
}
