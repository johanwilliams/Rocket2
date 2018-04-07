using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreboardPrefab;

    [SerializeField]
    Transform playerScoreboardList;

    private void OnEnable() {
        Player[] players = GameManager.GetPlayers();

        foreach(Player player in players) {
            GameObject itemGO = Instantiate(playerScoreboardPrefab, playerScoreboardList);
            PlayerScoreBoardItem item = itemGO.GetComponent<PlayerScoreBoardItem>();
            if (item != null) {
                item.Setup(player.username, player.kills, player.deaths);
            }
        }
    }

    private void OnDisable() {
        foreach(Transform child in playerScoreboardList) {
            Destroy(child.gameObject);
        }
    }
}
