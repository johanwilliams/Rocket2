using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(RocketEngine))]
[RequireComponent(typeof(RocketWeaponController))]
[RequireComponent(typeof(Player))]
public class RocketController : NetworkBehaviour {

    // Component caching
    private RocketEngine rocketEngine;
    private RocketWeaponController rocketWeapons;
    private Player player;

    public bool lockCursor;

    // Use this for initialization
    void Start () {
        rocketEngine = GetComponent<RocketEngine>();
        rocketWeapons = GetComponent<RocketWeaponController>();
        player = GetComponent<Player>();        
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;
            
        if (PauseMenu.IsOn) {
            if (Cursor.lockState != CursorLockMode.None && lockCursor)
                Cursor.lockState = CursorLockMode.None;
            rocketEngine.ApplyRotation(0f);
            rocketEngine.ApplyThruster(0f);

            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked && lockCursor)
            Cursor.lockState = CursorLockMode.Locked;

        // Get the horizontal and vertical input
        float _inputHor = Input.GetAxis("Horizontal");
        float _inputVer = Input.GetAxis("Vertical");

        // Send the horizontal/rotate input to the rocket engine
        rocketEngine.ApplyRotation(_inputHor);

        // Send the vertical/thruster input to the rocket engine
        rocketEngine.ApplyThruster(_inputVer);

        if (Input.GetKeyDown(KeyCode.K)) {
            player.CmdTakeDamage(50, player.name);
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            player.AddForce(-player.transform.up * 10f);
        }

        if (Input.GetButtonDown("Fire1"))
            rocketWeapons.Fire(Weapon.Slot.Primary);
        else if (Input.GetButtonUp("Fire1"))
            rocketWeapons.Ceasefire(Weapon.Slot.Primary);

        if (Input.GetButtonDown("Fire2"))
            rocketWeapons.Fire(Weapon.Slot.Seconday);
        else if (Input.GetButtonUp("Fire2"))
            rocketWeapons.Ceasefire(Weapon.Slot.Seconday);


        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1)) {
            foreach(Player p in GameManager.GetPlayers())
                p.weaponController.ToggleWeapon(WeaponInventory.Name.Lasergun);
        }            
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            rocketWeapons.ToggleWeapon(WeaponInventory.Name.Lasergun);
    }
}
