using UnityEngine;

[RequireComponent(typeof(RocketEngine))]
public class RocketController : MonoBehaviour {

    // Component caching
    private RocketEngine rocketEngine;

	// Use this for initialization
	void Start () {
        rocketEngine = GetComponent<RocketEngine>();
		
	}
	
	// Update is called once per frame
	void Update () {

        // Get the horizontal and vertical input
        float _inputHor = Input.GetAxis("Horizontal");
        float _inputVer = Input.GetAxis("Vertical");

        // Send the horizontal/rotate input to the rocket engine
        rocketEngine.ApplyRotation(_inputHor);

        // Send the vertical/thruster input to the rocket engine
        rocketEngine.ApplyThruster(_inputVer);
	}
}
