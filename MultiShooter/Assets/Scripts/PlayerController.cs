using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField]
    private Rigidbody rigidBody;
    [SerializeField]
    private GameObject topdownCameraPrefab;
    [SerializeField]
    private CapsuleCollider capsuleCollider;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private float jumpForce = 500.0f;


    [Header("Stats")]
    [SerializeField]
    private float maxHealth;

    [Header("Shooting")]
    [SerializeField]
    private LayerMask projectileLayermask;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileOrigin;

    private GameObject topDownCameraPivot;
    private Camera topDownCamera;

    private int walkAnimationId;

    [SyncVar]
    private float currenthealth;

    private void Start()
    {
        if (isLocalPlayer)
        {
            topDownCameraPivot = Instantiate(topdownCameraPrefab);
            topDownCamera = topDownCameraPivot.GetComponentInChildren<Camera>();
        }

        currenthealth = maxHealth;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            //CheckForProjectile();
            if (Input.GetButton("Fire1"))
            {
                CmdFire();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Move();
        }
    }

    /*
    private void CheckForProjectile()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit result;
            if (Physics.Raycast(topDownCamera.ScreenPointToRay(Input.mousePosition), out result, 100.0f, projectileLayermask))
            {
                Vector3 cachedPosition = projectileOrigin.transform.position;
                Vector3 targetPosition = result.point;
                targetPosition.y = cachedPosition.y;

                //This runs on the local authority client
                CmdRequestProjectile(cachedPosition, targetPosition - cachedPosition, gameObject);
            }
        }
    }

    [Command]
    private void CmdRequestProjectile(Vector3 position, Vector3 targetPosition, GameObject obj)
    {
        //This runs on the server
        RpcSpawnProjectile(position, targetPosition);
    }

    [ClientRpc]
    private void RpcSpawnProjectile(Vector3 position, Vector3 targetPosition)
    {
        SimpleProjectile newProjectile = Instantiate(projectilePrefab);
        newProjectile.InitProjectile(position, targetPosition, this);
    }
    */

    [Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            projectilePrefab,
            projectileOrigin.position,
            projectileOrigin.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

    IEnumerator WaitForNextBullet()
    {
        Debug.Log("A");
        yield return new WaitForSeconds(5f);
    }

    private void Move()
    {
        Vector3 motion = Vector3.zero;
        bool bMoving = false;

        float horizontalMove = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalMove) > 0.0f)
        {
            motion += topDownCamera.transform.right * horizontalMove * moveSpeed;
            bMoving = true;
        }

        float verticalMove = Input.GetAxis("Vertical");
        if (Mathf.Abs(verticalMove) > 0.0f)
        {
            Vector3 forward = topDownCamera.transform.forward;
            forward.y = 0.0f;
            forward.Normalize();

            motion += forward * verticalMove * moveSpeed;
            bMoving = true;
        }

        motion += Physics.gravity;

        Vector3 velocity = rigidBody.velocity;
        motion *= Time.fixedDeltaTime;

        velocity.x = motion.x;
        velocity.y += motion.y;
        velocity.z = motion.z;
        rigidBody.velocity = velocity;

        if (bMoving)
        {
            Vector3 lookDirection = motion;
            lookDirection.y = 0.0f;
            rigidBody.MoveRotation(Quaternion.LookRotation(lookDirection, Vector3.up));
        }

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        topDownCameraPivot.transform.position = transform.position;
    }


    public void Damage(float damageAmount)
    {
        currenthealth -= damageAmount;

        if (currenthealth <= 0.0f)
        {
            enabled = false;
            capsuleCollider.gameObject.SetActive(false);
        }
    }
}
