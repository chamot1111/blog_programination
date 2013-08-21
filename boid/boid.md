Boids
=====

Boids is an artificial life program, developed by Craig Reynolds in 1986, which simulates the flocking behaviour of birds. \[1\]
You can visit the page of Craig Reynolds on the Boids. \[2\]

This simulation is very interesting because:
- it's one of the ancestor for the multi-agent simulation. 
- it's beautifull to look
- it's simple

The beauty of muti agent simulation
-----------------------------------

The nice think when you looks at this kind of simulation is the simplicity. Each agent has only a few simple rules. The beauty comes from the interaction of all the agents. We see emergence of very complicated behaviour from the group that can resolve some problems. In this case, ti's the mimic of bird fly.

Simulation
----------

Let's enter the Boid simulation in details. What we wants is to have a simulation of Boids as explain by Craig Reynold. The boid fligh in a sphere.

I choose to use [Unity] as programing tool. It's an heavily used multi platform game engine. The basic version is free and I will have the possibility to export an HTML version. There are mainly two languages for the scripting in [Unity]: unity script (sort of javascript) and C#. By experience, I will use C# here.

Field of view
-------------

A Boid can only see until a certain distance. Moreover it can't see in his back. So we have a field of view that is a sphere minus a cone in his back.

```
<<field-of-view>> =
	float d = Vector3.Distance( pos, this.transform.position );
	if( d < flockManager.kFOVDistance_m ) {
		float angle = Vector3.Angle(pos - this.transform.position, velocity);
		if( Mathf.Abs( angle ) < flockManager.kFOVAngle_d ) {
			return true;
		}
	}
	return false;
```

The 3 rules!
------------

There are mainly 3 rules that create the Boid behaviour. The 3 rules are:

1. Collision Avoidance: avoid collisions with nearby flockmates 
2. Velocity Matching: attempt to match velocity with nearby flockmates 
3. Flock Centering: attempt to stay close to nearby flockmates 

These tree rules need to know all the boid in the field of view. So we get them only once in an array for each frame. We will see later how we capture the neighbours and at this moment we assume `neighbours` var contains an array with current neighbours.

### 1. Collision avoidance

Each Boids try to stay at a minimum distance of all his neighbours in his field of view. Each Boid under the minimum distance `kMinimumSeparationDistance` create a repulsion force linear proportional to distance (more closer, more powerfull).

```
<<collision-avoidance>> =
	Vector3 sumForce = Vector3.zero;
	int closerCount = 0;
	
	foreach ( Boid b in neighbours )
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
```

### 2. Velocity matching

Boid tend to keep the same velocity as neighbors. We calculate the velocity average and add a fraction of the difference. If the boid was around at the same velocity as the neighbours, it will have almost any effect. The effect is proportional to the speed difference to the group average.

```
<<velocity-matching>> =
	Vector3 v = Vector3.zero;
	foreach ( Boid b in neighbours )
	{
		v += b.GetVelocity();
	}
	if(neighbours.Count > 0) {
		Vector3 avgVelocity = ( v / (float) neighbours.Count );
		return (avgVelocity - velocity) * flockManager.kVelocityMatchingCoeff;
	}
	return Vector3.zero;
```

### 3. Flock centering

Each boid tend to stay at the gravity center of the neighbours in the field of view. It will give the effect of the organizion of the flight.

```
<<flock-centering>> =
	Vector3 center = Vector3.zero;
	foreach ( Boid b in neighbours )
	{
		center += b.transform.position - this.transform.position;
	}
	return (neighbours.Count > 0) ? ( center / (float) neighbours.Count ) * flockManager.kFlockCenteringCoeff : Vector3.zero;
```

Update velocity
---------------

We first sum all the force from the 3 rules.

```
<<update-velocity>> =
	Vector3 forceSum = Vector3.zero;
	forceSum += GetCollisionAvoidance();
	forceSum += GetVelocityMatching();
	forceSum += GetFlockCentering();
	// we add optional external forces
	forceSum += flockManager.GetExternalForce( this.transform.position );
```

Then we add it to the Boid velocity.

```
<<update-velocity>> +=
	velocity += forceSum;
```

The velocity has a maximum limit.

```
<<update-velocity>> +=
	velocity = Vector3.ClampMagnitude( velocity, flockManager.kMaxVelocity_m_s );
	if(velocity.magnitude < flockManager.kMinVelocity_m_s ) {
		velocity = velocity.normalized * flockManager.kMinVelocity_m_s;
	}
```

Update position
---------------

```
<<update-position>> =
	this.transform.position += velocity * Time.deltaTime;
```

As the velocity vector change we must update the rotation of the model. The Boid model look a the velocity vector head.

```
<<update-position>> += 
	Quaternion oldRotation = transform.rotation;
	Quaternion targetRotation = Quaternion.LookRotation(velocity);
	// clamp the rotation to 90 degrees per second
	transform.rotation = Quaternion.RotateTowards(oldRotation, targetRotation, 90.0f * Time.deltaTime);
```

Avoid obstacle
--------------

To keep it simple we use only a confinement force applied by the `FlockManager`. When boids are near the border, a force toward center of the sphere is applied.

```
	<<force-confinement>> =
		float dist = pos.magnitude;
		if( dist > kSphereRadius * 0.8f ) {
			return -pos.normalized * kSphereConfinement;
		}
		return Vector3.zero;
```

Capture neighbours
------------------

For this feature we could use the FlockManager to retrieve neighbours. But it more funny and maintanable if the boid are independent from the FlockManager. The FlockManager just create them and maintain constant. The drawback of this method is that we can't remove a Boid from the scene, because reference are potentially in all other Boids.


We will use a feature from the physics engine called `sphere collider`. When you use a collider component on an object, every rigidbody object that enter, exit or stay in the collider area will trigger event on the monobehviour script. We will use `OnTriggerEnter` and ÒnTriggerExit`. We will keep up to date a list will all the boid in the collider.

```
<<collider-events>> =
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
```

Then each frame update we filter boids that are in the field of view.

```
	<<filter-fov>> =
		neighbours.Clear();
		foreach(Boid b in boidsInCollider) {
			if( IsInFieldOfView(b.transform.position) ) {
				neighbours.AddFirst(b);
			}
		}
```

Appendix
=======

Unity stuff
-----------

```
<<./Boid.cs>> = 
	<<::Boid.cs>>
```

```
<<./FlockManager.cs>> = 
	<<::FlockManager.cs>>
```

References
----------

\[1\]: [http://en.wikipedia.org/wiki/Boids]()

\[2\]: [Craig Reynolds Boid page](http://www.red3d.com/cwr/boids/)

[Unity]: [http://unity3d.com/]()

