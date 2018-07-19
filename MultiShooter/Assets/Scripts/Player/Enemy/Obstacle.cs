﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Obstacle : NetworkBehaviour {

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
                Destroy(gameObject);
            }
        }
    }
    
}