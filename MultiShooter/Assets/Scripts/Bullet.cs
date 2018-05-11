using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }
}
