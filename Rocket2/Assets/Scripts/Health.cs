using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField] private int maxHealth = 100;
    [SyncVar] private int health;
    [SerializeField] private bool destroyOnDeath = false;

    // Delegate and Action called when health reaches 0 and we die
    public delegate void DiedAction(string source);
    public event DiedAction OnDeath;

    private string source;
    private bool isDead;

    private void Start() {
        Reset();

        // Subscibe to our own action if we are to destroy this gameobject on death
        if (destroyOnDeath)
            OnDeath += CmdDie;
    }

    // Calles the OnDeath action if health reacehs 0 (and we are not already dead)
    private void Update() {
        if (health <= 0 && !isDead) {
            isDead = true;
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
        isDead = false;
    }

    // Update the current health on all clients
    public void TakeDamage(int damage, string _source) {
        if (!isServer || health <= 0)
            return;

        health = Mathf.Clamp(health - damage, 0, maxHealth);
        source = _source;
        Debug.Log(transform.name + " took " + damage + " from " + source + " and now has " + health + " HP in health");
    }

    // Called if we die and if we should destoy on death
    [Command]
    private void CmdDie(string source) {
        Debug.Log(gameObject.name + " was killed by " + source);
        NetworkServer.Destroy(gameObject);
    }
}
