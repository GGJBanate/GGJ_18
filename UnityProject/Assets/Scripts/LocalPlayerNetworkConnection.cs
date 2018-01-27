﻿using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class LocalPlayerNetworkConnection : NetworkBehaviour
{
    public TrackController trackControllerPrefab;
    public ControlRoomController controlRoomControllerPrefab;

    [SyncVar]
    public bool isCartPlayer;

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
            controlRoomControllerInstance = Instantiate(controlRoomControllerPrefab, transform.position, transform.rotation);
            controlRoomControllerInstance.track = TrackData.DeserializeFromNetwork(TrackServer.Instance.serializedTrack);
            controlRoomControllerInstance.map = JsonConvert.DeserializeObject<Dictionary<Pos, TrackPieceData>>(TrackServer.Instance.serializedMap);
            controlRoomControllerInstance.Init();
        }
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
        // TODO
    }
}
