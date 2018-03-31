using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class RocketShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";    

    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;
    private RocketWeapon currentWeapon;


    private void Start() {
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update() {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon.fireRate <= 0f) {
            // Single fire
            if (Input.GetButtonDown("Fire1")) {
                Shoot();
            }
        } else {
            // Autofire
            if (Input.GetButtonDown("Fire1")) {
                // Start autofire. Repeat value to calculate RPM --> seconds
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);    
            } else if (Input.GetButtonUp("Fire1")) {
                // Stop autofire
                CancelInvoke("Shoot");
            }
        }
    }

    // Called on the server when a player shoots
    [Command]
    void CmdOnShoot(Vector3 _hitPos) {
        RpcDoShootEffect(_hitPos);
    }

    // Called on all clients when we need to do a shoot effect
    [ClientRpc]
    void RpcDoShootEffect(Vector3 _hitPos) {
        weaponManager.GetCurrentWeaponGraphics().mussleFlash.Play();
        RenderTrail(_hitPos);
    }

    // Renders a weapon trail (if we have one configured)
    private void RenderTrail(Vector3 _hitPos) {
        Debug.Log("RENDER TRAIL");
        GameObject trailEffectPrefab = weaponManager.GetCurrentWeaponGraphics().trailEffectPrefab;
        if (trailEffectPrefab != null) {
            Transform _weaponSlot = weaponManager.GetWeaponSlot();
            GameObject trailClone = Instantiate(trailEffectPrefab, _weaponSlot.position, _weaponSlot.rotation);            
            LineRenderer lr = trailClone.GetComponent<LineRenderer>();
            if (lr != null) {
                lr.SetPosition(0, _weaponSlot.transform.position);
                lr.SetPosition(1, _hitPos);
            }
            Destroy(trailClone, 0.04f);
        }
    }

    // Called on the server when we hit something. Takes the hitpoint and the normal of the hit surface as parameters
    [Command]
    void CmdOnHit(Vector3 _hitPos, Vector3 _normal) {
        RpcDoHitEffect(_hitPos, _normal);
    }

    // Called on all clienct to show hit effect
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _hitPos, Vector3 _normal) {

        //TODO: Object pooling could come in useful here if we do a lot of instantiating
        GameObject _hitEffect = Instantiate(weaponManager.GetCurrentWeaponGraphics().hitEffectPrefab, _hitPos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    [Client]
    void Shoot() {

        // Only do the raycast if we are the local player
        if (!isLocalPlayer)
            return;

        Transform _weaponSlot = weaponManager.GetWeaponSlot();
        RaycastHit2D _hit = Physics2D.Raycast(_weaponSlot.transform.position, _weaponSlot.transform.up, currentWeapon.range, mask);
        Debug.DrawLine(_weaponSlot.transform.position, _weaponSlot.transform.position + _weaponSlot.transform.up * currentWeapon.range, Color.cyan);

        Vector3 _hitPos = _weaponSlot.transform.position + _weaponSlot.transform.up * currentWeapon.range;
        if (_hit.collider != null) {
            if (_hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }
            _hitPos = _hit.point;

            // Call the OnHit method on the server
            CmdOnHit(_hitPos, _hit.normal);
        }

        // Call the OnShoot method on the server
        CmdOnShoot(_hitPos);
    }

    // Command (server side method) which takes care of a player shooting another player
    [Command]
    void CmdPlayerShot (string _playerID, int _damage) {        

        // Get the player being shot from the game manager so we can damage him
        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}
