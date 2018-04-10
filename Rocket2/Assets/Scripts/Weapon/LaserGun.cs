using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour, IWeapon {

    public new string name = "Laser gun";
    public float range = 100f;
    public float damage = 5f;
    private const string PLAYER_TAG = "Player";    

    [SerializeField]
    private LayerMask mask;

    public void Shoot(Player shooter, Vector2 position, Vector2 direction) {
        Debug.Log(shooter.username + " shot " + name);

        // Raycast to detect if we hit something 
        RaycastHit2D _hit = Physics2D.Raycast(position, direction, range, mask);
        Vector3 _hitPos = position + direction * range;

        Debug.DrawLine(position, position + direction * range, Color.red);


        if (_hit.collider != null) {
            if (_hit.collider.tag == PLAYER_TAG) {
                GameManager.instance.CmdDamagePlayer(_hit.collider.name, shooter.name, damage);
            }
            _hitPos = _hit.point;

            // Call the OnHit method on the server to render a hit effect
            //CmdOnHit(_hitPos, _hit.normal);
        }
    }
}
