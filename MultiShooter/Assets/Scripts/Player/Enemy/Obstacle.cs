using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Obstacle : NetworkBehaviour, IDamageable {

    public BoxCollider boxCollider;
    [SyncVar]
    public float currenthealth;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Damage(float damageAmount) {
        currenthealth -= damageAmount;

        if (currenthealth <= 0.0f) {
            RpcDestroy();
        }
    }

    [ClientRpc]
    void RpcDestroy() {
        if (!isServer)
            return;

        enabled = false;
        gameObject.SetActive(false);
    }
}
