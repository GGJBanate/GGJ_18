using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEntryCollider : MonoBehaviour
{
    [HideInInspector] public TrackPiece track;

    [HideInInspector] private bool _active;

    public GameObject activityIndicator;

    public bool active
    {
        get
        {
            return _active;
        }

        set
        {
            if (activityIndicator != null) activityIndicator.SetActive(value && GameServer.Instance.DebugMode);
            _active = value;
        }
    }

    void Start()
    {
        if (activityIndicator != null) activityIndicator.SetActive(GameServer.Instance.DebugMode);
    }

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