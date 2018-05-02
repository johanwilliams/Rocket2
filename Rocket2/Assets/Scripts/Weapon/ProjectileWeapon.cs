using UnityEngine;

public class ProjectileWeapon : Weapon {

    public Projectile projectilePrefab;

    public override void Shoot(Player shooter) {
        base.Shoot(shooter);
        Debug.Log("Shooting a projectile weapon!");

        //Projectile projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        //NetworkServer.Spawn(projectile.gameObject);

    }
}
