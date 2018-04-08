using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killfeed : MonoBehaviour {

    [SerializeField]
    GameObject killdfeedItemPrefab;

    public float killfeedDelay = 4f;

	// Use this for initialization
	void Start () {
        GameManager.instance.onPlayerKilledCallback += OnKill;		
	}

    public void OnKill(string player, string source) {
        GameObject go = Instantiate(killdfeedItemPrefab, this.transform);
        go.GetComponent<KillfeedItem>().Setup(player, source);

        Destroy(go, killfeedDelay);
    }
}
