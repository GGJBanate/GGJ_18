using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlRoomController : MonoBehaviour
{
    public TrackData track;
    public Dictionary<Pos, TrackPieceData> map;

    public void Init()
    {
    	TrackMapDisplay mapScreen = GameObject.Find("MapScreen").GetComponent<TrackMapDisplay>();

    	mapScreen.init(new Dictionary<Pos, TrackData> ());
    }
}
