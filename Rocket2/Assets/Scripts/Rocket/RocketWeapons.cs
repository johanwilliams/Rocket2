using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class RocketWeapons : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    private Player player;

    [SerializeField]
    private Transform weaponSlot;

    [SerializeField]
    private WeaponInventory.Name defaultWeapon;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;

    NHNetworkedPool bulletsPool;
    public float fireRate = 0.1f;
    float lastShotTime;
    private bool isFiring = false;


    private void Start() {
        player = GetComponent<Player>();
        // Use this for serialization
        if (weaponSlot == null) {
            Debug.LogError("RocketWeapons: No reference to weaponSlot transform");
            this.enabled = false;
        }
        //if (isLocalPlayer) {
          //  EquipWeapon(defaultWeapon);
        //}
            
        //EquipWeapon(WeaponInventory.Name.HomingMissile);

        bulletsPool = FindObjectOfType<NHNetworkedPool>();
        if (bulletsPool == null)
            Debug.LogError("No missile pool found");
    }

    // Enable the weapons when we are enabled (respawns)
    private void OnEnable() {        
        if (primaryWeapon != null) 
            primaryWeapon.gameObject.SetActive(true);
        if (secondaryWeapon != null) 
            secondaryWeapon.gameObject.SetActive(true);
    }

    // Disable the weapons and stop shooting if we are disabled (killed)
    private void OnDisable() {        
        if (primaryWeapon != null) {
            CancelInvoke("CmdFirePrimary");
            primaryWeapon.gameObject.SetActive(false);
        }            
        if (secondaryWeapon != null) {
            CancelInvoke("CmdFireSecondary");
            secondaryWeapon.gameObject.SetActive(false);
        }            
    }

    private void Update() {
        if (isServer)
            UpdateFire();

        if (isLocalPlayer) {
            if (Input.GetKeyDown(KeyCode.P))
                player.rocketWeapons.CmdFire(true);
            if (Input.GetKeyUp(KeyCode.P))
                player.rocketWeapons.CmdFire(false);
        }
    }

    // Stops any invoke repeating (autofire) of a weapon
    public void Ceasefire(Weapon.Slot slot) {
        if (slot == Weapon.Slot.Primary)
            CancelInvoke("CmdFirePrimary");
        else if (slot == Weapon.Slot.Seconday)
            CancelInvoke("CmdFireSecondary");
    }

    // Fires a weapon if pre-conditions are met. Initiates a repeting invoce call if the gun has autofire
    [Client]
    public void Fire(Weapon.Slot slot) {
        if (!isLocalPlayer)
            return;

        if (slot == Weapon.Slot.Primary && primaryWeapon != null) {
            if (primaryWeapon.fireRate <= 0f)
                CmdFirePrimary();    // Single fire
            else
                InvokeRepeating("CmdFirePrimary", 0f, 1f / primaryWeapon.fireRate);    // Autofire
        }
        else if (slot == Weapon.Slot.Seconday && secondaryWeapon != null) {
            if (secondaryWeapon.fireRate <= 0f)
                CmdFireSecondary();    // Single fire
            else
                InvokeRepeating("CmdFireSecondary", 0f, 1f / secondaryWeapon.fireRate);    // Autofire
        }
    }

    // Fire primary weapon
    [Command]
    private void CmdFirePrimary() {
        try { 
            primaryWeapon.Shoot(player);
        } catch(ShootException e) {
            Debug.Log(e.Message);
            CancelInvoke("FirePrimary");
        }
    }

    // Fire secondary weapon
    [Command]
    private void CmdFireSecondary() {
        try {
            secondaryWeapon.Shoot(player);
        }
        catch (ShootException e) {
            Debug.Log(e.Message);
            CancelInvoke("FireSecondary");
        }
    }

    // Returns the transform where weapons are put
    public Transform GetWeaponSlot() {
        return weaponSlot;
    }

    // For debug use: Toggles weapons being equipped/unequipped
    public void ToggleWeapon(WeaponInventory.Name weaponName) {
        Debug.Log("Toggle " + weaponName);
        Weapon weapon = WeaponInventory.instance.getWeapon(weaponName);
        if (weapon != null) {
            if (weapon.slot == Weapon.Slot.Primary && primaryWeapon == null)
                CmdEquipWeapon(weaponName);
            else
                CmdUnequipWeapon(Weapon.Slot.Primary);

        }
    }    

    // Equips specified weapon and spawns it using the network server with client authority. This to have the player control the weapon (and let the
    // weapon have authority to call the server directly). Also makes an rpc call to all clients so they can setup the wepon properly (layer names, parent transform etc).
    [Command]
    private void CmdEquipWeapon(WeaponInventory.Name weaponName) {
        Weapon weapon = WeaponInventory.instance.getWeapon(weaponName);
        if (weapon == null) {
            Debug.LogWarning(player.name + " could not equip weapon " + weaponName.ToString() + " as it was not found in the weapon inventory");
            return;
        }

        CmdUnequipWeapon(weapon.slot);

        Weapon _weaponIns = Instantiate(weapon, weaponSlot.position, weaponSlot.rotation);        
        NetworkServer.SpawnWithClientAuthority(_weaponIns.gameObject, player.gameObject);
        EquipWeapon(_weaponIns.gameObject);

        RpcEquipWeapon(_weaponIns.gameObject);        
        Debug.Log("Player " + player.name + " spawning " + _weaponIns.name + " with client authority");
    }

    // Sets up the weapon on all clients (layer names, parent transform etc)
    [ClientRpc]
    void RpcEquipWeapon(GameObject weapon) {
        EquipWeapon(weapon);
    }

    // Sets the rocket weapon slot as parent transform to the weapon. Also sets the weapon layer to all game objects of the weapon and assigns the weapon to the correct weapon slot
    private void EquipWeapon(GameObject weapon) {
        weapon.transform.SetParent(weaponSlot.transform);
        Util.SetLayerRecursively(weapon, LayerMask.NameToLayer(weaponLayerName));
        setWeaponSlot(weapon.GetComponent<Weapon>());
    }

    // Sets the weapon to the correct weapon slot
    private void setWeaponSlot(Weapon weapon) {
        switch (weapon.slot) {
            case Weapon.Slot.Primary:
                primaryWeapon = weapon;
                break;
            case Weapon.Slot.Seconday:
                secondaryWeapon = weapon;
                break;
            default:
                Debug.LogWarning("Invalid weaponslot " + weapon.slot + " for weapon " + weapon.displayName);
                break;
        }

    }

    // Destroys the gameobject in the specified slot
    [Command]
    private void CmdUnequipWeapon(Weapon.Slot slot) {
        Weapon current = getWeapon(slot);
        if (current != null)
            NetworkServer.Destroy(current.gameObject);
    }

    // returns the weapon in the specified weapon slot
    private Weapon getWeapon(Weapon.Slot slot) {
        if (slot == Weapon.Slot.Primary)
            return primaryWeapon;
        else if (slot == Weapon.Slot.Seconday)
            return secondaryWeapon;
        return null;
    }

    // Call the server to notify it that a shot has been fired
    [Command]
    public void CmdOnWeaponShot(Weapon.Slot slot) {
        RpcOnWeaponShot(slot);
    }

    // Calls all client to notify them that a shot has been fired
    [ClientRpc]
    private void RpcOnWeaponShot(Weapon.Slot slot) {
        Weapon weapon = getWeapon(slot);
        if (weapon != null) {
            weapon.DoShotEffects();
        }
    }

    // Call the server to notify it that a shot has been fired and a hit has been detected
    [Command]
    public void CmdOnWeaponShotAndHit(Weapon.Slot slot, Vector3 hitPosition, Vector3 hitNormal) {
        RpcOnWeaponShotAndHit(slot, hitPosition, hitNormal);
    }

    // Call all clients to notify it that a shot has been fired and a hit has been detected
    [ClientRpc]
    private void RpcOnWeaponShotAndHit(Weapon.Slot slot, Vector3 hitPosition, Vector3 hitNormal) {
        Weapon weapon = getWeapon(slot);
        if (weapon != null) {
            weapon.OnShootAndHit(hitPosition, hitNormal);
        }
    }

    // Call the server to notify it that a shot has been fired and a hit has been detected
    [Command]
    public void CmdTakeDamage(GameObject go, int damage) {
        if (go.GetComponent<Health>() != null)
            RpcTakeDamage(go, damage);
    }

    [ClientRpc]
    private void RpcTakeDamage(GameObject go, int damage) {
        // Can we damage what we hit?
        if (go != null) { 
            Health health = go.GetComponent<Health>();
            if (health != null) {
                health.TakeDamage(damage, player.name);
            }
        }
    }

    [Command]
    public void CmdSpawnProjectile(Vector3 _position, Quaternion _rotation) {
        /*    Debug.Log("Shooting the missile launcher!");

            GameObject missile;
            missilePool.InstantiateFromPool(weaponSlot.position, weaponSlot.rotation, out missile);
            if (missile == null)
                Debug.LogError("No missile could be spawned from the pool");
            else
                Debug.Log("Shooting " + missile.name);
                */
    }

    [Command]
    public void CmdFire(bool _isFiring) {
        isFiring = _isFiring;
    }

    private void UpdateFire() {
        if (!isFiring || Time.timeSinceLevelLoad - lastShotTime < fireRate) return;

        GameObject bullet;
        bulletsPool.InstantiateFromPool(transform.position + transform.up * 2, Quaternion.identity, out bullet);
        bullet.GetComponent<Bullet>().Shoot(transform.up);

        lastShotTime = Time.timeSinceLevelLoad;
    }

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    // This [Command] code is called on the Client �
    // � but it is run on the Server!
    [Command]
    public void CmdFire2() {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * 6;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}
