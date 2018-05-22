using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Smooth;

[RequireComponent(typeof(RocketEngine))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Energy))]
[RequireComponent(typeof(RocketWeapons))]
[RequireComponent(typeof(SmoothSync))]
public class Player : NetworkBehaviour {

    [HideInInspector] public RocketEngine rocketEngine;
    [HideInInspector] public Health health;
    [HideInInspector] public Energy energy;
    [HideInInspector] public RocketWeapons rocketWeapons;
    SmoothSync smoothSync;

    [SyncVar]
    public string username = "Loading";

    private float spawnDelay = 0.2f;

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

    public override void PreStartClient() {
        health = GetComponent<Health>();
        health.OnDeath += Die;
    }

    private void Start() {
        rocketEngine = GetComponent<RocketEngine>();
        energy = GetComponent<Energy>();
        rocketWeapons = GetComponent<RocketWeapons>();
        smoothSync = GetComponent<SmoothSync>();
    }

    // DEBUG method only to kill the local player instantly
    private void Update() {
        if (!isLocalPlayer)
            return;
    }

    // Sets the defaul values for the player at startup
    public void SetDefaults() {
        health.Reset();
        energy.Reset();

        ToggleComponents(true);

        // Instantiate a spawn effect
        GameObject _spawnEffectInst = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_spawnEffectInst, 3f);
    }

    // Enables and disables components and gameobjects on player death and respawn
    private void ToggleComponents(bool enable) {
        // Components
        for (int i = 0; i < disableOnDeath.Length; i++) {
            if (enable)
                disableOnDeath[i].enabled = wasEnabled[i];
            else
                disableOnDeath[i].enabled = false;
        }
            
        // GameObjects
        foreach (GameObject go in disableGameObjectsOnDeath)
            go.SetActive(enable);

        // Special case since a collider is not deriving from Behavioud
        Collider _col = GetComponent<Collider>();
        if (_col != null) {
            _col.enabled = enable;
        }
    }

    #region "Die and respawn functionality"
    // Called when a player dies (health <= 0)
    private void Die(string _sourcePlayerID) {
        Debug.Log(transform.name + " got killed by " + _sourcePlayerID);

        UpdateScore(_sourcePlayerID);
        ToggleComponents(false);
        rocketEngine.Kill();
        SpawnDeathEffect();

        StartCoroutine(Respawn());
    }

    private void UpdateScore(string _sourcePlayerID) {
        // Update kill/death stats
        Player sourcePlayer = GameManager.GetPlayer(_sourcePlayerID);
        if (sourcePlayer != null) {
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);

            // Add a kill (if you don't kill yourself)
            if (!username.Equals(sourcePlayer.username))
                sourcePlayer.kills++;
        }
        deaths++;
    }

    private void SpawnDeathEffect() {
        // Spawn a deatch effect
        GameObject _deatchEffectInst = Instantiate(deathEffect, transform.position, Quaternion.identity);
        AudioManager.instance.Play("Explosion1");
        Destroy(_deatchEffectInst, 3f);
    }

    // Respawns a player
    private IEnumerator Respawn() {
        // Clear the interpolation buffer
        smoothSync.clearBuffer();

        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;
        rocketEngine.Reset();

        // Give some time for the tranform to be sent to all clients before reseting the player
        yield return new WaitForSeconds(spawnDelay);

        SetupPlayer();

        Debug.Log("Player " + transform.name + " respawned at spawnpoint " + _spawnPoint.transform.name);
    }
    #endregion
}
