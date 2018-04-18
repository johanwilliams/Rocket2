using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : Weapon {

    //public new string name = "Laser gun";
    //public float range = 100f;
    //public int damage = 5;
    //private const string PLAYER_TAG = "Player";    

    [SerializeField]
    private LayerMask mask;

    public GameObject laserTrailEffectPrefab;
    public GameObject hitEffectPrefab;

    protected override void Start() {
        base.Start();
        if (laserTrailEffectPrefab == null)
            Debug.LogWarning("No laser trail prefab found on weapon " + this.GetType().Name);
        if (hitEffectPrefab == null)
            Debug.LogWarning("No hit effect prefab found on weapon " + this.GetType().Name);
    }

    public override void Shoot(Player shooter) {                
        // Raycast to detect if we hit something 
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.up, range, mask);
        Vector3 hitPosition = firePoint.position + firePoint.up * range;
        Debug.DrawLine(firePoint.position, firePoint.position + firePoint.up * range, Color.red);

        // Did we hit something?
        if (hit.collider != null) {
            Health health = hit.collider.GetComponent<Health>();
            if (health != null) {
                GameManager.instance.CmdDamagePlayer(hit.collider.name, shooter.name, damage);
            }
            hitPosition = hit.point;            
        }
        shooter.weaponManager.CmdOnWeaponShotAndHit(slot, hitPosition, hit.normal);
    }

    public override void OnShootAndHit(Vector3 hitPosition, Vector3 hitNormal) {
        base.OnShootAndHit(hitPosition, hitNormal);
        RenderTrail(hitPosition);
        DoHit(hitPosition, hitNormal);
    }

    // Renders the laser wepon trail
    private void RenderTrail(Vector3 hitPosition) {
        if (laserTrailEffectPrefab != null) {
            GameObject trailClone = Instantiate(laserTrailEffectPrefab, firePoint.position, firePoint.rotation);
            LineRenderer lr = trailClone.GetComponent<LineRenderer>();
            if (lr != null) {
                lr.SetPosition(0, firePoint.position);
                lr.SetPosition(1, hitPosition);
            }
            Destroy(trailClone, 0.04f);
        }
    }

    private void DoHit(Vector3 hitPosition, Vector3 hitNormal) {
        if (hitEffectPrefab != null && hitNormal != Vector3.zero) {
            //TODO: Object pooling could come in useful here if we do a lot of instantiating
            GameObject _hitEffect = Instantiate(hitEffectPrefab, hitPosition, Quaternion.LookRotation(hitNormal));
            Destroy(_hitEffect, 2f);
        }
    }
}
