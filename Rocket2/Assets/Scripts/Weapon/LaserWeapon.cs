using UnityEngine;

public class LaserWeapon : Weapon {

    [SerializeField] protected int range;
    [SerializeField] protected int damage;

    [SerializeField] protected LayerMask hitMask;

    [SerializeField] protected  GameObject laserTrailEffectPrefab;
    private float trailDestroyTime = 0.04f;
    [SerializeField] protected  GameObject hitEffectPrefab;
    private float hitDestroyTime = 2f;

    protected override void Start() {
        base.Start();
        if (laserTrailEffectPrefab == null)
            Debug.LogWarning("No laser trail prefab found on weapon " + this.GetType().Name);
        if (hitEffectPrefab == null)
            Debug.LogWarning("No hit effect prefab found on weapon " + this.GetType().Name);
    }

    public override void Shoot(Player shooter) {
        base.Shoot(shooter);

        // Raycast to detect if we hit something 
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.up, range, hitMask);
        Vector3 hitPosition = firePoint.position + firePoint.up * range;
        Debug.DrawLine(firePoint.position, firePoint.position + firePoint.up * range, Color.red);

        // Did we hit something?
        if (hit.collider != null && hit.collider.gameObject != null) {
            shooter.rocketWeapons.CmdTakeDamage(hit.collider.gameObject, damage);
            hitPosition = hit.point;
        }
        shooter.rocketWeapons.CmdOnWeaponShotAndHit(slot, hitPosition, hit.normal);
    }

    // Override and do nothing as we want to take control of when we show our shot effects to show them all at one (muzzleflash sound, trail & hit)
    protected override void OnWeaponShot(Player shooter) {
        return;
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
            Destroy(trailClone, trailDestroyTime);
        }
    }

    private void DoHit(Vector3 hitPosition, Vector3 hitNormal) {
        if (hitEffectPrefab != null && hitNormal != Vector3.zero) {
            //TODO: Object pooling could come in useful here if we do a lot of instantiating
            GameObject _hitEffect = Instantiate(hitEffectPrefab, hitPosition, Quaternion.LookRotation(hitNormal));
            Destroy(_hitEffect, hitDestroyTime);
        }
    }}
