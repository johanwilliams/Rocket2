using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel =0, sendInterval =0.033f)]
public class NetworkTransform : NetworkBehaviour {

    [SerializeField] Transform _transform;
    public float lerpRate = 15;

    [SyncVar]
    private Vector3 syncPosition;    // Server will sync the position to all clients        
    private Vector3 lastPosition;
    [SerializeField] float movementThreshold = 0.5f;
    
    [SyncVar]
    private Quaternion syncRotation;    // Server will sync the rotation to all clients        
    private Quaternion lastRotation;
    [SerializeField] bool includeRotation = false;
    [SerializeField] float rotationThreshold = 5f;

    private void FixedUpdate () {
        TransmitPosition();        
        TransmitRotation();        
	}

    private void Update() {
        LerpPosition();
        LerpRotation();
    }

    // Lerp the position for all remote transforms
    private void LerpPosition() {
        if (!isLocalPlayer)
            _transform.position = Vector3.Lerp(_transform.position, syncPosition, Time.deltaTime * lerpRate);       
    }

    // Lerp the rotation for all remote transforms
    private void LerpRotation() {
        if (!isLocalPlayer && includeRotation)
            _transform.rotation = Quaternion.Lerp(_transform.rotation, syncRotation, Time.deltaTime * lerpRate);
    }

    // Send our (local tranform) position to the server
    [ClientCallback]
    private void TransmitPosition() {
        if (isLocalPlayer && Vector3.Distance(_transform.position, lastPosition) > movementThreshold) {
            CmdProvidePositionToServer(_transform.position);
            lastPosition = _transform.position;
        }            
    }

    // Send our (local tranform) rotation to the server
    [ClientCallback]
    private void TransmitRotation() {
        if (isLocalPlayer && includeRotation && Quaternion.Angle(_transform.rotation, lastRotation) > rotationThreshold) {
            CmdProvideRotationToServer(_transform.rotation);
            lastRotation = _transform.rotation;
        }
            
    }

    // Only run on server. Updates the syncPosition for the clients
    [Command]
    private void CmdProvidePositionToServer(Vector3 position) {
        syncPosition = position;  // Since syncPosition is a sync var it will be sent to all clients
    }

    // Only run on server. Updates the syncRotation for the clients
    [Command]
    private void CmdProvideRotationToServer(Quaternion rotation) {
        syncRotation = rotation;  // Since syncRotation is a sync var it will be sent to all clients
    }

}
