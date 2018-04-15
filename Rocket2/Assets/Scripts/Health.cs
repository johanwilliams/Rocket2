using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField] private int maxHealth = 100;
    [SerializeField] [SyncVar(hook = "OnHealthChanged")] private int health;

    public delegate void DiedAction(string source);
    public event DiedAction OnDeath;
    private string source;

    private bool eventTriggered;

    private void Start() {
        Reset();
    }

    private void Update() {
        if (health <= 0) {
            if (eventTriggered)
                return;
            eventTriggered = true;
            if (OnDeath != null)
                OnDeath(source);            
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
        eventTriggered = false;
    }

    internal void TakeDamage(int damage, string _source) {
        if (health <= 0)
            return;
        
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        source = _source;
        //if (health <= 0 && OnDeath != null)
        //    OnDeath(source);
        Debug.Log(transform.name + " took " + damage + " from " + source + " and now has " + health + " HP in health");
    }

    public void OnHealthChanged(int _health) {
        health = _health;
    }
}
