using UnityEngine;

public class TrackEntryCollider : MonoBehaviour
{
    [HideInInspector] private bool _active;

    public GameObject activityIndicator;
    [HideInInspector] public TrackPiece track;

    public bool active
    {
        get { return _active; }

        set
        {
            if (activityIndicator != null) activityIndicator.SetActive(value && GameServer.Instance.DebugMode);
            _active = value;
        }
    }

    private void Start()
    {
        if (activityIndicator != null) activityIndicator.SetActive(GameServer.Instance.DebugMode);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        CartPlayerController player = other.GetComponent<CartPlayerController>();

        if (active)
        {
            Destroy(player.currentTrack.gameObject, 1);
            player.currentTrack = track;
            track.SpawnNextPieces();
        }
    }
}