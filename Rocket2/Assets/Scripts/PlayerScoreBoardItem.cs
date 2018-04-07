using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreBoardItem : MonoBehaviour {

    [SerializeField]
    Text usernameText;

    [SerializeField]
    Text killsNumberText;

    [SerializeField]
    Text deathsNumberText;

    public void Setup(string username, int kills, int deaths) {
        usernameText.text = username;
        killsNumberText.text = kills.ToString();
        deathsNumberText.text = deaths.ToString();
    }
}
