using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

    private int lastKills = 0;
    private int lastDeaths = 0;

    public float syncDelay = 5f;
    Player player;

    // Use this for initialization
    private void Start () {
        player = GetComponent<Player>();

        // Start a coroutine which will synd the data avery x seconds
        StartCoroutine(SyncScoreTimer());
	}

    private void OnDestroy() {
        if (player != null)
            SyncScore();
    }

    // Syncs the data and then waits for x secods
    IEnumerator SyncScoreTimer() {        
        while (true) {
            yield return new WaitForSeconds(syncDelay);
            SyncScore();            
        }
    }

    // Synd the player score to the server if the player is logged in
    private void SyncScore() {
        if (UserAccountManager.isLoggedIn)
            UserAccountManager.instance.GetData(OnDataRecieved);
    }

    // Called when we have recieved the user data from the server. Then adds the current kills/deaths and writes the data back
    void OnDataRecieved(string data) {
        // Don't sync the data if there is nothing new to sync
        if (player.kills <= lastKills && player.deaths <= lastDeaths)
            return;

        int killsSinceLastSync = player.kills - lastKills;
        int deathsSinceLastSync = player.deaths - lastDeaths;

        int totalKills = killsSinceLastSync + UserAccountDataParser.GetKills(data);
        int totalDeaths = deathsSinceLastSync + UserAccountDataParser.GetDeaths(data);
        Debug.Log("Syncing score (" + totalKills + "/" + totalDeaths + ") for player " + player.name);
        UserAccountManager.instance.SendData(UserAccountDataParser.ValuesToData(totalKills, totalDeaths));

        lastKills = player.kills;
        lastDeaths = player.deaths;
    }

}
