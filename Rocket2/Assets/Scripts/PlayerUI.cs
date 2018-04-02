using UnityEngine;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    private RectTransform fuelFill;

    private RocketEngine rocketEngine;

    public void SetRocketEngine(RocketEngine _rocketEngine) {
        rocketEngine = _rocketEngine;
    }

    private void Update() {
        SetFuelAmount(rocketEngine.GetFuelAmount());
    }

    void SetFuelAmount(float _amount) {
        fuelFill.localScale = new Vector3(_amount, 1f, 1f);
    }

}
