using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Singelton(ish) pattern to make sure we only have one GameManager
    public static GameManager instance;

    public MatchSettings matchSettings;

    public delegate void OnPlayerKilledCallback(string player, string source);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    private void Awake() {
        if (instance != null) {
            Debug.LogError("More than one GameManager in scene.");
        }
        else {
            instance = this;
        }
        
    }

    #region Player tracking

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    // Register a player i.e. add the player from our players dictionary
    public static void RegisterPlayer(string _netID, Player _player) {        
        string _playerID = PLAYER_ID_PREFIX + _netID;
        Debug.Log("Registering player " + _player.name + " with id " + _netID + " as " + _playerID);
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    // Unregister a player i.e. remove the player from our players dictionary
    internal static void UnregisterPlayer(string _playerID) {
        Debug.Log("Unregistering player with id " + _playerID);
        players.Remove(_playerID);
    }

    // Returns the player with the specified player id
    public static Player GetPlayer(string _playerID) {
        return players[_playerID];
    }

    // Resturn all players in the dictionary
    public static Player[] GetPlayers() {
        return players.Values.ToArray();
    }

    /*private void OnGUI() {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        foreach (string _playerID in players.Keys) {
            GUILayout.Label(_playerID + " - " + players[_playerID].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }*/

    #endregion
}
