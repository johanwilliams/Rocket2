using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

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
        if (player.kills == 0 && player.deaths == 0)
            return;

        int kills = player.kills + UserAccountDataParser.GetKills(data);
        int deaths = player.deaths + UserAccountDataParser.GetDeaths(data);
        Debug.Log("Syncing score (" + kills + "/" + deaths + ") for player " + player.name);
        UserAccountManager.instance.SendData(UserAccountDataParser.ValuesToData(kills, deaths));

        // Reset the kills/deaths on the player since we have written (saved) the data
        player.kills = 0;
        player.deaths = 0;
    }

}
