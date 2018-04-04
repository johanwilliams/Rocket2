using System;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    private RectTransform fuelFill;

    private Player player;
    private RocketEngine rocketEngine;

    [SerializeField]
    GameObject pauseMenu;

    private void Start() {
        PauseMenu.IsOn = false;
    }

    public void SetPlayer(Player _player) {
        player = _player;
        rocketEngine = player.GetComponent<RocketEngine>();
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
