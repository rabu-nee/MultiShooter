using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameEvents;
using UnityEngine.UI;

public class CreateSeed : NetworkBehaviour, IGameEventListener<GameEvent_ScoreUpdate> {

    public string seed;
    public GameObject buttonObj;
    public MapGenerator mapGen;
    public Text[] scoreTexts;


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
        this.EventStartListening<GameEvent_ScoreUpdate>();

    }
    public void OnDisable() {
        this.EventStartListening<GameEvent_ScoreUpdate>();
    }


    public void OnGameEvent(GameEvent_ScoreUpdate gameEvent) {
        float score = gameEvent.GetScore();
        int index = gameEvent.GetIndex();

        RpcUpdateScore(index, score);
    }

    [ClientRpc]
    void RpcUpdateScore( int index,float score) {

        scoreTexts[index].text = "Player " +index + ": " + score;
    }
}
