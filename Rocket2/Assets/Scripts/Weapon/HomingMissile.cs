using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour {

    private enum State { Launched, Searching, Locked };

    private Rigidbody2D rb;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float searchStartTime = 1f;
    [SerializeField] private float searchRate = 2f;
    [SerializeField] private float searchRadius = 20f;

    private State state;

    public Transform target;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        state = State.Launched;
        StartCoroutine(Searching());
	}

    // IEnumerator which searches for targets to lock onto
    private IEnumerator Searching() {
        Debug.Log("Launched");
        yield return new WaitForSeconds(searchStartTime);
        state = State.Searching;
        while(target != null) {
            Debug.Log("Searching");
            SearchForTarget();
            yield return new WaitForSeconds(1f / searchRate);    
        }
        yield return true;
    }

    // Search for nearest rockets to lock onto
    private void SearchForTarget() {

        float currentTargetDistance = searchRadius;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);

        foreach(Collider hitCollider in hitColliders) {
            if (hitCollider.gameObject.GetComponent<Player>() != null) {
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distanceToTarget < currentTargetDistance) {
                    state = State.Locked;
                    target =  hitCollider.transform;    
                }
            }
        }
    }

    // Debug drawing
    private void OnDrawGizmos() {
        if (state == State.Searching) {
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, searchRadius);
        }            
        else if (state == State.Locked)
            Debug.DrawLine(transform.position, target.position, Color.red);
    }

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

    //TODO: Change to collission detection so we know what we hit (and can damage it)
    void OnTriggerEnter2D() {
        //TODO: Put a particle effect here
        Destroy(gameObject);
    }
}
