using System;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    private RectTransform healthFill;

    [SerializeField]
    private RectTransform fuelFill;

    private Player player;
    private RocketEngine rocketEngine;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject scoreboard;

    private void Start() {
        PauseMenu.IsOn = false;
    }

    public void SetPlayer(Player _player) {
        player = _player;
        rocketEngine = player.GetComponent<RocketEngine>();
    }

    private void Update() {
        SetHealthAmount(player.GetHealthPct());
        SetFuelAmount(rocketEngine.GetFuelAmount());        

        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            scoreboard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab)) {
            scoreboard.SetActive(false);
        }
    }

    public void TogglePauseMenu() {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;        
    }

    void SetFuelAmount(float _amount) {
        fuelFill.localScale = new Vector3(_amount, 1f, 1f);
    }

    void SetHealthAmount(float _amount) {
        healthFill.localScale = new Vector3(_amount, 1f, 1f);
    }

}
