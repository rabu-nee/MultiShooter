using UnityEngine;
using UnityEngine.AI;
using C;

public class EnemyPatrol : IFSMState<EnemyController> {
    private int currentWaypoint = 0;
    //private float closeEnoughToWaypoint = 0.5f;
    //private float speed = 8.0f;
    private NavMeshAgent agent;

	static readonly EnemyPatrol instance = new EnemyPatrol();
	public static EnemyPatrol Instance {
		get { return instance; }
	}

    public void Enter(EnemyController e) {
        agent = e.GetComponent<NavMeshAgent>();
        agent.SetDestination(e.waypoints[currentWaypoint]);
		Debug.Log( "started patrolling" );
	}

	public void Exit(EnemyController e) {
		Debug.Log("stopped patrolling");
	}
		
	public void Reason(EnemyController e) {
        
		// can we see the player? if so, we gotta chase after him!
		RaycastHit hit;
		if( Physics.Raycast( e.transform.position, e.playerTransform.position - e.transform.position, out hit ) ) {
      if (hit.transform.tag == Tags.PLAYER){
          Debug.DrawRay(e.transform.position, e.playerTransform.position - e.transform.position, Color.red);
          e.ChangeState(new EnemyChase());
      } else {
          Debug.DrawRay(e.transform.position, e.playerTransform.position - e.transform.position, Color.green);
      }
		}
	}
		
	public void Update( EnemyController e ) {
		// really simple waypoint navigation. Just finds a waypoint to run to
		//if( Vector3.Distance( e.transform.position, e.waypoints[currentWaypoint] ) < closeEnoughToWaypoint ) {
    if (agent.remainingDistance <= agent.stoppingDistance) {
      currentWaypoint = (currentWaypoint + 1) % e.waypoints.Count;

      Debug.Log("Changed Waypoint to #"+currentWaypoint);
			agent.SetDestination(e.waypoints[currentWaypoint]);
		}

		//Vector3 directionToWaypoint = e.waypoints[currentWaypoint] - e.transform.position;
		//e.transform.rotation = Quaternion.LookRotation( directionToWaypoint );
    //e.transform.position += ( directionToWaypoint.normalized * speed * Time.deltaTime );

	}
}
