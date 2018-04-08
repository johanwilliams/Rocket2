using System;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreboardPrefab;

    [SerializeField]
    Transform playerScoreboardList;

    private void OnEnable() {
        Player[] players = GameManager.GetPlayers();

        // Sort the players array by most kills
        Array.Sort(players, delegate (Player player1, Player player2) {
            return player2.kills.CompareTo(player1.kills); // (player2.kills - player1.kills)
        });


        foreach (Player player in players) {
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
