using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour {

    public Text usernameText;

    private void Start() {
        if (UserAccountManager.isLoggedIn)
            usernameText.text = UserAccountManager.playerUsername;
    }

    public void LogOut() {
        AudioManager.instance.Play("ButtonDown");
        if (UserAccountManager.isLoggedIn)
            UserAccountManager.instance.LogOut();
    }
}
