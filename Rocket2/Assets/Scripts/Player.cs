using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(RocketEngine))]
public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    private RocketEngine rocketEngine;

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    // A list of all game object components to disable/enable when the player dies/respawns
    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private void Start() {
        rocketEngine = GetComponent<RocketEngine>();
    }

    // The player setup method which initiate all variables, components etc at spawn
    public void Setup() {        
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++) {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }


    // DEBUG method only to kill the local player instantly
    private void Update() {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K)) {
            RpcTakeDamage(99999);
        }
    }

    // Sets the defaul values for the player at startup
    public void SetDefaults() {
        isDead = false;
        currentHealth = maxHealth;

        // Reset all components
        for (int i = 0; i < disableOnDeath.Length; i++) 
            disableOnDeath[i].enabled = wasEnabled[i];

        // Enable GameObjects
        foreach (GameObject gameObject in disableGameObjectsOnDeath)
            gameObject.SetActive(true);

        // Special case since a collider is not deriving from Behaviour
        Collider _col = GetComponent<Collider>();
        if (_col != null) {
            _col.enabled = true;
        }

        // Spawn a deatch effect
        GameObject _spawnEffectInst = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_spawnEffectInst, 3f);

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

        // Disable components
        foreach (Behaviour component in disableOnDeath)
            component.enabled = false;

        // Disable GameObjects
        foreach (GameObject gameObject in disableGameObjectsOnDeath)
            gameObject.SetActive(false);

        // Special case since a collider is not deriving from Behavioud
        Collider _col = GetComponent<Collider>();
        if (_col != null) {
            _col.enabled = false;
        }

        // Spawn a deatch effect
        GameObject _deatchEffectInst = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_deatchEffectInst, 3f);

        StartCoroutine(Respawn());
    }

    // Respawns a player
    private IEnumerator Respawn() {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;
        rocketEngine.Reset();

        SetDefaults();

        Debug.Log("Player " + transform.name + " respawned at spawnpoint " + _spawnPoint.transform.name);
    }
}
