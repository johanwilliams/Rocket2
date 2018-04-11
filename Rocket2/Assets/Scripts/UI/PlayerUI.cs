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
    private RocketEngine rocketEngine;

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
        rocketEngine = player.GetComponent<RocketEngine>();
    }

    private void Update() {
        SetHealthAmount(player.GetHealthPct());
        SetFuelAmount(rocketEngine.GetFuelPct());
        SetEnergyAmount(player.GetEnergyPct());

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

    void SetFuelAmount(float _amount) {
        fuelFill.localScale = new Vector3(_amount, 1f, 1f);
    }

    void SetEnergyAmount(float _amount) {
        energyFill.localScale = new Vector3(_amount, 1f, 1f);
    }

    void SetHealthAmount(float _amount) {
        healthFill.localScale = new Vector3(_amount, 1f, 1f);
    }

}
