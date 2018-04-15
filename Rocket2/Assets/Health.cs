using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    [SerializeField] private int maxHealth = 100;
    [SyncVar(hook = "OnHealthChanged")] private int health;
    private bool isDead;
    private bool shouldDie;

    public delegate void DiedAction(string source);
    public static event DiedAction OnDeath;

    private void Start() {
        Reset();
    }

    // Returns the current health in percentage (often used in UI for helthbars)
    public float GetHealthPct() {
        return health / maxHealth;
    }

    // Returns the current health
    public float GetHealth() {
        return health;
    }

    public void Reset() {
        health = maxHealth;
        isDead = false;
        shouldDie = false;
    }

    public void TakeDamage(int damage, string source) {
        if (isDead)
            return;

        SetHealth(health - damage);
        Debug.Log(transform.name + " took " + damage + " from " + source + " and now has " + health + " HP in health");

        if (isDead && OnDeath != null)
            OnDeath(source);
        
    }

    public void OnHealthChanged(int _health) {
        SetHealth(_health);
    }

    private void SetHealth(int _health) {
        health = Mathf.Clamp(_health, 0, maxHealth);
        if (health <= 0)
            isDead = true;
    }
}
