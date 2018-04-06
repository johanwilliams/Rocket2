using System;
using UnityEngine;

//TODO: Reformat the data into json
public class AccountDataTranslator : MonoBehaviour {

    private static string KEY_KILLS = "[KILLS]";
    private static string KEY_DEATHS = "[DEATHS]";

    // Returns the number of kills from the user account data
    public static int GetKills(string data) {
        return int.Parse(GetDataByKey(data, KEY_KILLS));
    }

    // Returns the number of deaths from the user account data
    public static int GetDeaths(string data) {
        return int.Parse(GetDataByKey(data, KEY_DEATHS));
    }

    // Returns the value of the specified key from the data
    private static string GetDataByKey(string data, string key) {
        string[] dataNodes = data.Split('/');
        foreach (string dataNode in dataNodes) {
            if (dataNode.StartsWith(key)) {
                return dataNode.Substring(key.Length);
            }
        }
        Debug.LogError(key + " not found in data: " + data);
        return "";
    }
}
