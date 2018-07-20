using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using GameEvents;

public class Health : NetworkBehaviour {

    public const int maxHealth = 5;

    public bool destroyOnDeath;

    public float addScore;

    public GameObject latestDamageGiver;

    [SyncVar]
    public int currentHealth = maxHealth;


    public void TakeDamage(int amount, GameObject instigator) {
        if (!isServer)
            return;
        latestDamageGiver = instigator;

        currentHealth -= amount;
        if (currentHealth <= 0) {
            if (destroyOnDeath) {
                GameEventManager.TriggerEvent(new GameEvent_EnemyKill(addScore, latestDamageGiver));
                RpcDestroy();
            }
            else {
                GameEventManager.TriggerEvent(new GameEvent_EnemyKill(addScore, latestDamageGiver));
                GameEventManager.TriggerEvent(new GameEvent_RespawnDeath(100f, latestDamageGiver));
                currentHealth = maxHealth;
            }
        }

        latestDamageGiver = null;
    }

    [ClientRpc]
    void RpcDestroy() {
        Destroy(gameObject);
    }
}