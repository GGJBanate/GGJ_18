﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class TrackServer : NetworkBehaviour
{
    public int trackLength = 10;

    public float deadEndBreakageProbability = .8f;

    public Dictionary<Pos, TrackData> map = new Dictionary<Pos, TrackData>();

    private static TrackServer instance;

    public static TrackServer Instance
    {
        get { return instance; }
    }
    
    [SyncVar] public string serializedTrack;

    private TrackData track;

    private LocalPlayerNetworkConnection ControlRoomPlayer;
    private LocalPlayerNetworkConnection CartPlayer;

    public void Awake()
    {
        instance = this;
    }

    public override void OnStartServer()
    {
        track = GenerateTrack(0);
        serializedTrack = track.SerializeForNetwork();
    }

    public bool RegisterClient(LocalPlayerNetworkConnection client)
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

        throw new InvalidOperationException();
    }

    private TrackData GenerateTrack(int depth)
    {
        TrackData generatedTrack = new TrackData(Orientation.NN);
        generatedTrack.type = TrackType.Start;
        this.GenerateTrackStep(depth, new Pos(), Orientation.NN, false, generatedTrack);

        return generatedTrack;
    }


    private void GenerateTrackStep(int depth, Pos pos, Orientation o, bool broken, TrackData parent)
    {
        TrackData generatedTrack = new TrackData(o);
        map.Add(pos, generatedTrack);
        parent.track.Add(generatedTrack);


        if (!broken && depth > trackLength)
        {
            generatedTrack.type = TrackType.Finish;
            return;
        }


        if (broken && Random.value < deadEndBreakageProbability)
        {
            generatedTrack.type = TrackType.DeadEnd;
            return;
        }


        Pos pL = pos.Go(o, -1);
        Pos pC = pos.Go(o, 0);
        Pos pR = pos.Go(o, +1);

        switch (Random.Range(0, 4))
        {
            case 0:
            case 1:
                if (map.ContainsKey(pL))
                {
                    generatedTrack.type = TrackType.DeadEnd;
                    break;
                }

                generatedTrack.type = TrackType.Straight;
                GenerateTrackStep(depth + 1, pos.Go(o, 0), o, broken, generatedTrack);
                break;


            case 2:
                if (map.ContainsKey(pL) || map.ContainsKey(pR))
                    goto case 1;

                int brokenDir = Random.Range(0, 2);
                generatedTrack.type = TrackType.TwoWayJunction;
                GenerateTrackStep(depth + 1, pL, (Orientation) ((int) (o + 5) % 6), brokenDir == 0 ? true : broken,
                    generatedTrack);
                GenerateTrackStep(depth + 1, pR, (Orientation) ((int) (o + 1) % 6), brokenDir == 1 ? true : broken,
                    generatedTrack);
                break;


            case 3:
                if (map.ContainsKey(pL) || map.ContainsKey(pC) || map.ContainsKey(pR))
                    goto case 2;

                brokenDir = Random.Range(0, 3);
                generatedTrack.type = TrackType.ThreeWayJunction;
                GenerateTrackStep(depth + 1, pL, (Orientation) ((int) (o + 5) % 6), brokenDir == 0 ? true : broken,
                    generatedTrack);
                GenerateTrackStep(depth + 1, pC, o, brokenDir == 1 ? true : broken, generatedTrack);
                GenerateTrackStep(depth + 1, pR, (Orientation) ((int) (o + 1) % 6), brokenDir == 2 ? true : broken,
                    generatedTrack);
                break;
        }
    }
}