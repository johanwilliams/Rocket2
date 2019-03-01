using UnityEngine;

public class MissileLauncher : ProjectileWeapon {

    NHNetworkedPool missilePool;

    protected override void Start() {
        base.Start();
        displayName = "a missile launcher";

        missilePool = FindObjectOfType<NHNetworkedPool>();
        if (missilePool == null)
            Debug.LogError("No missile pool found");
    }

    public override void Shoot(Player shooter) {
        base.Shoot(shooter);

        //shooter.weaponController.CmdSpawnProjectile(firePoint.position, firePoint.rotation);

        //Projectile projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        //NetworkServer.Spawn(projectile.gameObject);

    }
}
