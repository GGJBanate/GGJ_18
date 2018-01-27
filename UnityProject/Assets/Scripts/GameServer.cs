using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameServer : MonoBehaviour
{

    public bool isHostCartOverride = true;

    private static GameServer instance;

    public static GameServer Instance
    {
        get { return instance; }
    }


    private LocalPlayerNetworkConnection ControlRoomPlayer;
    private LocalPlayerNetworkConnection CartPlayer;

    public void Awake()
    {
        instance = this;
    }

    public bool RegisterClient(LocalPlayerNetworkConnection client)
    {
        if (isHostCartOverride)
        {
            if (CartPlayer == null)
            {
                CartPlayer = client;
                return true;
            }

            if (ControlRoomPlayer == null)
            {
                ControlRoomPlayer = client;
                return false;
            }
        }
        else
        {
            if (ControlRoomPlayer == null)
            {
                ControlRoomPlayer = client;
                return false;
            }

            if (CartPlayer == null)
            {
                CartPlayer = client;
                return true;
            }
        }


        throw new InvalidOperationException();
    }
}
