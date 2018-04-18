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

    private void Update() {
        if (PauseMenu.IsOn || !isLocalPlayer)
            return;

        return;

        //TODO: For debug only
        if (Input.GetKeyDown(KeyCode.K))
            weaponManager.CmdDamageGameObject(player.gameObject, player.name, 50);

        if (Input.GetButtonDown("Fire1"))
            weaponManager.Fire(Weapon.Slot.Primary);
        else if (Input.GetButtonUp("Fire1"))
            weaponManager.Ceasefire(Weapon.Slot.Primary);

        if (Input.GetButtonDown("Fire2"))
            weaponManager.Fire(Weapon.Slot.Seconday);
        else if (Input.GetButtonUp("Fire2"))
            weaponManager.Ceasefire(Weapon.Slot.Seconday);
    }

}
