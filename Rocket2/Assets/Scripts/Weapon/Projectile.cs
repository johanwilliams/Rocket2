using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : NetworkBehaviour {

    [SerializeField] protected string displayname;
    [SerializeField] protected int damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float lifeTime;

    [SerializeField] protected GameObject hitEffect;
    private float hitEffectDuration = 3f;
    [SerializeField] protected string hitSound;

    protected Vector3 direction;

    protected Rigidbody2D rb;

    // Use this for initialization
    protected virtual void Awake () {
        rb = GetComponent<Rigidbody2D>();        
        if (hitEffect == null)
            Debug.LogWarning("No hit effect has been configured for " + this.GetType().Name);        
	}

    protected virtual void OnEnable() {
        //rb.velocity = transform.up * speed;
        //StartCoroutine(StartLifeTime());
    }

    protected virtual void OnDisable() {
        if (!isServer) return;

        rb.velocity = Vector2.zero;
        StopAllCoroutines();
    }

    public virtual void Shoot(Vector3 _direction) {
        direction = _direction;
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
        if (!isServer) return;
        // Keep the speed for the projectile
        rb.velocity = direction * speed;
    }
	
    void OnCollisionEnter2D(Collision2D collision) {
        if (!isServer) return;
        OnHit(collision.gameObject);
    }

    protected virtual void OnHit(GameObject go) {
        SpawnHitEffect();
        //Destroy(gameObject);
        SendMessage("SetObjectInactive", SendMessageOptions.DontRequireReceiver);
        if (go != null) { 
            Health health = go.GetComponent<Health>();
            if (health != null)
                health.RpcTakeDamage(damage, displayname);
        }
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
