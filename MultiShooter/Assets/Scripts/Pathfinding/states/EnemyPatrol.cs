﻿using UnityEngine;
using UnityEngine.AI;
using C;

public class EnemyPatrol : IFSMState<EnemyController> {
    private int currentWaypoint = 0;
    private NavMeshAgent agent;

    static readonly EnemyPatrol instance = new EnemyPatrol();
    public static EnemyPatrol Instance {
        get { return instance; }
    }

    public void Enter(EnemyController e) {
        agent = e.GetComponent<NavMeshAgent>();
        agent.SetDestination(e.waypoints[currentWaypoint]);
    }

    public void Exit(EnemyController e) {
    }

    public void Reason(EnemyController e) {



        // can we see the player? if so, we gotta chase after him!
        foreach (Transform transform in e.playerTransform) {


            RaycastHit hit;
            if (Physics.Raycast(e.transform.position, transform.position - e.transform.position, out hit)) {
                if (hit.transform.tag == Tags.PLAYER) {
                    Debug.DrawRay(e.transform.position, transform.position - e.transform.position, Color.red);
                    e.target = transform;
                    e.ChangeState(new EnemyChase());
                }
                else {
                    Debug.DrawRay(e.transform.position, transform.position - e.transform.position, Color.green);
                }
            }
        }
    }

    public void Update(EnemyController e) {
        // really simple waypoint navigation. Just finds a waypoint to run to
        if (agent.remainingDistance <= agent.stoppingDistance) {
            currentWaypoint = (currentWaypoint + 1) % e.waypoints.Count;
            agent.SetDestination(e.waypoints[currentWaypoint]);
        }


    }
}
