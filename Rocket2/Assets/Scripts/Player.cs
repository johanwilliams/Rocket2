using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    // A list of all game object components to disable/enable when the player dies/respawns
    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    // The player setup method which initiate all variables, components etc at spawn
    public void Setup() {        
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++) {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }


    // DEBUG method only to kill the local player instantly
    /*private void Update() {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K)) {
            RpcTakeDamage(99999);
        }
    }*/

    // Sets the defaul values for the player at startup
    public void SetDefaults() {
        isDead = false;
        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++) {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        // Special case since a collider is not deriving from Behavioud
        Collider _col = GetComponent<Collider>();
        if (_col != null) {
            _col.enabled = true;
        }
    }

    // RPC call going out to all players to they can update which player took damage
    [ClientRpc]
    internal void RpcTakeDamage(int damage) {
        if (isDead)
            return;

        currentHealth -= damage;
        Debug.Log(transform.name + " took " + damage + " HP damage and now has " + currentHealth + " HP in health");

        if (currentHealth <= 0) {
            Die();
        }
    }

    // Called when a player dies (health <= 0)
    private void Die() {
        Debug.Log(transform.name + " is dead");
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++) {
            disableOnDeath[i].enabled = false;
        }

        // Special case since a collider is not deriving from Behavioud
        Collider _col = GetComponent<Collider>();
        if (_col != null) {
            _col.enabled = false;
        }

        StartCoroutine(Respawn());
    }

    // Respawns a player
    private IEnumerator Respawn() {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;
        Debug.Log("Player " + transform.name + " respawned at spawnpoint " + _spawnPoint.transform.name);
    }
}
