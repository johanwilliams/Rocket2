using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponSlot;

    [SerializeField]
    private RocketWeapon primaryWeapon;

    private RocketWeapon currentWeapon;

    private void Start() {
        // Use this for serialization
        if (weaponSlot == null) {
            Debug.LogError("WeaponManager: No reference to weaponSlot transform");
            this.enabled = false;
        }

        EquipWeapon(primaryWeapon);    
    }

    public RocketWeapon GetCurrentWeapon() {
        return currentWeapon;
    }

    public Transform GetWeaponSlot() {
        return weaponSlot;
    }

    private void EquipWeapon(RocketWeapon _weapon) {
        currentWeapon = _weapon;

        if (_weapon.graphics != null) { 
            GameObject _weaponIns = Instantiate(_weapon.graphics, weaponSlot.position, weaponSlot.rotation);
            _weaponIns.transform.SetParent(weaponSlot);
            if (isLocalPlayer) {
                _weaponIns.layer = LayerMask.NameToLayer(weaponLayerName);
            }
        }
    }

}
