using UnityEngine.Networking;

public class ProjectileWeapon : Weapon {

    public Projectile projectilePrefab;

    public override void Shoot(Player shooter) {
        base.Shoot(shooter);

        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        NetworkServer.Spawn(projectile.gameObject);

    }
}
