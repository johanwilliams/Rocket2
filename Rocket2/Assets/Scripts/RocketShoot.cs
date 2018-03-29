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

    [Client]
    void Shoot() {
        Transform _weaponSlot = weaponManager.GetWeaponSlot();
        RaycastHit2D _hit = Physics2D.Raycast(_weaponSlot.transform.position, _weaponSlot.transform.up, currentWeapon.range, mask);
        Debug.DrawLine(_weaponSlot.transform.position, _weaponSlot.transform.position + _weaponSlot.transform.up * 100, Color.cyan);

        if (_hit.collider != null) {
            if (_hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }
        }
    }

    // Command (server side method) which takes care of a player shooting another player
    [Command]
    void CmdPlayerShot (string _playerID, int _damage) {        

        // Get the player being shot from the game manager so we can damage him
        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}
