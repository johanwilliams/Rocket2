using System;
using UnityEngine;
using UnityEngine.Networking;

public class RocketSetup : NetworkBehaviour {

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

        RegisterPlayer();
    }

    // Sets the name of the player to the net id
    void RegisterPlayer() {
        transform.name = "Player " + GetComponent<NetworkIdentity>().netId;
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
}
