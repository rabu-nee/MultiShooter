using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int damage;
    [SerializeField]
    private GameObject projectileInstigator;
    [SerializeField]
    private Collider instigatorCollider;


    public void InitProjectile(GameObject instigator) {
        projectileInstigator = instigator;

        instigatorCollider = instigator.GetComponent<Collider>();
    }


    void OnTriggerEnter(Collider collision) {
        if (collision != instigatorCollider) {
            var hit = collision.gameObject;
            var health = hit.GetComponent<Health>();
            if (health != null) {
                health.TakeDamage(damage);
            }
            gameObject.SetActive(false);
        }
    }
}
