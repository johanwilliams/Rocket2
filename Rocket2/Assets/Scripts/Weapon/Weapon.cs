using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon {

    void Shoot(Player shooter, Vector2 position, Vector2 direction);

}
