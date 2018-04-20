using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour {

    [SerializeField] protected string displayname;
    [SerializeField] protected int damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float lifeTime;

    [SerializeField] protected GameObject hitEffect;
    private float hitEffectDuration = 3f;
    [SerializeField] protected string hitSound;

    protected Rigidbody2D rb;

    // Use this for initialization
    protected virtual void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
        if (hitEffect == null)
            Debug.LogWarning("No hit effect has been configured for " + this.GetType().Name);
        StartCoroutine(StartLifeTime());
	}

    // If a lifetime is set we will destroy the game object when this time has passed
    protected virtual IEnumerator StartLifeTime() {
        if (lifeTime > 0) {
            yield return new WaitForSeconds(lifeTime);
            OnHit(null);
        }
        yield return true;
    }

    // Move the projectile
    protected virtual void FixedUpdate() {
        // Keep the speed for the projectile
        rb.velocity = transform.up * speed;
    }
	
    void OnCollisionEnter2D(Collision2D collision) {
        OnHit(collision.gameObject);
    }

    protected virtual void OnHit(GameObject go) {
        SpawnHitEffect();
        Destroy(gameObject);
        //TODO: Add damage to what we hit
    }

    // Spawn a hit effect
    //TODO: We could perhaps send in the normal of the hit contact point(s) to be able to rotate the hit effect properly (explosions out from a wall etc)
    protected virtual void SpawnHitEffect() {
        if (hitEffect != null) {
            GameObject hitEffectInst = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(hitEffectInst, hitEffectDuration);
        }
        AudioManager.instance.Play(hitSound);

    }
}
