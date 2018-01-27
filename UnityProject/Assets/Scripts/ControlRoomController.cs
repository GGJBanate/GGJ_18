using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlRoomController : MonoBehaviour
{
    public TrackData track;
    
    public void Init()
    {
    	TrackMapDisplay mapScreen = GameObject.Find("MapScreen").GetComponent<TrackMapDisplay>();

    	mapScreen.init(new Dictionary<Pos, TrackData> ());
    }
}
