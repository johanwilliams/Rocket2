using System;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    private Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "PlayerRemote";

    private void Start()
    {
        if (isLocalPlayer)
        {
            AttachCamera();
        } else {            
            DisableComponents();
            AssignRemoteLayer();
        }     
    }

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
