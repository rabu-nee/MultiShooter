using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using GameEvents;

public class Health : NetworkBehaviour {

    public const int maxHealth = 5;

    public bool destroyOnDeath;
    public bool obstacle;

    public float addScore;

    public GameObject latestDamageGiver;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public RectTransform healthBar;
    private float initialWidth;

    public override void OnStartLocalPlayer() {
        if(!destroyOnDeath)
        initialWidth = healthBar.sizeDelta.x;
    }


    public void TakeDamage(int amount, GameObject instigator) {
        if (!isServer)
            return;
        latestDamageGiver = instigator;

        currentHealth -= amount;
        if (currentHealth <= 0) {
            if (destroyOnDeath) {
                if(!obstacle)
                    GameEventManager.TriggerEvent(new GameEvent_EnemyKill(addScore, latestDamageGiver));
                RpcDestroy();
            }
            else {
                GameEventManager.TriggerEvent(new GameEvent_PlayerKill(addScore, latestDamageGiver));
                GameEventManager.TriggerEvent(new GameEvent_RespawnDeath(100f, latestDamageGiver));
                currentHealth = maxHealth;
            }
        }

        latestDamageGiver = null;
    }

    void OnChangeHealth(int health) {
        if (!destroyOnDeath) {
            healthBar.sizeDelta = new Vector2(healthBar.sizeDelta.x * health / maxHealth, healthBar.sizeDelta.y);
            if (health == 0)
                healthBar.sizeDelta = new Vector2(initialWidth, healthBar.sizeDelta.y);
        }
    }

    [ClientRpc]
    void RpcDestroy() {
        Destroy(gameObject);
    }
}