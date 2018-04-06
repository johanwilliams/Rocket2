using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private int joinTimeout = 10;

    [SerializeField]
    private GameObject roomListItemPrefab;
    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    private void Start() {
        // Cache the network manager
        networkManager = NetworkManager.singleton;
        StartMatchMaker();
        RefreshRoomList();
    }

    private void StartMatchMaker() {
        if (networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }
    }

    public void RefreshRoomList() {
        ClearRoomList();
        StartMatchMaker();
        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        status.text = "Loading room list...";
    }

    // Callback method which lists all available games in the join game scroll list
    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList) {
        status.text = "";

        if (!success || matchList == null) {
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
        StartCoroutine(WaitForJoin());
    }

    // Subroutine that waits for a specified amounts of seconds to join and if unsuccessful refreshes the room list
    IEnumerator WaitForJoin() {
        ClearRoomList();        

        int countdown = joinTimeout;
        while (countdown > 0) {
            status.text = "Joining... (" + countdown + ")";
            yield return new WaitForSeconds(1);
            countdown--;
        }

        //Failed to connect
        status.text = "Failed to connect";
        yield return new WaitForSeconds(1);
        
        MatchInfo matchInfo = networkManager.matchInfo;
        if (matchInfo != null) { 
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
        }

        RefreshRoomList();
    }
}
