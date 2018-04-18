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
        //EquipWeapon(primaryWeapon);
        EquipWeapon(WeaponInventory.Name.Gun);
        EquipWeapon(WeaponInventory.Name.Lasergun);
    }

    public void CmdFire(Weapon.Slot slot) {
        //TODO: Check that we can fire (energy and possibly fireRate or should this check be on the weapon itself?)

        //TODO: Add recoil or should this be on the weapon?

        if (slot == Weapon.Slot.Primary)
            primaryWeapon.Shoot(player);
        else if (slot == Weapon.Slot.Seconday)
            secondaryWeapon.Shoot(player);
    }

    /*[Command]
    void CmdFireBullet() {
        var bullet = Instantiate(bulletPrefab, weaponSlot.transform.position, weaponSlot.transform.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = weaponSlot.transform.up * 50f;
        rb.velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
        //rb.AddForce(weaponSlot.transform.up * 6, ForceMode2D.Impulse);       

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }*/

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
            weapon.OnShoot();
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

}
