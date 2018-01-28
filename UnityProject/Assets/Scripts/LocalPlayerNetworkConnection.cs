using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class LocalPlayerNetworkConnection : NetworkBehaviour
{
    public TrackController trackControllerPrefab;
    public ControlRoomController controlRoomControllerPrefab;

    public GameObject winScreen;
    public GameObject gameOverScreen;

    [SyncVar] public bool isCartPlayer;

    [SyncVar] public bool hasStarted;

    private TrackController trackControllerInstance;
    private ControlRoomController controlRoomControllerInstance;

    public override void OnStartLocalPlayer()
    {
        CmdRegisterClient();

        if (isCartPlayer)
        {
            trackControllerInstance = Instantiate(trackControllerPrefab, transform.position, transform.rotation);
            trackControllerInstance.track = TrackData.DeserializeFromNetwork(TrackServer.Instance.serializedTrack);
            trackControllerInstance.BuildTrack();
        }
        else
        {
            controlRoomControllerInstance =
                Instantiate(controlRoomControllerPrefab, transform.position, transform.rotation);
            controlRoomControllerInstance.track =
                TrackData.DeserializeFromNetwork(TrackServer.Instance.serializedTrack);
            controlRoomControllerInstance.map =
                JsonConvert.DeserializeObject<Dictionary<Pos, TrackPieceData>>(TrackServer.Instance.serializedMap);
            controlRoomControllerInstance.Init();
        }
    }

    [ClientRpc]
    public void RpcSetSwitchState(int trackPieceId, int switchDirection)
    {
        TrackController.Instance.SetSwitchState(trackPieceId, switchDirection);
    }

    [Command]
    private void CmdRegisterClient()
    {
        try
        {
            isCartPlayer = GameServer.Instance.RegisterClient(this);
        }
        catch (InvalidOperationException)
        {
            Debug.LogWarning("Too many cooks");
            GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        GameServer.Instance.SendChatMessage(message, this);
    }

    [ClientRpc]
    public void RpcReceiveMessage(string message)
    {
        if (isLocalPlayer)
            Messenger.Instance.receiveMsg(message);
    }

    public void SendChatMessage(string msg)
    {
        if (isLocalPlayer)
            CmdSendMessage(msg);
    }

    public void SetGameStatus(GameStatus newStatus)
    {
        CmdSetGameStatus(newStatus);
    }

    [Command]
    public void CmdSetGameStatus(GameStatus newStatus)
    {
        GameServer.Instance.SetGameStatus(newStatus);
    }

    public void StartGame()
    {
        CmdStartGame();
    }

    [Command]
    public void CmdStartGame()
    {
        this.hasStarted = true;
        GameServer.Instance.RefreshWaiting();
    }

    private bool barrierSettingBlocked;

    public void ResetBarrier()
    {
        barrierSettingBlocked = false;

        CmdSetBarrier();
    }

    public void SetBarrier()
    {
        if (barrierSettingBlocked) return;

        barrierSettingBlocked = true;

        CmdSetBarrier();

        Invoke("ResetBarrier", 4f);
    }

    [Command]
    public void CmdSetBarrier()
    {
        GameServer.Instance.crossesAreOpen = !GameServer.Instance.crossesAreOpen;
    }

    [ClientRpc]
    public void RpcNotifyGameStateChange(GameStatus state)
    {
        if (isLocalPlayer)
        {
            winScreen.SetActive(state == GameStatus.Won);
            gameOverScreen.SetActive(state == GameStatus.GameOver);
        }
    }
}