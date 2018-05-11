using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 150.0f;
    public float rotationSpeed = 3.0f;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public float bulletSpeed = 6.0f;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * rotationSpeed;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdShoot();
        }
    }

    [Command]
    private void CmdShoot()
    {
        //create the bullet
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        //add velocity to bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        //Spawn bullet on clients
        NetworkServer.Spawn(bullet);
        
        //destroy bullet after 2 sec
        //TO BE CHANGED TO COLLISION
        Destroy(bullet, 2.0f);
    }

    public override void OnStartLocalPlayer()
    {
        //configure cameras and input
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}