using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(RocketEngine))]
public class Player : NetworkBehaviour {

    private RocketEngine rocketEngine;

    [SyncVar]
    public string username = "Loading";

    public int kills;
    public int deaths;


    // A list of all game object components to disable/enable when the player dies/respawns
    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private AudioClip deathSound;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;     

    // The player setup method which initiate all variables, components etc at spawn
    public void SetupPlayer() {
        CmdBroadcastNewPlayerSetup();
    }

    // Server mwthod that calls all clients, notifying them of a new player being setup
    [Command]
    private void CmdBroadcastNewPlayerSetup() {
        RpcSetupPlayerOnAllClients();
    }

    // Called on all clients to setup a new player
    [ClientRpc]
    private void RpcSetupPlayerOnAllClients() {
        if (firstSetup) {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++) {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }
        

        SetDefaults();
    }

    private void Start() {
        rocketEngine = GetComponent<RocketEngine>();
    }

    // DEBUG method only to kill the local player instantly
    private void Update() {
        if (!isLocalPlayer)
            return;

        RegenEnergy();

        //TODO: Remove. For debug only
        if (Input.GetKeyDown(KeyCode.K)) {
            RpcTakeDamage(20, transform.name);
        }
    }

    // Sets the defaul values for the player at startup
    public void SetDefaults() {
        SetHealthDefaults();
        SetEnergyDefaults();

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

        // Instantiate a spawn effect
        GameObject _spawnEffectInst = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_spawnEffectInst, 3f);
    }


    #region "Health functionality"
    [SerializeField]
    private float maxHealth = 100;
    [SyncVar]
    private float currentHealth;

    // Returns the current health in percentage (often used in UI for helthbars)
    public float GetHealthPct() {
        return currentHealth / maxHealth;
    }

    // Returns the current health in percentage (often used in UI for helthbars)
    public float GetHealth() {
        return currentHealth;
    }

    private void SetHealthDefaults() {
        isDead = false;
        currentHealth = maxHealth;
    }

    [SyncVar]
    private bool _isDead = false;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    
    // RPC call going out to all players to they can update which player took damage
    [ClientRpc]
    internal void RpcTakeDamage(float damage, string _sourcePlayerID) {
        if (isDead)
            return;

        currentHealth -= damage;
        Debug.Log(transform.name + " took " + damage + " HP damage and now has " + currentHealth + " HP in health");

        if (currentHealth <= 0) {            
            Die(_sourcePlayerID);
        }
    }
    #endregion

    #region "Energy functionality"
    [SerializeField]
    private float maxEnergy = 100f;
    private float currentEnergy;
    [SerializeField]
    private float energyRegenSpeed = 4f;

    public float GetEnergyPct() {
        return currentEnergy / maxEnergy;
    }

    public float GetEnergy() {
        return currentEnergy;
    }

    private void SetEnergyDefaults() {
        currentEnergy = maxEnergy;
    }

    public void ConsumeEnergy(float energy) {
        currentEnergy -= energy;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
    }

    private void RegenEnergy() {
        currentEnergy += energyRegenSpeed * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
    }
    #endregion

    #region "Die and respawn functionality"
    // Called when a player dies (health <= 0)
    private void Die(string _sourcePlayerID) {
        Debug.Log(transform.name + " is dead");
        isDead = true;

        // Update kill/death stats
        Player sourcePlayer = GameManager.GetPlayer(_sourcePlayerID);
        if (sourcePlayer != null) {            
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);

            // Add a kill (if you don't kill yourself)
            if (!username.Equals(sourcePlayer.username))
                sourcePlayer.kills++;
        }
        deaths++;

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

        // Kill the engine
        rocketEngine.Kill();

        // Spawn a deatch effect
        GameObject _deatchEffectInst = Instantiate(deathEffect, transform.position, Quaternion.identity);
        //AudioManager.instance.PlayClipAtPoint("Explosion1", transform.position);
        AudioManager.instance.Play("Explosion1");
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

        // Give some time for the tranform to be sent to all clients before reseting the player
        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log("Player " + transform.name + " respawned at spawnpoint " + _spawnPoint.transform.name);
    }
    #endregion
}
