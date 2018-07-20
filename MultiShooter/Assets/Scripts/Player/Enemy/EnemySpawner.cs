using UnityEngine;
using UnityEngine.Networking;
using GameEvents;

public class EnemySpawner : NetworkBehaviour, IGameEventListener<GameEvent_RespawnNow> {

    private int numberOfEnemies;
    private int numberOfObstacles;
    private ObjectPooler objectPooler;
    public Transform[] startingPositions;
    public Transform[] obstaclePos;


    public override void OnStartServer() {
        objectPooler = ObjectPooler.Instance;

        numberOfEnemies = Random.Range(10,30);
        numberOfObstacles = Random.Range(20,40);

    }

    public void OnEnable() {
        this.EventStartListening<GameEvent_RespawnNow>();
    }
    public void OnDisable() {
        this.EventStopListening<GameEvent_RespawnNow>();
    }
    public void OnGameEvent(GameEvent_RespawnNow gameEvent) {
        startingPositions = gameEvent.GetEnemyPos();

        for (int i = 0; i < numberOfEnemies; i++) {
            var spawnPosition = startingPositions[Random.Range(0, startingPositions.Length)].position;

            var spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);

            int r = Random.Range(1, 4);
            var enemy = objectPooler.SpawnFromPool("Enemy" + r, spawnPosition, spawnRotation);
            NetworkServer.Spawn(enemy);
        }

        obstaclePos = gameEvent.GetObstaclePos();

        for (int i = 0; i < numberOfObstacles; i++) {
            var spawnPosition = obstaclePos[Random.Range(0, obstaclePos.Length)].position;

            var spawnRotation = Quaternion.identity;

            var obstacle = objectPooler.SpawnFromPool("Obstacle", spawnPosition, spawnRotation);
            NetworkServer.Spawn(obstacle);
        }
    }
}