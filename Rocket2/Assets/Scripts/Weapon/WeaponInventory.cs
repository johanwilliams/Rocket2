using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour {

    public enum Name { Gun, Lasergun, HomingMissile, Gravityhole, Shotgun };    

    public WeaponInventoryItem[] inventory;

    // Singelton(ish) pattern to make sure we only have one GameManager
    public static WeaponInventory instance;

    private void Awake() {
        if (instance != null) {
            Debug.LogError("More than one GameManager in scene.");
        }
        else {
            instance = this;
        }
    }

    public Weapon getWeapon(Name weaponName) {
        WeaponInventoryItem weaponItem = Array.Find(inventory, weapon => weapon.name == weaponName);
        if (weaponItem == null) {
            Debug.LogWarning("Weapon '" + weaponName.ToString() + "' could not be found in the weaponinventory");
            return null;
        }
        return weaponItem.weapon;
    }
}
