using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameEvents;

public class PlayerController : NetworkBehaviour, IDamageable, IGameEventListener<GameEvent_RespawnNow> {
    #region Variables
    [Header("Components")]
    [SerializeField]
    private Rigidbody rigidBody;
    [SerializeField]
    private GameObject topdownCameraPrefab;
    [SerializeField]
    private BoxCollider boxCollider;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5.0f;

    [Header("Stats")]
    [SerializeField]
    private float maxHealth;

    [Header("Shooting")]
    [SerializeField]
    private LayerMask projectileLayermask;
    [SerializeField]
    private Transform projectileOrigin;
    [SerializeField]
    private float fireSpeed;
    private float nextFire;

    public NetworkIdentity netId;
    public Transform[] startingPositions;

    private GameObject topDownCameraPivot;
    private Camera topDownCamera;

    private ObjectPooler objectPooler;

    [SyncVar]
    private float currenthealth;
    #endregion


    private void Start() {
        if (isLocalPlayer) {
            topDownCameraPivot = Instantiate(topdownCameraPrefab);
            topDownCamera = topDownCameraPivot.GetComponentInChildren<Camera>();
        }
        
        
        objectPooler = ObjectPooler.Instance;
        currenthealth = maxHealth;
    }

    private void Update() {
        if (isLocalPlayer) {
            CheckForProjectile();
        }
    }

    private void FixedUpdate() {
        if (isLocalPlayer) {
            Move();
        }
    }

    void CheckForProjectile() {
        if (Input.GetButton("Fire1") && Time.time > nextFire) {
            Vector3 direction = projectileOrigin.position - gameObject.transform.position;
            CmdRequestProjectile(projectileOrigin.position, direction, gameObject);

            nextFire = Time.time + fireSpeed;
        }
    }

    [Command]
    private void CmdRequestProjectile(Vector3 position, Vector3 direction, GameObject obj) {
        RpcSpawnProjectile(position, direction);
    }

    [ClientRpc]
    private void RpcSpawnProjectile(Vector3 position, Vector3 direction) {
        var bullet = objectPooler.SpawnFromPool("PlayerBullet", projectileOrigin.position, projectileOrigin.rotation);
        SimpleProjectile newProjectile = bullet.GetComponent<SimpleProjectile>();
        newProjectile.InitProjectile(position, direction, this);
    }

    private void Move() {
        Vector3 motion = Vector3.zero;
        bool bMoving = false;

        float horizontalMove = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalMove) > 0.0f) {
            motion += topDownCamera.transform.right * horizontalMove * moveSpeed;
            bMoving = true;
        }

        float verticalMove = Input.GetAxis("Vertical");
        if (Mathf.Abs(verticalMove) > 0.0f) {
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

        if (bMoving) {
            Vector3 lookDirection = motion;
            lookDirection.y = 0.0f;
            rigidBody.MoveRotation(Quaternion.LookRotation(lookDirection, Vector3.up));
        }

        UpdateCamera();
    }

    private void UpdateCamera() {
        topDownCameraPivot.transform.position = transform.position;
    }

    public void Damage(float damageAmount) {
        currenthealth -= damageAmount;

        
        if (currenthealth <= 0.0f) {
            CmdRequestRespawn();
        }
    }

    [Command]
    void CmdRequestRespawn() {
        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn() {
        if (isLocalPlayer) {
            // Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick one at random
            if (startingPositions != null && startingPositions.Length > 0) {
                spawnPoint = startingPositions[Random.Range(0, startingPositions.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
        }
    }



    public void OnEnable() {
        this.EventStartListening<GameEvent_RespawnNow>();
    }
    public void OnDisable() {
        this.EventStopListening<GameEvent_RespawnNow>();
    }
    public void OnGameEvent(GameEvent_RespawnNow gameEvent) {
        startingPositions = gameEvent.GetStartPos();
        CmdRequestRespawn();
    }

}
