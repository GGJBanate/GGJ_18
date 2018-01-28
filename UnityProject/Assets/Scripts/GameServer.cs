using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameServer : NetworkBehaviour
{

    public bool isHostCartOverride = true;
    
    [SyncVar]
    public GameStatus gameStatus = GameStatus.Waiting;

    //TODO write SETTER for it LUCA pls!!!
    [SyncVar]
    public bool crossesAreOpen = false;

    public static GameServer Instance { get; private set; }


    private LocalPlayerNetworkConnection controlRoomPlayer;
    private LocalPlayerNetworkConnection cartPlayer;

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
		if (controlRoomPlayer && cartPlayer && controlRoomPlayer.hasStarted && cartPlayer.hasStarted) {
			gameStatus = GameStatus.Ongoing;
		} else {
			gameStatus = GameStatus.Waiting;
		}
    }

    public void SendChatMessage(string message, LocalPlayerNetworkConnection sender)
    {
        LocalPlayerNetworkConnection receiver = sender == cartPlayer ? controlRoomPlayer : cartPlayer;
        if (receiver == null) return;
        receiver.RpcReceiveMessage(message);
    }
}

public enum GameStatus {
    Ongoing = 0,
    Won = 1,
    GameOver = 2,
    Waiting = 3
}
