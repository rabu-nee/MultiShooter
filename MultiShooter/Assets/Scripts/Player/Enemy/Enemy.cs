using UnityEngine;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour {
    public Transform[] bulletSpawn;

    [SerializeField]
    private float fireSpeed;
    private float nextFire;
    public float bulletSpeed;


    private ObjectPooler objectPooler;

    private void Start() {
        objectPooler = ObjectPooler.Instance;
    }

    void Update() {
        if (!isLocalPlayer) {
            return;
        }

        if (Time.time > nextFire) {
            CmdFire();
            nextFire = Time.time + fireSpeed;
        }
    }

    // This [Command] code is called on the Client …
    // … but it is run on the Server!
    [Command]
    void CmdFire() {
        foreach (Transform bulletSp in bulletSpawn) {
            // Create the Bullet from the Bullet Prefab
            var bullet = objectPooler.SpawnFromPool("EnemyBullet1", bulletSp.position, bulletSp.rotation);

            // Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

            // Spawn the bullet on the Clients
            NetworkServer.Spawn(bullet);
        }
    }
}