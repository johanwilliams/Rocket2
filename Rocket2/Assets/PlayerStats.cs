using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    public Text killCount;
    public Text deatchCount;

    // Use this for initialization
    void Start () {
        if (UserAccountManager.isLoggedIn)
            UserAccountManager.instance.GetData(OnReceivedData);	
	}
	
    void OnReceivedData(string data) {
        killCount.text = AccountDataTranslator.GetKills(data) + " kills";
        deatchCount.text = AccountDataTranslator.GetDeaths(data) + " deaths";
    }
	
}
