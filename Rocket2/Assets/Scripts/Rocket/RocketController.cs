using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(RocketEngine))]
[RequireComponent(typeof(RocketWeapons))]
[RequireComponent(typeof(Player))]
public class RocketController : NetworkBehaviour {

    // Component caching
    private RocketEngine rocketEngine;
    private RocketWeapons rocketWeapons;
    private Player player;

    public bool lockCursor;

    // Use this for initialization
    void Start () {
        rocketEngine = GetComponent<RocketEngine>();
        rocketWeapons = GetComponent<RocketWeapons>();
        player = GetComponent<Player>();        
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Space)) {
            rocketWeapons.CmdFire2();
        }

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

        //TODO: For debug only
        if (Input.GetKeyDown(KeyCode.K)) {
            //player.rocketWeapons.CmdTakeDamage(player.gameObject, 50);
            //player.health.RpcTakeDamage(50, player.name);
        }
            

        if (Input.GetButtonDown("Fire1"))
            rocketWeapons.Fire(Weapon.Slot.Primary);
        else if (Input.GetButtonUp("Fire1"))
            rocketWeapons.Ceasefire(Weapon.Slot.Primary);

        if (Input.GetButtonDown("Fire2"))
            rocketWeapons.Fire(Weapon.Slot.Seconday);
        else if (Input.GetButtonUp("Fire2"))
            rocketWeapons.Ceasefire(Weapon.Slot.Seconday);


        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
            Debug.Log("Shift 1");
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            rocketWeapons.ToggleWeapon(WeaponInventory.Name.Lasergun);
    }
}
