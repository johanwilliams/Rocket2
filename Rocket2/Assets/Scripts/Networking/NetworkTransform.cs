using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class NetworkTransform : NetworkBehaviour {

    [SerializeField] private Transform _transform;
    [SerializeField] private float defaultLerpRate = 15;

    // Used for historic lerping
    [SerializeField] private bool useHistoricLerping = true;
    [SerializeField] private float lerpNextPositionDistance = 0.2f;
    [SerializeField] private float lerpNextRotationAngle = 0.5f;

    [SyncVar(hook ="SyncPositionValues")]
    private Vector3 syncPosition;    // Server will sync the position to all clients        
    private Vector3 lastPosition;
    private List<Vector3> syncPositionList = new List<Vector3>();
    private float lerpRatePosition;
    [SerializeField] private float movementThreshold = 0.5f;

    [SyncVar]
    private Quaternion syncRotation;    // Server will sync the rotation to all clients        
    private Quaternion lastRotation;
    private float lerpRateRotation;
    [SerializeField] private bool includeRotation = false;
    [SerializeField] private float rotationThreshold = 5f;

    private void Start() {
        lerpRatePosition = defaultLerpRate;
        lerpRateRotation = defaultLerpRate;
    }

    private void FixedUpdate() {
        TransmitPosition();
        TransmitRotation();
    }

    private void Update() {
        LerpPosition();
        LerpRotation();
    }

    #region "Movement"
    // Lerp the position for all remote transforms
    private void LerpPosition() {
        if (!isLocalPlayer) {
            if (useHistoricLerping) 
                LerpPositionHistorical();
            else 
                LerpPositionNormal();            
        }
    }

    // Use normal lerping. Takes no poition history into consideration
    private void LerpPositionNormal() {
        _transform.position = Vector3.Lerp(_transform.position, syncPosition, Time.deltaTime * lerpRatePosition);
    }

    // Use historical lerping. Takes position history into consideration
    private void LerpPositionHistorical() {
        if (syncPositionList.Count > 0) {
            _transform.position = Vector3.Lerp(_transform.position, syncPositionList[0], Time.deltaTime * lerpRatePosition);

            // Check if we are close enough to the next waypoint (and there is a next waypoint) so we should start lerping towards that instead
            if (Vector3.Distance(_transform.position, syncPositionList[0]) < lerpNextPositionDistance && syncPositionList.Count > 1) {
                syncPositionList.RemoveAt(0);
            }

            // Update the lerp rate depending on the size of the list
            lerpRatePosition = defaultLerpRate + syncPositionList.Count;
        }
    }

    // Send our (local tranform) position to the server
    [ClientCallback]
    private void TransmitPosition() {
        if (isLocalPlayer && Vector3.Distance(_transform.position, lastPosition) > movementThreshold) {
            CmdProvidePositionToServer(_transform.position);
            lastPosition = _transform.position;
        }
    }

    // Update the last known position we got from the server.
    [Client]
    private void SyncPositionValues(Vector3 _syncPosition) {
        syncPosition = _syncPosition;
        if (useHistoricLerping && !isLocalPlayer)
            syncPositionList.Add(syncPosition);
    }

    // Only run on server. Updates the syncPosition for the clients
    [Command]
    private void CmdProvidePositionToServer(Vector3 position) {
        syncPosition = position;  // Since syncPosition is a sync var it will be sent to all clients
    }
    #endregion

    #region "Rotation"
    // Lerp the rotation for all remote transforms
    private void LerpRotation() {
        if (!isLocalPlayer && includeRotation)
            _transform.rotation = Quaternion.Lerp(_transform.rotation, syncRotation, Time.deltaTime * lerpRateRotation);
    }


    // Send our (local tranform) rotation to the server
    [ClientCallback]
    private void TransmitRotation() {
        if (isLocalPlayer && includeRotation && Quaternion.Angle(_transform.rotation, lastRotation) > rotationThreshold) {
            CmdProvideRotationToServer(_transform.rotation);
            lastRotation = _transform.rotation;
        }

    }

    // Only run on server. Updates the syncRotation for the clients
    [Command]
    private void CmdProvideRotationToServer(Quaternion rotation) {
        syncRotation = rotation;  // Since syncRotation is a sync var it will be sent to all clients
    }
    #endregion

}
