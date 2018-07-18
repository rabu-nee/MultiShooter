using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using C;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : StatefulMonoBehaviour<EnemyController> {
	public Transform playerTransform;
	public List<Vector3> waypoints = new List<Vector3>();


	void Awake() {

		// fetch our waypoint positions so we have a purpose in life
    GameObject waypointRoot = GameObject.FindGameObjectWithTag( Tags.WAYPOINTS );
    Transform[] wayPoints = waypointRoot.GetComponentsInChildren<Transform>();
		// filter out the root objects position
		foreach( Transform t in wayPoints ) {
			if( !t.Equals( waypointRoot.transform ) )
				waypoints.Add( t.position );
		}

		fsm = new FSM<EnemyController>();
		fsm.Configure(this, new EnemyPatrol());
    }
}
