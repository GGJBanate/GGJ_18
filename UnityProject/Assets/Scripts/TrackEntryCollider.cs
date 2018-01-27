using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEntryCollider : MonoBehaviour
{
    [HideInInspector] public TrackPiece track;

    [HideInInspector] public bool active;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        CartPlayerController player = other.GetComponent<CartPlayerController>();

        if (active)
        {
            Destroy(player.currentTrack.gameObject, 1);
            player.currentTrack = this.track;
            track.SpawnNextPieces();
        }
        else
        {
           // Destroy(track.gameObject, 3); // TODO What
        }
    }
}