using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlRoomController : MonoBehaviour
{
    public TrackData track;
    public Dictionary<Pos, TrackPieceData> map;

    private TrackMapDisplay mapScreen;

    public void Init()
    {
    	mapScreen = GameObject.Find("MapScreen").GetComponent<TrackMapDisplay>();
    	mapScreen.init(map);
    }

    void Update () {
        mapScreen.map = this.map;
    }
}
