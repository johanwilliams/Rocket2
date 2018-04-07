using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillfeedItem : MonoBehaviour {

    [SerializeField]
    private Text killfeedItem;

    public void Setup(string player, string source) {
        killfeedItem.text = "<b>" + source + "</b> killed " + player;
    }
}
