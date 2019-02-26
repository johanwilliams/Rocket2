using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    private RectTransform healthFill;
    private float health;

    [SerializeField]
    private RectTransform fuelFill;
    private float fuel;

    [SerializeField]
    private RectTransform energyFill;
    private float energy;

    [SerializeField]
    [Range(0.01f, 1f)]
    private float fillSmoothness = 0.05f;

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
        fuel = Mathf.Lerp(fuel, player.rocketEngine.GetFuelPct(), fillSmoothness);
        FillRect(fuelFill, fuel);
    }    

    private void UpdateEnergy() {
        energy = Mathf.Lerp(energy, player.energy.GetEnergyPct(), fillSmoothness);
        FillRect(energyFill, energy);
    }

    private void UpdateHealth() {
        health = Mathf.Lerp(health, player.health.GetHealthPct(), fillSmoothness);
        FillRect(healthFill, health);
    }

    void FillRect(RectTransform rect, float _amount) {
        rect.localScale = new Vector3(_amount, 1f, 1f);
    }

}
