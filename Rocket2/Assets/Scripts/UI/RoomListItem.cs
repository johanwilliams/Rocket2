using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour {

    public delegate void JoinRoomDelegate(MatchInfoSnapshot _matchInfo);
    private JoinRoomDelegate joinRoomCallback;

    [SerializeField]
    private Text roomNameText;

    private MatchInfoSnapshot matchInfo;

    public void Setup(MatchInfoSnapshot _matchInfo, JoinRoomDelegate _joinRoomCallback) {
        matchInfo = _matchInfo;
        joinRoomCallback = _joinRoomCallback;
        roomNameText.text = matchInfo.name + " (" + matchInfo.currentSize + "/" + matchInfo.maxSize + ")";
    }

    public void JoinRoom() {
        joinRoomCallback.Invoke(matchInfo);
    }

}
