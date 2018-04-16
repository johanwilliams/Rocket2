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
    }

    [Command]
    public void CmdFirePrimaryWeapon() {
        primaryWeapon.Shoot(player, weaponSlot.transform.position, weaponSlot.transform.rotation, weaponSlot.transform.up);
    }

    [Command]
    public void CmdFireSecondaryWeapon() {
        secondaryWeapon.Shoot(player, weaponSlot.transform.position, weaponSlot.transform.rotation, weaponSlot.transform.up);
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

}
