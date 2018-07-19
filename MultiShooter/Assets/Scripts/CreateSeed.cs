using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameEvents;
using UnityEngine.UI;

public class CreateSeed : NetworkBehaviour {

    public string seed;
    public GameObject buttonObj;
    public MapGenerator mapGen;


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
        GameEventManager.TriggerEvent(new GameEvent_RespawnNow(mapGen.startingPositions));
        buttonObj.SetActive(false);
    }
}
