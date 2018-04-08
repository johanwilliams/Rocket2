using UnityEngine;

[System.Serializable]
public class RocketWeapon {

    public string name = "Laser gun";

    public int damage = 10;
    public float range = 100f;

    public float fireRate = 0f;

    public float energyCost = 10f;

    public AudioClip soundFire;

    public GameObject graphics;
}
