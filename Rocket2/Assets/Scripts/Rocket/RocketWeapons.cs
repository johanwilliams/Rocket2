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

    private void Start() {
        player = GetComponent<Player>();
        // Use this for serialization
        if (weaponSlot == null) {
            Debug.LogError("RocketWeapons: No reference to weaponSlot transform");
            this.enabled = false;
        }
        EquipWeapon(defaultWeapon);
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

    // Fire primary weapon
    private void FirePrimary() {
        primaryWeapon.Shoot(player);
    }

    // Fire secondary weapon
    private void FireSecondary() {
        secondaryWeapon.Shoot(player);
    }

    // Returns the transform where weapons are put
    public Transform GetWeaponSlot() {
        return weaponSlot;
    }

    // Equips a weapon and removes any weapon currently in that slot
    private void EquipWeapon(WeaponInventory.Name name) {
        Weapon weapon = WeaponInventory.instance.getWeapon(name);
        if (weapon == null) {
            Debug.LogWarning(player.name + " could not equip weapon " + name.ToString() + " as it was not found in the weapon inventory");
            return;
        }

        Weapon _weaponIns = Instantiate(weapon, weaponSlot.position, weaponSlot.rotation);
        _weaponIns.transform.SetParent(weaponSlot);

        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns.gameObject, LayerMask.NameToLayer(weaponLayerName));

        // Equip the weapon to the correct weapon slot
        if (weapon.slot == Weapon.Slot.Primary) {
            if (primaryWeapon != null)
                Destroy(primaryWeapon);
            primaryWeapon = _weaponIns;
        }
        else if (weapon.slot == Weapon.Slot.Seconday) {
            if (secondaryWeapon != null)
                Destroy(secondaryWeapon);
            secondaryWeapon = _weaponIns;
        }

        Debug.Log(player.name + " equipped weapon " + name + " in " + weapon.slot + " weapon slot");
    }

    // Unequips a weapon and equips our default weapon (if the unequipped weapon had that slot)
    private void UnequipWeapon(Weapon.Slot slot) {
        Weapon current = getWeapon(slot);
        if (current.slot == WeaponInventory.instance.getWeapon(defaultWeapon).slot)
            EquipWeapon(defaultWeapon);
        else
            Destroy(current);
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

    // Call the server to notify it a gameobject taken damage (if it has the health component)
    [Command]
    public void CmdDamageGameObject(GameObject _gameObject, string _sourcePlayerID, int _damage) {
        Health health = _gameObject.GetComponent<Health>();
        if (health != null) {
            health.RpcTakeDamage(_damage, _sourcePlayerID);
        }
    }

}
