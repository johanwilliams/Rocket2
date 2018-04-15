using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    private RectTransform healthFill;

    [SerializeField]
    private RectTransform fuelFill;

    [SerializeField]
    private RectTransform energyFill;

    private Player player;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject scoreboard;

    [SerializeField]
    Text latencyText;    

    private void Start() {
        PauseMenu.IsOn = false;
    }

    public void SetPlayer(Player _player) {
        player = _player;
    }

    private void Update() {
        UpdateHealth();
        UpdateFuel();
        UpdateEnergy();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            scoreboard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab)) {
            scoreboard.SetActive(false);
        }

        UpdateLatency();
    }

    private void UpdateLatency() {
        latencyText.text = "RTT: " + GameManager.instance.getLatency() + " ms";
    }

    public void TogglePauseMenu() {
        if (PauseMenu.IsOn)
            AudioManager.instance.Play("ButtonDown");
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;        
    }

    private void UpdateFuel() {
        FillRect(fuelFill, player.rocketEngine.GetFuelPct());
    }

    private void UpdateEnergy() {
        FillRect(energyFill, player.energy.GetEnergyPct());
    }

    private void UpdateHealth() {
        FillRect(healthFill, player.health.GetHealthPct());
    }

    void FillRect(RectTransform rect, float _amount) {
        rect.localScale = new Vector3(_amount, 1f, 1f);
    }

}
