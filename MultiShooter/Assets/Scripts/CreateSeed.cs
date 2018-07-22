using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameEvents;
using UnityEngine.UI;

public class CreateSeed : NetworkBehaviour, IGameEventListener<GameEvent_Spawn>, IGameEventListener<GameEvent_EnemyKill>{

    public string seed;
    public GameObject buttonObj;
    public GameObject exitbutton;
    public GameObject EndGametext;
    public MapGenerator mapGen;
    public int enemyNumber;
    public int remainingEnemies;


    private void Start() {
        if (!isServer) {
            buttonObj.SetActive(false);
        }
    }

    public void OnButtonClick() {
        CmdSendSeed();
        CmdStartGame();
    }

    [Command]
    public void CmdSendSeed() {
        string value = Random.Range(1, 1000).ToString();
        RpcSetSeed(value);
    }

    [ClientRpc]
    public void RpcSetSeed(string value) {
        seed = value;
        GameEventManager.TriggerEvent(new GameEvent_SendSeed(seed));
    }

    [Command]
    void CmdStartGame() {
        RpcStartGame();
    }

    [ClientRpc]
    void RpcStartGame() {
        mapGen.GenerateMap();
        GameEventManager.TriggerEvent(new GameEvent_RespawnNow(mapGen.startingPositions, mapGen.enemyStartPos, mapGen.obstacleStartPos, mapGen.waypointPos));
        buttonObj.SetActive(false);
    }


    public void OnEnable() {
        this.EventStartListening<GameEvent_Spawn>();
        this.EventStartListening<GameEvent_EnemyKill>();
    }
    public void OnDisable() {
        this.EventStopListening<GameEvent_Spawn>();
        this.EventStopListening<GameEvent_EnemyKill>();
    }
    public void OnGameEvent(GameEvent_Spawn gameEvent) {
        enemyNumber = gameEvent.GetEnemyNumber();
        remainingEnemies = enemyNumber;
    }

    public void OnGameEvent(GameEvent_EnemyKill gameEvent) {
            CmdCheckWin();
    }

    [Command]
    void CmdCheckWin() {
        RpcCheckWin();
    }

    [ClientRpc]
    void RpcCheckWin() {
        remainingEnemies--;
        if (remainingEnemies == 0) {

            GameEventManager.TriggerEvent(new GameEvent_Win());
            exitbutton.SetActive(true);
            EndGametext.SetActive(true);
        }
    }

    public void exitButton() {
        if (!isServer) {
            NetworkManager.singleton.StopClient();
        }
        else {
            NetworkManager.singleton.StopHost();
        }
    }
}
