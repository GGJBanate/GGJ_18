﻿using System;
using UnityEngine.Networking;

public class GameServer : NetworkBehaviour
{
    private LocalPlayerNetworkConnection cartPlayer;


    private LocalPlayerNetworkConnection controlRoomPlayer;

    //TODO write SETTER for it LUCA pls!!!
    [SyncVar] public bool crossesAreOpen = false;
    public bool DebugMode;

    [SyncVar] public GameStatus gameStatus = GameStatus.Waiting;

    public bool isHostCartOverride = true;

    public static GameServer Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public bool RegisterClient(LocalPlayerNetworkConnection client)
    {
        if (isHostCartOverride)
        {
            if (cartPlayer == null)
            {
                cartPlayer = client;
                return true;
            }

            if (controlRoomPlayer == null)
            {
                controlRoomPlayer = client;
                return false;
            }
        }
        else
        {
            if (controlRoomPlayer == null)
            {
                controlRoomPlayer = client;
                return false;
            }

            if (cartPlayer == null)
            {
                cartPlayer = client;
                return true;
            }
        }


        throw new InvalidOperationException();
    }

    public void RefreshWaiting()
    {
        if (DebugMode || controlRoomPlayer && cartPlayer && controlRoomPlayer.hasStarted && cartPlayer.hasStarted)
            gameStatus = GameStatus.Ongoing;
        else
            gameStatus = GameStatus.Waiting;
    }

    public void SendChatMessage(string message, LocalPlayerNetworkConnection sender)
    {
        LocalPlayerNetworkConnection receiver = sender == cartPlayer ? controlRoomPlayer : cartPlayer;
        if (receiver == null) return;
        receiver.RpcReceiveMessage(message);
    }

    public void SetGameStatus(GameStatus newStatus)
    {
        gameStatus = newStatus;

        if (cartPlayer != null)
            cartPlayer.RpcNotifyGameStateChange(newStatus);
        if (controlRoomPlayer != null)
            controlRoomPlayer.RpcNotifyGameStateChange(newStatus);
    }
}

public enum GameStatus
{
    Ongoing = 0,
    Won = 1,
    GameOver = 2,
    Waiting = 3
}