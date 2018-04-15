using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(RocketEngine))]
public class RocketController : NetworkBehaviour {

    // Component caching
    private RocketEngine rocketEngine;

    // Use this for initialization
    void Start () {
        rocketEngine = GetComponent<RocketEngine>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;

        if (PauseMenu.IsOn) {
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;
            rocketEngine.ApplyRotation(0f);
            rocketEngine.ApplyThruster(0f);

            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;

        // Get the horizontal and vertical input
        float _inputHor = Input.GetAxis("Horizontal");
        float _inputVer = Input.GetAxis("Vertical");

        // Send the horizontal/rotate input to the rocket engine
        rocketEngine.ApplyRotation(_inputHor);

        // Send the vertical/thruster input to the rocket engine
        rocketEngine.ApplyThruster(_inputVer);
    }
}
