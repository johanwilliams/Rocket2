using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string _netID, Player _player) {        
        string _playerID = PLAYER_ID_PREFIX + _netID;
        Debug.Log("Registering player " + _player.name + " with id " + _netID + " as " + _playerID);
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    internal static void UnregisterPlayer(string _playerID) {
        Debug.Log("Unregistering player with id " + _playerID);
        players.Remove(_playerID);
    }

    public static Player GetPlayer(string _playerID) {
        return players[_playerID];
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
}
