using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    //    public GameObject bulletPrefab;
    private Player player;

    [SerializeField]
    private Transform weaponSlot;

    [SerializeField]
    private Weapon primaryWeapon;

    [SerializeField]
    private Weapon secondaryWeapon;

    private RocketWeapon currentWeapon;
    private WeaponGraphics currentWeaponGraphics;

    private void Start() {
        player = GetComponent<Player>();
        // Use this for serialization
        if (weaponSlot == null) {
            Debug.LogError("WeaponManager: No reference to weaponSlot transform");
            this.enabled = false;
        }
        EquipWeapon(WeaponInventory.Name.Gun);
        EquipWeapon(WeaponInventory.Name.Lasergun);
    }

    private void OnEnable() {        
        if (primaryWeapon != null) 
            primaryWeapon.gameObject.SetActive(true);
        if (secondaryWeapon != null) 
            secondaryWeapon.gameObject.SetActive(true);
    }

    private void OnDisable() {        
        if (primaryWeapon != null) {
            CancelInvoke("FirePrimary");
            primaryWeapon.gameObject.SetActive(false);
        }            
        if (secondaryWeapon != null) {
            CancelInvoke("FireSecondary");
            secondaryWeapon.gameObject.SetActive(false);
        }            
    }

    // Stops any invoke repeating (autofire) of a weapon
    public void Ceasefire(Weapon.Slot slot) {
        Debug.Log("Ceasefire " + slot);
        if (slot == Weapon.Slot.Primary)
            CancelInvoke("FirePrimary");
        else if (slot == Weapon.Slot.Seconday)
            CancelInvoke("FireSecondary");
    }

    // Fires a weapon if pre-conditions are met. Initiates a repeting invoce call if the gun has autofire
    public void Fire(Weapon.Slot slot) {
        if (slot == Weapon.Slot.Primary && primaryWeapon != null && primaryWeapon.isShootingAllowed(player)) {
            if (primaryWeapon.fireRate <= 0f)
                FirePrimary();    // Single fire
            else
                InvokeRepeating("FirePrimary", 0f, 1f / primaryWeapon.fireRate);    // Autofire
        }
        else if (slot == Weapon.Slot.Seconday && secondaryWeapon != null && secondaryWeapon.isShootingAllowed(player)) {
            if (secondaryWeapon.fireRate <= 0f)
                FireSecondary();    // Single fire
            else
                InvokeRepeating("FireSecondary", 0f, 1f / secondaryWeapon.fireRate);    // Autofire
        }
    }

    private void FirePrimary() {
        primaryWeapon.Shoot(player);
    }

    private void FireSecondary() {
        secondaryWeapon.Shoot(player);
    }

    public RocketWeapon GetCurrentWeapon() {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentWeaponGraphics() {
        return currentWeaponGraphics;
    }

    public Transform GetWeaponSlot() {
        return weaponSlot;
    }

    private void EquipWeapon(WeaponInventory.Name name) {
        Weapon weapon = WeaponInventory.instance.getWeapon(name);
        if (weapon == null) {
            Debug.LogWarning("Could not equip weapon " + name.ToString() + " as it was not found in the weapon inventory");
            return;
        }

        Weapon _weaponIns = Instantiate(weapon, weaponSlot.position, weaponSlot.rotation);
        _weaponIns.transform.SetParent(weaponSlot);

        // Equip the weapon to the correct weapon slot
        if (weapon.slot == Weapon.Slot.Primary)
            primaryWeapon = _weaponIns;
        else if (weapon.slot == Weapon.Slot.Seconday)
            secondaryWeapon = _weaponIns;

        Debug.Log("Equipped wepon " + name + " in " + weapon.slot + " weapon slot");
    }

    //TODO: Remove when the other equipweapon is working
        private void EquipWeapon(RocketWeapon _weapon) {
        currentWeapon = _weapon;

        if (_weapon.graphics != null) { 
            GameObject _weaponIns = Instantiate(_weapon.graphics, weaponSlot.position, weaponSlot.rotation);
            _weaponIns.transform.SetParent(weaponSlot);

            currentWeaponGraphics = _weaponIns.GetComponent<WeaponGraphics>();
            if (currentWeaponGraphics == null) {
                Debug.LogError("No WeaponGraphics on the weapon " + _weaponIns.name);
            }

            if (isLocalPlayer) {
                Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
            }
        }
    }

    private Weapon getWeapon(Weapon.Slot slot) {
        if (slot == Weapon.Slot.Primary)
            return primaryWeapon;
        else if (slot == Weapon.Slot.Seconday)
            return secondaryWeapon;
        return null;
    }

    // Called on the server when a weapon is beiing shot
    [Command]
    public void CmdOnWeaponShot(Weapon.Slot slot) {
        RpcOnWeaponShot(slot);
    }

    // Called on all clients when we weapon is being shot
    [ClientRpc]
    private void RpcOnWeaponShot(Weapon.Slot slot) {
        Weapon weapon = getWeapon(slot);
        if (weapon != null) {
            weapon.DoShotEffects();
        }
    }

    // Called on the server when a weapon is beiing shot
    [Command]
    public void CmdOnWeaponShotAndHit(Weapon.Slot slot, Vector3 hitPosition, Vector3 hitNormal) {
        RpcOnWeaponShotAndHit(slot, hitPosition, hitNormal);
    }

    // Called on all clients when we weapon is being shot
    [ClientRpc]
    private void RpcOnWeaponShotAndHit(Weapon.Slot slot, Vector3 hitPosition, Vector3 hitNormal) {
        Weapon weapon = getWeapon(slot);
        if (weapon != null) {
            weapon.OnShootAndHit(hitPosition, hitNormal);
        }
    }

    [Command]
    public void CmdDamageGameObject(GameObject _gameObject, string _sourcePlayerID, int _damage) {
        Health health = _gameObject.GetComponent<Health>();
        if (health != null) {
            health.RpcTakeDamage(_damage, _sourcePlayerID);
        }
    }

}
