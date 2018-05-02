using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/*
*	Author: Juan Camilo Charria
*	Website: http://www.justjuank.com
*	Description: This class represents a pool of objects, it will register the prefab in the NetworkManager at first so you don't have to manually do it.
*	Secondly it will allow you to instantiate objects of @template prefab and reuse them as necessary.
*	Inspired by: Kevin Somers' EZ Object Pools
*/

public class NHNetworkedPool : NetworkBehaviour {

    public string poolname="";
    public GameObject template;
    public int poolsize;

    public List<GameObject> ObjectList;
    protected List<GameObject> AvailableObjects;

    public bool AutoResize = true;

    GameObject ClientSpawnHandler(Vector3 position, NetworkHash128 assetId)
    {
        var go = InstantiatePrefab();
        return go;
    }

    void ClientUnSpawnHandler(GameObject spawned)
    {
        spawned.GetComponent<NHNetworkedPoolObject>().SetObjectInactive();
    }

    void Awake()
    {
        ClientScene.RegisterSpawnHandler(template.GetComponent<NetworkIdentity>().assetId, ClientSpawnHandler, ClientUnSpawnHandler);
    }

	void Start () {

        if( NetworkServer.active )
        {
            ObjectList = new List<GameObject>();
            AvailableObjects = new List<GameObject>();

            for (int i = 0; i < poolsize; i++)
            {
                ObjectList.Add(InstantiatePrefab());
                AvailableObjects.Add(ObjectList[ObjectList.Count-1]);
            }
        }

	}

    GameObject InstantiatePrefab()
    {
        GameObject obj = Instantiate(template, transform.position, Quaternion.identity) as GameObject;
        obj.transform.parent = transform;
        NHNetworkedPoolObject nhobj = obj.AddComponent<NHNetworkedPoolObject>();
        nhobj.SetObjectInactive();
        if(isServer)
            NetworkServer.Spawn(obj);

        return obj;
    }

	public bool InstantiateFromPool(Vector3 pos, Quaternion rot, out GameObject obj) {
        if (!isServer) { obj = null; return false; }

        int lastIndex = AvailableObjects.Count - 1;

        if (AvailableObjects.Count > 0)
        {
            if (AvailableObjects[lastIndex] == null)
            {
                Debug.LogError("AvailableObject is missing?");
                obj = null;
                return false;
            }

            AvailableObjects[lastIndex].transform.position = pos;
            AvailableObjects[lastIndex].transform.rotation = rot;
            AvailableObjects[lastIndex].GetComponent<NHNetworkedPoolObject>().SetObjectActive();
            obj = AvailableObjects[lastIndex];
            AvailableObjects.RemoveAt(lastIndex);
            return true;
        }

        if (AutoResize)
        {
            GameObject g = InstantiatePrefab();
            g.transform.position = pos;
            g.transform.rotation = rot;
            g.GetComponent<NHNetworkedPoolObject>().SetObjectActive();
            ObjectList.Add(g);
            obj = g;
            return true;
        }
        else
        {
            obj = null;
            return false;
        }
	}

    public void AddToAvailableObjects(GameObject obj)
    {
        if (!isServer) return;

        AvailableObjects.Add(obj);
    }
}
