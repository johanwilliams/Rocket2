using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : Projectile {

    private enum State { Launched, Searching, Locked };

    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float searchStartTime = 1f;
    [SerializeField] private float searchRate = 2f;
    [SerializeField] private float searchRadius = 20f;
    [SerializeField] private float searchAngle = 180f;
    [SerializeField] private ParticleSystem trail;

    private State state;
    private float angle;
    private Transform target;

    // Launches the homing missile and starts the routine to search for targets
    protected override void Start() {
        base.Start();
        state = State.Launched;
        StartCoroutine(Searching());
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

        // Search all game objects in a circle around us
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        // For each collider we found
        foreach(Collider2D hitCollider in hitColliders) {
            // Is the collider a Player we can lock onto?
            if (hitCollider.gameObject.GetComponent<Player>() != null) {
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);
                float angleToTarget = AngleToTarget(hitCollider.transform) * 2f;
                // Is the player closer than current target and within our seachangle?
                if (distanceToTarget < currentTargetDistance && angleToTarget <= searchAngle) {
                    // Lock onto the new target
                    state = State.Locked;
                    target =  hitCollider.transform;    
                }
            }
        }
    }

    // Returns the angle from the missile direction (up) to the target
    private float AngleToTarget(Transform _target) {
        Vector3 targetDir = _target.position - transform.position;
        return Vector3.Angle(targetDir, transform.up);
    }

    // Update is called once per physics frame
    protected override void FixedUpdate () {
        base.FixedUpdate();

        //Rotate
        if (target != null) {
            Vector2 dirToTarget = (Vector2)target.position - rb.position;
            dirToTarget.Normalize();
            float rotateAmount = Vector3.Cross(dirToTarget, transform.up).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
        }
	}

    // We hit something
    protected override void OnHit(GameObject go) {
        // Avoid hitting ourself during launch
        //TODO: Improve by filtering on layers or tags
        if (state != State.Launched) {
            trail.Stop();
            trail.transform.parent = null;
            Destroy(trail.gameObject, 10f);
            base.OnHit(go);
        }
    }
}
