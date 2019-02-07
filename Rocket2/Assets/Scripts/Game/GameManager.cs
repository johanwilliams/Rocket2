using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

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

    private void Start() {
        // Kill the theme music
        AudioManager.instance.Stop("Theme");
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
        if (players.ContainsKey(_playerID))
            return players[_playerID];
        return null;
        
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

    // Command (server side method) which takes care of a player taking damage from another player
    [Command]
    public void CmdDamagePlayer(string _playerID, string _sourcePlayerID, int _damage) {

        // Get the player takning damage
        Debug.Log("GameManager.cs: " + _playerID);
        Player _player = GetPlayer(_playerID);
        //_player.RpcTakeDamage(_damage, _sourcePlayerID);
        _player.health.RpcTakeDamage(_damage, _sourcePlayerID);
    }

    [Command]
    public void CmdDamageGameObject(GameObject _gameObject, string _sourcePlayerID, int _damage) {

        Health health = _gameObject.GetComponent<Health>();
        if (health != null) {
            health.RpcTakeDamage(_damage, _sourcePlayerID);
        }
    }

    public int getLatency() {
        return NetworkManager.singleton.client.GetRTT();
    } 


}
