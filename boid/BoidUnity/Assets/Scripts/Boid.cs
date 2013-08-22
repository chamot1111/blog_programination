using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour {

    // make it public to be seen in the inspector
    // use getter/sstter instead of direct access in the code
    public Vector3 velocity;

    public bool IsInFieldOfView(Vector3 pos) {
        float d = Vector3.Distance( pos, this.transform.position );
        if( d < flockManager.kFOVDistance_m ) {
            float angle = Vector3.Angle(pos - this.transform.position, velocity);
            if( Mathf.Abs( angle ) < flockManager.kFOVAngle_d ) {
                return true;
            }
        }
        return false;
    }

    public Vector3 GetCollisionAvoidance() {
        Vector3 sumForce = Vector3.zero;
        int closerCount = 0;
        
        foreach ( Boid b in neighbors )
        {
            float d = Vector3.Distance( b.transform.position, this.transform.position );
            if ( d != 0.0f && d < flockManager.kMinimumSeparationDistance_m ) {
                Vector3 forceDir = ( this.transform.position - b.transform.position ).normalized;
                // repulsion force depends on proximity distance
                sumForce += forceDir / d;
                closerCount++;
            }
        }
        
        return (closerCount > 0) ? (sumForce / (float) closerCount) * flockManager.kRepulsionCoeff : Vector3.zero;
    }

    public Vector3 GetVelocityMatching() {
        Vector3 v = Vector3.zero;
        foreach ( Boid b in neighbors )
        {
            v += b.GetVelocity();
        }
        if(neighbors.Count > 0) {
            Vector3 avgVelocity = ( v / (float) neighbors.Count );
            return (avgVelocity - velocity) * flockManager.kVelocityMatchingCoeff;
        }
        return Vector3.zero;
    }

    public Vector3 GetFlockCentering() {
        Vector3 center = Vector3.zero;
        foreach ( Boid b in neighbors )
        {
            center += b.transform.position - this.transform.position;
        }
        return (neighbors.Count > 0) ? ( center / (float) neighbors.Count ) * flockManager.kFlockCenteringCoeff : Vector3.zero;
    }

    public Vector3 GetVelocity() {
        return velocity;
    }

    public void SetVelocity(Vector3 v) {
        velocity = v;
    }

    void Awake() {
        boidsInCollider = new LinkedList<Boid>();
        neighbors = new LinkedList<Boid>();
    }

    void Start() {
        flockManager = FlockManager.GetMain();
    }
    
    void Update() {
        neighbors.Clear();
        foreach(Boid b in boidsInCollider) {
            if( IsInFieldOfView(b.transform.position) ) {
                neighbors.AddFirst(b);
            }
        }

        Vector3 forceSum = Vector3.zero;
        forceSum += GetCollisionAvoidance();
        forceSum += GetVelocityMatching();
        forceSum += GetFlockCentering();
        // we add optional external forces
        forceSum += flockManager.GetExternalForce( this.transform.position );
        velocity += forceSum;
        velocity = Vector3.ClampMagnitude( velocity, flockManager.kMaxVelocity_m_s );
        if(velocity.magnitude < flockManager.kMinVelocity_m_s ) {
            velocity = velocity.normalized * flockManager.kMinVelocity_m_s;
        }

        this.transform.position += velocity * Time.deltaTime;
        Quaternion oldRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(velocity);
        // clamp the rotation to 90 degrees per second
        transform.rotation = Quaternion.RotateTowards(oldRotation, targetRotation, 90.0f * Time.deltaTime);
    }

    public void OnTriggerEnter (Collider other) {
        GameObject gameObject = other.gameObject;
        Boid b = gameObject.GetComponent<Boid>();
        if(b != null) {
            boidsInCollider.AddFirst(b);
        }
    }
        
    public void OnTriggerExit (Collider other) {
        GameObject gameObject = other.gameObject;
        Boid b = gameObject.GetComponent<Boid>();
        if(b != null) {
            boidsInCollider.Remove(b);
        }
    }

    private FlockManager flockManager;
    private LinkedList<Boid> neighbors;
    private LinkedList<Boid> boidsInCollider;
}
