using System;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    private RectTransform fuelFill;

    private RocketEngine rocketEngine;

    [SerializeField]
    GameObject pauseMenu;

    private void Start() {
        PauseMenu.IsOn = false;
    }

    public void SetRocketEngine(RocketEngine _rocketEngine) {
        rocketEngine = _rocketEngine;
    }

    private void Update() {
        SetFuelAmount(rocketEngine.GetFuelAmount());

        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu() {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;        
    }

    void SetFuelAmount(float _amount) {
        fuelFill.localScale = new Vector3(_amount, 1f, 1f);
    }

}
