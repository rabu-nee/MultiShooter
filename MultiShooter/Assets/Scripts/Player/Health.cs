using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using GameEvents;

public class Health : NetworkBehaviour {

    public const int maxHealth = 5;

    public bool destroyOnDeath;

    [SyncVar]
    public int currentHealth = maxHealth;


    public void TakeDamage(int amount) {
        if (!isServer)
            return;

        currentHealth -= amount;
        if (currentHealth <= 0) {
            if (destroyOnDeath) {
                RpcDestroy();
            }
            else {
                GameEventManager.TriggerEvent(new GameEvent_RespawnDeath(100f));
                currentHealth = maxHealth;
            }
        }
    }

    [ClientRpc]
    void RpcDestroy() {
        Destroy(gameObject);
    }
}