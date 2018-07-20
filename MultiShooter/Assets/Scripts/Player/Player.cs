using UnityEngine;
using UnityEngine.Networking;
using GameEvents;

public class Player : NetworkBehaviour, IGameEventListener<GameEvent_RespawnNow>, IGameEventListener<GameEvent_RespawnDeath>, IGameEventListener<GameEvent_EnemyKill> {
    public Transform bulletSpawn;
    public Material playerMat;


    [SerializeField]
    private GameObject topdownCameraPrefab;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private Rigidbody rigidBody;

    [SerializeField]
    private float fireSpeed;
    private float nextFire;
    public float bulletSpeed;

    public NetworkIdentity netIdentity;
    public int index;

    [SyncVar]
    private float score;

    public Health health;

    public Transform[] startingPositions;

    private GameObject topDownCameraPivot;
    private Camera topDownCamera;

    private ObjectPooler objectPooler;

    private void Start() {
        if (isLocalPlayer) {
            topDownCameraPivot = Instantiate(topdownCameraPrefab);
            topDownCamera = topDownCameraPivot.GetComponentInChildren<Camera>();
        }

        index = int.Parse(netId.ToString());

        objectPooler = ObjectPooler.Instance;
    }

    void Update() {
        if (!isLocalPlayer) {
            return;
        }

        Move();

        if (Input.GetButton("Fire1") && Time.time > nextFire) {
            CmdFire();
            GameEventManager.TriggerEvent(new GameEvent_Shoot());
            nextFire = Time.time + fireSpeed;
        }
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

    [Command]
    void CmdFire() {
        RpcSpawnBullet();
    }

    [ClientRpc]
    void RpcSpawnBullet() {
        // Create the Bullet from the Bullet Prefab
        var bullet = objectPooler.SpawnFromPool("PlayerBullet", bulletSpawn.position, bulletSpawn.rotation);

        Bullet newProjectile = bullet.GetComponent<Bullet>();
        newProjectile.InitProjectile(gameObject);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;


    }

    public override void OnStartLocalPlayer() {
        GetComponent<MeshRenderer>().material = playerMat;
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
        this.EventStartListening<GameEvent_RespawnDeath>();
        this.EventStartListening<GameEvent_EnemyKill>();
    }
    public void OnDisable() {
        this.EventStopListening<GameEvent_RespawnNow>();
        this.EventStopListening<GameEvent_RespawnDeath>();
        this.EventStopListening<GameEvent_EnemyKill>();
    }
    public void OnGameEvent(GameEvent_RespawnNow gameEvent) {
        startingPositions = gameEvent.GetStartPos();
        CmdRequestRespawn();
    }

    public void OnGameEvent(GameEvent_RespawnDeath gameEvent) {
        if (health.currentHealth <= 0) {
            GameObject killer = gameEvent.GetInstigator();

            if(!GameObject.ReferenceEquals(killer, gameObject)) {
                score -= gameEvent.GetPenalty();
                GameEventManager.TriggerEvent(new GameEvent_ScoreUpdate(score, index));
            }

            CmdRequestRespawn();
        }
    }

    public void OnGameEvent(GameEvent_EnemyKill gameEvent) {
        GameObject enemyKiller = gameEvent.GetInstigator();

        if(GameObject.ReferenceEquals(enemyKiller,gameObject)) {
            score += gameEvent.GetScoreAdd();
            GameEventManager.TriggerEvent(new GameEvent_ScoreUpdate(score, index));
        }
    }
}