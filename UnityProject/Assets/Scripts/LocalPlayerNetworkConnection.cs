using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalPlayerNetworkConnection : NetworkBehaviour
{
    public TrackController trackControllerPrefab;

    [SyncVar]
    public bool isCartPlayer;

    private TrackController trackControllerInstance;

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
            // TODO
        }
    }

    [Command]
    private void CmdRegisterClient()
    {
        try
        {
            isCartPlayer = TrackServer.Instance.RegisterClient(this);
        }
        catch (InvalidOperationException)
        {
            Debug.LogWarning("Too many cooks");
            GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }
    }
}
