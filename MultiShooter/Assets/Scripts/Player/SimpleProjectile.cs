using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour {
    [SerializeField]
    private Rigidbody projectileRigidBody;
    [SerializeField]
    private int damageAmount;
    [SerializeField]
    private float speed;
    [SerializeField]
    private PlayerController projectileInstigator;
    [SerializeField]
    private Collider instigatorCollider;

    void Start() {

    }

    public void InitProjectile(Vector3 position, Vector3 direction, PlayerController instigator) {

        transform.position = position;
        projectileRigidBody.velocity = direction * speed;
        projectileInstigator = instigator;

        instigatorCollider = instigator.GetComponentInChildren<Collider>();
    }


    void OnTriggerEnter(Collider collision) {
        if (collision != instigatorCollider) {
            var hit = collision.gameObject;
            var health = hit.GetComponent<Health>();
            if (health != null) {
                health.TakeDamage(damageAmount);
            }
                    gameObject.SetActive(false) ;
        }
    }


}
