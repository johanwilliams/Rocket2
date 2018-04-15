using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Weapon : NetworkBehaviour {

    public int range;
    public int speed;

    public abstract void Shoot(Vector3 position, Quaternion rotation, Vector3 direction);

}
