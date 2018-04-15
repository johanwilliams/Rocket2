using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody2D))]
public class RocketShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";    

    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;
    private Player player;
    private Rigidbody2D rb;
    private RocketWeapon primaryWeapon;
    private RocketWeapon secondaryWeapon;

    private float lastShotTime = 0f;

    private void Start() {
        weaponManager = GetComponent<WeaponManager>();
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable() {
        // Stop autofire
        CancelInvoke("Shoot");
    }

    private void Update() {
        if (PauseMenu.IsOn)
            return;

        primaryWeapon = weaponManager.GetCurrentWeapon();

        if (Input.GetButtonDown("Fire1") && isShootingAllowed()) {
            if (primaryWeapon.fireRate <= 0f)
                Shoot();    // Single fire
            else
                InvokeRepeating("Shoot", 0f, 1f/primaryWeapon.fireRate);    // Autofire
        } else if (Input.GetButtonUp("Fire1")) {
            // Stop autofire
            CancelInvoke("Shoot");
        }

        if (Input.GetButtonDown("Fire2")) {
            //weaponManager.FireSecondaryWeapon(player);
        }
    }

    // Check that we are not cheating the rapid fire but tapping fire
    private bool isShootingAllowed() {
        // Check the time since last shot
        if ((Time.time - lastShotTime) < 1f / primaryWeapon.fireRate)
            return false;        

        return true;
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
        AudioManager.instance.PlayClipAtPoint("LaserShot", transform.position);
    }

    // Renders a weapon trail (if we have one configured)
    private void RenderTrail(Vector3 _hitPos) {
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

        // Check that we have enough energy        
        if (primaryWeapon.energyCost > player.energy.GetEnergy())
            return;

        // Add the recoil (todo: Maybe refactor to a separate AddForce method as we want to add forces in other situations)
        rb.AddForce(-rb.transform.up * primaryWeapon.recoil * Time.deltaTime, ForceMode2D.Impulse);

        // Raycast to detect if we hit something (should perhaps be refactored for the specific weapon?)
        Transform _weaponSlot = weaponManager.GetWeaponSlot();
        RaycastHit2D _hit = Physics2D.Raycast(_weaponSlot.transform.position, _weaponSlot.transform.up, primaryWeapon.range, mask);
        Debug.DrawLine(_weaponSlot.transform.position, _weaponSlot.transform.position + _weaponSlot.transform.up * primaryWeapon.range, Color.cyan);

        Vector3 _hitPos = _weaponSlot.transform.position + _weaponSlot.transform.up * primaryWeapon.range;
        if (_hit.collider != null) {
            if (_hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot(_hit.collider.name, primaryWeapon.damage, transform.name);
            }
            _hitPos = _hit.point;

            // Call the OnHit method on the server
            CmdOnHit(_hitPos, _hit.normal);
        }

        // Call the OnShoot method on the server
        CmdOnShoot(_hitPos);

        lastShotTime = Time.time;

        player.energy.ConsumeEnergy(primaryWeapon.energyCost);
    }

    // Command (server side method) which takes care of a player shooting another player
    [Command]
    void CmdPlayerShot (string _playerID, int _damage, string _sourcePlayerID) {        

        // Get the player being shot from the game manager so we can damage him
        Player _player = GameManager.GetPlayer(_playerID);
        //_player.RpcTakeDamage(_damage, _sourcePlayerID);
        _player.health.TakeDamage(_damage, _sourcePlayerID);
    }
}
