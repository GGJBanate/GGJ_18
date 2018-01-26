using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEntryCollider : MonoBehaviour
{
    public TrackPiece track;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        track.SpawnNextPieces();
    }

}
