using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField] private int maxHealth = 100;
    [SyncVar(hook = "OnHealthChanged")] private int health;
    [SerializeField] private bool destroyOnDeath = false;

    public delegate void DiedAction(string source);
    public event DiedAction OnDeath;


    private string source;
    private bool isDead;

    private void Start() {
        Reset();
        if (destroyOnDeath)
            OnDeath += Die;
    }

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

    [ClientRpc]
    public void RpcTakeDamage(int damage, string _source) {
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

    // Called if we die and if we should destoy on death
    private void Die(string source) {
        Debug.Log(gameObject.name + " was killed by " + source);
        Destroy(gameObject);
    }
}
