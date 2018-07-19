using UnityEngine;
using UnityEngine.Networking;
using GameEvents;

public class EnemySpawner : NetworkBehaviour, IGameEventListener<GameEvent_RespawnNow> {

    private int numberOfEnemies;
    public int max, min;
    private ObjectPooler objectPooler;
    public Transform[] startingPositions;


    public override void OnStartServer() {
        objectPooler = ObjectPooler.Instance;

        numberOfEnemies = Random.Range(min,max);
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

            var spawnRotation = Quaternion.Euler(
                0.0f,
                Random.Range(0, 180),
                0.0f);

            //var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);

            int r = Random.Range(1, 3);
            var enemy = objectPooler.SpawnFromPool("Enemy" + r, spawnPosition, spawnRotation);
            NetworkServer.Spawn(enemy);
        }
    }
}