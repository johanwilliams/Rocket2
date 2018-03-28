using System;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    // A list of all game object componets to disable for all remote players
    [SerializeField]
    private Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "PlayerRemote";

    private void Start()
    {
        if (isLocalPlayer)
        {
            // Stuff todo for the local player
            AttachCamera();
        } else {            
            // Stuff to do for all remote players
            DisableComponents();
            AssignRemoteLayer();
        }

        // Call setup on the player to setup all propeties for the player
        GetComponent<Player>().Setup();
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

    // Disable all components for the non-local player (i.e. rocket control scripts etc)
    void DisableComponents() {        
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
