using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movetest : MonoBehaviour {

    public float moveSpeed = 150.0f;
    public float rotationSpeed = 3.0f;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public float bulletSpeed = 6.0f;

    void Update()
    {

        //MOVE CODE

        float x = Input.GetAxis("Horizontal");
        float z = -Input.GetAxis("Vertical");
        if (x != 0.0f || z != 0.0f)
        {
            float angle = Mathf.Atan2(z, x) * Mathf.Rad2Deg;

            transform.localRotation = Quaternion.Euler(new Vector3(0, angle+90, 0));
        }

        transform.Translate(x, 0, -z);

        //MOVE END

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {

    }

}
