using UnityEngine;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour {
    public Transform[] bulletSpawn;

    [SerializeField]
    private float fireSpeed;
    private float nextFire;
    public float bulletSpeed;


    private ObjectPooler objectPooler;

    public void Start() {
        objectPooler = ObjectPooler.Instance;
    }
    
    void Update() {
        if (!isServer)
            return;

        if (Time.time > nextFire) {
            CmdRequestBullet();
            nextFire = Time.time + fireSpeed;
        }
    }

    [Command]
    void CmdRequestBullet() {

 

        RpcSpawnBullet();
    }

    [ClientRpc]
    void RpcSpawnBullet() {
        foreach (Transform bulletSp in bulletSpawn) {
            var bullet = objectPooler.SpawnFromPool("EnemyBullet1", bulletSp.position, bulletSp.rotation);

            Bullet newProjectile = bullet.GetComponent<Bullet>();
            newProjectile.InitProjectile(gameObject);
            Vector3 direction = bulletSp.position - gameObject.transform.position;
            // Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;


        }
    }
}