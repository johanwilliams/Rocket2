using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(RocketEngine))]
[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(Player))]
public class RocketController : NetworkBehaviour {

    // Component caching
    private RocketEngine rocketEngine;
    private WeaponManager weaponManager;
    private Player player;

    public bool lockCursor;

    // Use this for initialization
    void Start () {
        rocketEngine = GetComponent<RocketEngine>();
        weaponManager = GetComponent<WeaponManager>();
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
