using UnityEngine;
using UnityEngine.Networking;
using GameEvents;
using UnityEngine.UI;
using Cinemachine;

public class Player : NetworkBehaviour, IGameEventListener<GameEvent_RespawnNow>, IGameEventListener<GameEvent_RespawnDeath>,
    IGameEventListener<GameEvent_EnemyKill>, IGameEventListener<GameEvent_Win>, IGameEventListener<GameEvent_PlayerKill> {
    public Transform bulletSpawn;
    public Material playerMat;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private Rigidbody rigidBody;

    [SerializeField]
    private float fireSpeed;
    private float nextFire;
    public float bulletSpeed;

    [SyncVar(hook = "OnChangeScore")]
    private float score;
    public RectTransform scoreTransform;
    public Text scoreText;

    public Health health;

    public Transform[] startingPositions;
 
    private ObjectPooler objectPooler;

    public bool canMove;

    public GameObject cameraPrefab;
    public CinemachineVirtualCamera cam;


    private void Start() {
        if (isLocalPlayer) {
            GameObject camera = Instantiate(cameraPrefab);
            cam = camera.GetComponentInChildren<CinemachineVirtualCamera>();
            cam.Follow = gameObject.transform;
            cam.LookAt = gameObject.transform;
        }

        objectPooler = ObjectPooler.Instance;
    }

    void Update() {
        if (!isLocalPlayer) {
            return;
        }

        if (canMove) {
            Move();

            if ((Input.GetButton("Fire1") || Input.GetAxis("Triggers") == 1 || Input.GetAxis("Triggers") == -1) && Time.time > nextFire) {
                CmdFire();
                GameEventManager.TriggerEvent(new GameEvent_Shoot());
                nextFire = Time.time + fireSpeed;
            }
        }
    }

    /*
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

    */

    private void Move() {
        float horizontalMove = Input.GetAxis("LeftHorizontal");
        float verticalMove = Input.GetAxis("LeftVertical");

        rigidBody.velocity = new Vector3(horizontalMove * moveSpeed, rigidBody.velocity.y, verticalMove * moveSpeed);

        if (Input.GetAxis("RightHorizontal") != 0 || Input.GetAxis("RightVertical") != 0) {
            float rx = Input.GetAxis("RightHorizontal");
            float ry = Input.GetAxis("RightVertical");

            float angle = Mathf.Atan2(rx, ry);

            transform.rotation = Quaternion.EulerAngles(0, angle, 0);
        }
        else {

            float angle1 = Mathf.Atan2(horizontalMove, verticalMove);
            transform.rotation = Quaternion.EulerAngles(0, angle1, 0);
        }

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
            canMove = true;
        }
    }
    public void OnEnable() {
        this.EventStartListening<GameEvent_RespawnNow>();
        this.EventStartListening<GameEvent_RespawnDeath>();
        this.EventStartListening<GameEvent_EnemyKill>();
        this.EventStartListening<GameEvent_PlayerKill>();        
        this.EventStartListening<GameEvent_Win>();        
    }
    public void OnDisable() {
        this.EventStopListening<GameEvent_RespawnNow>();
        this.EventStopListening<GameEvent_RespawnDeath>();
        this.EventStopListening<GameEvent_EnemyKill>();
        this.EventStopListening<GameEvent_PlayerKill>();
        this.EventStopListening<GameEvent_Win>();
    }
    public void OnGameEvent(GameEvent_RespawnNow gameEvent) {
        startingPositions = gameEvent.GetStartPos();
        score = 0;
        CmdRequestRespawn();
    }

    public void OnGameEvent(GameEvent_RespawnDeath gameEvent) {
        if (health.currentHealth <= 0) {
            GameObject killer = gameEvent.GetInstigator();

            if(!GameObject.ReferenceEquals(killer, gameObject)) {
                score -= gameEvent.GetPenalty();
            }

            CmdRequestRespawn();
        }
    }

    public void OnGameEvent(GameEvent_EnemyKill gameEvent) {
        GameObject enemyKiller = gameEvent.GetInstigator();

        if(GameObject.ReferenceEquals(enemyKiller,gameObject)) {
            score += gameEvent.GetScoreAdd();
        }
    }

    public void OnGameEvent(GameEvent_PlayerKill gameEvent) {
        GameObject playerKiller = gameEvent.GetInstigator();

        if (GameObject.ReferenceEquals(playerKiller, gameObject)) {
            score += gameEvent.GetScoreAdd();
        }
    }

    public void OnGameEvent(GameEvent_Win gameEvent) {
        CmdDisableMove(false);
    }

    [Command]
    void CmdDisableMove(bool state) {
        RpcDisableMove(state);
    }

    [ClientRpc]
    void RpcDisableMove(bool state) {
        canMove = state;
    }

    void OnChangeScore(float score) {
        scoreText.text = "" + score;
    }

    public override void OnStartLocalPlayer() {
        GetComponent<MeshRenderer>().material = playerMat;

        //scoreTransform.position = new Vector2(scoreTransform.position.x, scoreTransform.position.y);
    }
}