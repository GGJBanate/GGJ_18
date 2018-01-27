using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameServer : MonoBehaviour
{

    public bool isHostCartOverride = true;

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

    public void SendChatMessage(string message, LocalPlayerNetworkConnection sender)
    {
        LocalPlayerNetworkConnection receiver = sender == cartPlayer ? controlRoomPlayer : cartPlayer;

        receiver.RpcReceiveMessage(message);
    }
}
