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

    public float GetHealthPct() {
        return (float)currentHealth / maxHealth;
    }

    private float energyMaxAmount = 100f;
    private float energyAmount;
    [SerializeField]
    private float energyRegenSpeed = 4f;

    public float GetEnergyPct() {
        return energyAmount / energyMaxAmount;
    }

    public float GetEnergy() {
        return energyAmount;
    }

    public void ConsumeEnergy(float energy) {
        energyAmount -= energy;
        energyAmount = Mathf.Clamp(energyAmount, 0f, energyMaxAmount);
    }

    private void RegenEnergy() {
        energyAmount += energyRegenSpeed * Time.deltaTime;
        energyAmount = Mathf.Clamp(energyAmount, 0f, energyMaxAmount);
    }

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

    private void Start() {
        rocketEngine = GetComponent<RocketEngine>();
    }

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

    // DEBUG method only to kill the local player instantly
    private void Update() {
        if (!isLocalPlayer)
            return;

        RegenEnergy();

        if (Input.GetKeyDown(KeyCode.K)) {
            RpcTakeDamage(20, transform.name);
        }
    }

    // Sets the defaul values for the player at startup
    public void SetDefaults() {
        isDead = false;
        currentHealth = maxHealth;
        energyAmount = energyMaxAmount;

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

        // Spawn a spawn effect
        GameObject _spawnEffectInst = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_spawnEffectInst, 3f);

    }

    // RPC call going out to all players to they can update which player took damage
    [ClientRpc]
    internal void RpcTakeDamage(int damage, string _sourcePlayerID) {
        if (isDead)
            return;

        currentHealth -= damage;
        Debug.Log(transform.name + " took " + damage + " HP damage and now has " + currentHealth + " HP in health");

        if (currentHealth <= 0) {            
            Die(_sourcePlayerID);
        }
    }

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
}
