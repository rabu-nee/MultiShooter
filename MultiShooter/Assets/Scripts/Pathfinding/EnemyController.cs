using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using C;
using GameEvents;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : StatefulMonoBehaviour<EnemyController>, IGameEventListener<GameEvent_RespawnNow> {
    public Transform[] playerTransform;
    public List<Vector3> waypoints = new List<Vector3>();
    public Enemy enemyScript;
    public Transform target;


    /*
    void Awake() {
        // fetch our waypoint positions so we have a purpose in life
        GameObject waypointRoot = GameObject.FindGameObjectWithTag(Tags.WAYPOINTS);
        Transform[] wayPoints = waypointRoot.GetComponentsInChildren<Transform>();

        playerTransform = GameObject.FindGameObjectWithTag(Tags.PLAYER).GetComponent<Transform>();
        /*
        GameObject[] players = GameObject.FindGameObjectsWithTag(Tags.PLAYER);
        for (int i = 0; i< players.Length; i++) {
            playerTransform[i] = players[i].GetComponent<Transform>();
        }
        

        // filter out the root objects position
        foreach (Transform t in wayPoints) {
            if (!t.Equals(waypointRoot.transform))
                waypoints.Add(t.position);
        }

        fsm = new FSM<EnemyController>();
        fsm.Configure(this, new EnemyPatrol());
    }

    */


    public void OnEnable() {
        this.EventStartListening<GameEvent_RespawnNow>();
       
    }
    public void OnDisable() {
        this.EventStopListening<GameEvent_RespawnNow>();
        
    }
    public void OnGameEvent(GameEvent_RespawnNow gameEvent) {
        Transform[] wayPoints = gameEvent.GetWaypointPos();

        GameObject[] players = GameObject.FindGameObjectsWithTag(Tags.PLAYER);
        playerTransform = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++) {
            playerTransform[i] = players[i].transform;
        }

        // filter out the root objects position
        foreach (Transform t in wayPoints) {
                waypoints.Add(t.position);
        }

        fsm = new FSM<EnemyController>();
        fsm.Configure(this, new EnemyPatrol());
    }
}
