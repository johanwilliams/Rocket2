using System;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(RocketEngine))]
public class PlayerSetup : NetworkBehaviour {

    // A list of all game object componets to disable for all remote players
    [SerializeField]
    private Behaviour[] remoteComponentsToDisable;

    [SerializeField]
    private Behaviour[] localComponentsToDisable;


    [SerializeField]
    private string remoteLayerName = "PlayerRemote";

    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    private void Start()
    {
        if (isLocalPlayer)
        {
            // Stuff todo for the local player
            AttachCamera();

            // Create playerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            //Configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            
            if (ui == null)
                Debug.LogError("No PlayerUI component on PlayerUI game object");
            else
                ui.SetPlayer(GetComponent<Player>());

            // Call setup on the player to setup all propeties for the player
            GetComponent<Player>().SetupPlayer();

            // Set the username of the player (or the player id if not logged in)
            string _username = transform.name;
            if (UserAccountManager.isLoggedIn)
                _username = UserAccountManager.playerUsername;
            CmdSetUsername(transform.name, _username);

            DisableComponents(localComponentsToDisable);
        } else {            
            // Stuff to do for all remote players
            DisableComponents(remoteComponentsToDisable);
            AssignRemoteLayer();                
        }
    }

    // Set the username of the player on the server. Since the username is a syncvar variable all clients will be notified
    [Command]
    void CmdSetUsername(string playerID, string username) {
        Player player = GameManager.GetPlayer(playerID);
        if (player != null) {
            Debug.Log(username + " has joined the game!");
            player.username = username;
        }
    }

    // Called when a cliend connects. Registers the player with the game manager
    public override void OnStartClient() {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
    }

    // Assign remote player to the remote player layer
    private void AssignRemoteLayer() {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    // Disable all components 
    void DisableComponents(Behaviour[] componentsToDisable) {        
        foreach (Behaviour component in componentsToDisable)
            component.enabled = false;
    }

    // Attaching the camera to follow the local player
    void AttachCamera() {
        Camera.main.GetComponent<CameraFollow>().target = transform;
    }

    // Called when we are ddestroyed
    private void OnDisable() {
        GameManager.UnregisterPlayer(transform.name);
    }
}
