using UnityEngine;
using UnityEngine.AI;
using C;
using UnityEngine.Networking;

public class EnemyChase : IFSMState<EnemyController>{
    //private float speed = 8f;
    private float notSeePlayerTime = 0;
    private float chasingTime = 0;
    private NavMeshAgent agent;

    public void Enter(EnemyController e) {
        agent = e.GetComponent<NavMeshAgent>();
        agent.SetDestination(e.playerTransform.position);
        Debug.Log("started chasing");
    }

    public void Exit(EnemyController e) {
        Debug.Log("stopped chasing");
    }

    public void Reason(EnemyController e) {
        // can we see the player? If not, we get out of here
        RaycastHit hit;
        var canSeePlayer = false;
        if (Physics.Raycast(e.transform.position, e.playerTransform.position - e.transform.position, out hit)) {
            if (hit.transform.tag == Tags.PLAYER) {
                canSeePlayer = true;
                Debug.DrawRay(e.transform.position, e.playerTransform.position - e.transform.position, Color.red);
            }
            else {
                Debug.DrawRay(e.transform.position, e.playerTransform.position - e.transform.position, Color.green);
            }
        }

        if (!canSeePlayer) {
            notSeePlayerTime += Time.deltaTime;
        }
        else {
            notSeePlayerTime = 0;
        }

        if (notSeePlayerTime > 3.0) {
            e.ChangeState(EnemyPatrol.Instance);
            RpcSetEnemyScriptActive(e.enemyScript, false);
        }
    }

    public void Update(EnemyController e) {

        chasingTime += Time.deltaTime;
        if (chasingTime > 0.5f) {
            agent.SetDestination(e.playerTransform.position);
            chasingTime = 0;
            Debug.Log("Setting Target...");
            RpcSetEnemyScriptActive(e.enemyScript, true);
        }

        // run after the player!
        //var directionToPlayer = e.playerTransform.position - e.transform.position;
        //e.transform.rotation = Quaternion.LookRotation( directionToPlayer );
        //e.transform.position += ( directionToPlayer.normalized * speed * Time.deltaTime );
    }

    [ClientRpc]
    void RpcSetEnemyScriptActive(Enemy enemy, bool state) {
        enemy.enabled = state;
    }
}
