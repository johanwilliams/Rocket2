using UnityEngine;
using UnityEngine.Networking;

public class RocketShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";
    
    public RocketWeapon weapon;

    [SerializeField]
    private GameObject gun;

    [SerializeField]
    private LayerMask mask;

    
    private void Start() {
        // Use this for serialization
        if (gun == null) {
            Debug.LogError("RocketShoot: No reference to gun transform");
            this.enabled = false;
        }
    }

    private void Update() {
        if (Input.GetButtonDown("Fire1")) {
            Shoot();
        }
    }

    [Client]
    void Shoot() {
        RaycastHit2D _hit = Physics2D.Raycast(gun.transform.position, gun.transform.up, weapon.range, mask);
        Debug.DrawLine(gun.transform.position, gun.transform.position + gun.transform.up * 100, Color.cyan);

        if (_hit.collider != null) {
            if (_hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot(_hit.collider.name, weapon.damage);
            }
        }
    }

    [Command]
    void CmdPlayerShot (string _playerID, int _damage) {
        Debug.Log(_playerID + " has been shot.");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.TakeDamage(_damage);
    }
}
