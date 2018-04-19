using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour {

    private enum State { Launched, Searching, Locked };

    private Rigidbody2D rb;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float searchStartTime = 1f;
    [SerializeField] private float searchRate = 2f;
    [SerializeField] private float searchRadius = 20f;
    [SerializeField] private float searchAngle = 180f;

    [SerializeField] private GameObject deathEffect;
    [SerializeField] private LayerMask mask;

    private State state;
    public float angle;

    private Transform target;
    public ParticleSystem trail;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        state = State.Launched;
        StartCoroutine(Searching());
	}

    private void Update() {
        if (target != null)
            angle = AngleToTarget(target);
    }

    // IEnumerator which searches for targets to lock onto
    private IEnumerator Searching() {
        yield return new WaitForSeconds(searchStartTime);
        state = State.Searching;
        while(isActiveAndEnabled) {
            SearchForTarget();
            yield return new WaitForSeconds(1f / searchRate);    
        }
        yield return true;
    }

    // Search for nearest rockets to lock onto
    private void SearchForTarget() {

        float currentTargetDistance = searchRadius;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);


        foreach(Collider2D hitCollider in hitColliders) {
            if (hitCollider.gameObject.GetComponent<Player>() != null) {
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);
                float angleToTarget = AngleToTarget(hitCollider.transform) * 2f;
                if (distanceToTarget < currentTargetDistance && angleToTarget <= searchAngle) {
                    state = State.Locked;
                    target =  hitCollider.transform;    
                }
            }
        }
    }

    private float AngleToTarget(Transform _target) {
        Vector3 targetDir = _target.position - transform.position;
        return Vector3.Angle(targetDir, transform.up);
    }

    // Debug drawing
    /*private void OnDrawGizmos() {
        if (state == State.Searching) {
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, searchRadius);
        }            
        else if (state == State.Locked)
            Debug.DrawLine(transform.position, target.position, Color.red);
    }*/

    // Update is called once per frame
    void FixedUpdate () {                
        //Rotate
        if (target != null) {
            Vector2 dirToTarget = (Vector2)target.position - rb.position;
            dirToTarget.Normalize();
            float rotateAmount = Vector3.Cross(dirToTarget, transform.up).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
        }

        //Move
        rb.velocity = transform.up * speed;
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        
    }

    private void SpawnDeathEffect() {
        // Spawn a deatch effect
        GameObject _deatchEffectInst = Instantiate(deathEffect, transform.position, Quaternion.identity);
        AudioManager.instance.Play("Explosion1");
        Destroy(_deatchEffectInst, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //GameManager.instance.CmdDamageGameObject(collision.gameObject, "homingmissile", 50);
        SpawnDeathEffect();
        trail.Stop();
        trail.transform.parent = null;
        Destroy(trail.gameObject, 10f);
        Destroy(gameObject);
    }
}
