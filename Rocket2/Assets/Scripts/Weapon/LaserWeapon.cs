using UnityEngine;
using UnityEngine.Networking;

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
        Debug.Log(shooter.name + " fires a laser weapon!");
        isShootingAllowed(shooter);

        lastShotTime = Time.time;
        shooter.energy.ConsumeEnergy(energyCost);

        // Raycast to detect if we hit something 
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.up, range, hitMask);
        Vector3 hitPosition = firePoint.position + firePoint.up * range;
        Debug.DrawLine(firePoint.position, firePoint.position + firePoint.up * range, Color.red);

        // Did we hit something?
        if (hit.collider != null && hit.collider.gameObject != null) {
            hitPosition = hit.point;

            // Damage the gameobject we hit if it has a Health component
            if (hit.collider.gameObject.GetComponent<Health>() != null)
                CmdTakeDamage(shooter.gameObject, hit.collider.gameObject);            
        }
        CmdDoShotAndHitEffect(hitPosition, hit.normal);

    }

    [Command]
    public void CmdDoShotAndHitEffect(Vector3 hitPosition, Vector3 hitNormal) {
        RpcDoShotAndHitEffect(hitPosition, hitNormal);
    }

    [ClientRpc]
    public void RpcDoShotAndHitEffect(Vector3 hitPosition, Vector3 hitNormal) {
        DoShotAndHitEffect(hitPosition, hitNormal);
    }

    public void DoShotAndHitEffect(Vector3 hitPosition, Vector3 hitNormal) {
        base.DoShotEffect();
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
    }

    // Call the server to notify it that a shot has been fired and a hit has been detected
    [Command]
    public void CmdTakeDamage(GameObject shooter, GameObject victim) {
        victim.GetComponent<Health>().TakeDamage(damage, shooter.GetComponent<Player>().name);
    }

}
