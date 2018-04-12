using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class NetworkTransform : NetworkBehaviour {

    [SerializeField] private Transform _transform;
    private float lerpRate;
    private float lerpRateNormal = 15;
    private float lerpRateFast = 25;
    private int lerpRateFastSize = 10;

    [SerializeField] private bool useHistoricLerping = false;
    private float lerpNextValueDistance = 0.2f;

    [SyncVar(hook ="SyncPositionValues")]
    private Vector3 syncPosition;    // Server will sync the position to all clients        
    private Vector3 lastPosition;
    private List<Vector3> syncPositionList = new List<Vector3>();
    [SerializeField] private float movementThreshold = 0.5f;

    [SyncVar]
    private Quaternion syncRotation;    // Server will sync the rotation to all clients        
    private Quaternion lastRotation;
    [SerializeField] private bool includeRotation = false;
    [SerializeField] private float rotationThreshold = 5f;

    private void Start() {
        lerpRate = lerpRateNormal;
    }

    private void FixedUpdate() {
        TransmitPosition();
        TransmitRotation();
    }

    private void Update() {
        LerpPosition();
        LerpRotation();
    }

    private void UpdateLerpSpeed(float _lerpSpeed, int lerpListSize) {
        //TODO: Maybe this can be smoothed out even more to have the lerprate adjust linear to the size?
        // lerpSpeed = lerpSpeedNormal + (syncPosList.Count * y);
        // A recommendation is also to tie the lerp value to the speed of the transform
        if (lerpListSize > lerpRateFastSize)
            _lerpSpeed = lerpRateFast;
        else
            _lerpSpeed = lerpRateNormal;
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
        _transform.position = Vector3.Lerp(_transform.position, syncPosition, Time.deltaTime * lerpRate);
    }

    // Use historical lerping. Takes position history into consideration
    private void LerpPositionHistorical() {
        if (syncPositionList.Count > 0) {
            _transform.position = Vector3.Lerp(_transform.position, syncPositionList[0], Time.deltaTime * lerpRate);

            // Check if we are close enough to the next waypoint (and there is a next waypoint) so we should start lerping towards that instead
            if (Vector3.Distance(_transform.position, syncPositionList[0]) < lerpNextValueDistance && syncPositionList.Count > 1) {
                syncPositionList.RemoveAt(0);
            }

            UpdateLerpSpeed(lerpRate, syncPositionList.Count);
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

    [Client]
    private void SyncPositionValues(Vector3 _syncPosition) {
        syncPosition = _syncPosition;
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
            _transform.rotation = Quaternion.Lerp(_transform.rotation, syncRotation, Time.deltaTime * lerpRate);
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
