using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;
    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    private void Start() {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList() {
        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        status.text = "Loading room list...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList) {
        status.text = "";

        if (matchList == null) {
            status.text = "Couldn't get get room list";
            return;
        }        

        foreach (MatchInfoSnapshot match in matchList) {
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
            _roomListItemGO.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
            if (_roomListItem != null) {
                _roomListItem.Setup(match, JoinRoom);
            }

            roomList.Add(_roomListItemGO);            
        }

        if (roomList.Count == 0)
            status.text = "No rooms at the moment.";
    }

    private void ClearRoomList() {
        foreach (GameObject room in roomList) 
            Destroy(room);
        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot _match) {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";
    }
}
