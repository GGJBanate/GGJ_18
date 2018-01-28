using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackMapDisplay : MonoBehaviour {

    public GameObject canvas;

    public Sprite hexagon;
    public Sprite hexagonStraight;
    public Sprite hexagonTwoWayJunction;
    public Sprite hexagonThreeWayJunction;
    public Sprite hexagonFinish;
    public Sprite hexagonDeadEnd;
    public Sprite hexagonStart;

    private Vector3 offset;

    const float PER_TILE_OFFSET_X = 0.14f;

    public static readonly Vector3 SCALE = new Vector3(1, 0.866f, 1);

    public Dictionary<Pos, TrackPieceData> map;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        /*
        foreach(KeyValuePair<Pos, TrackPieceData> entry in map)
        {
            UpdatePiece(entry.Key, entry.Value);
        }
        */
    }

    public void init(Dictionary<Pos, TrackPieceData> map) {
        this.map = map;

        int top = 0, right = 0, bottom = 0, left = 0;

        Pos p;
        foreach(KeyValuePair<Pos, TrackPieceData> entry in map)
        {
            p = entry.Key;
            if(p.x < left) left = p.x;
            if(p.x > right) right = p.x;
            if(p.y < top) top = p.y;
            if(p.y > bottom) bottom = p.y;
        }

        offset = new Vector3(- (right + left) / 2, (top + bottom) / 2, 0);

        foreach(KeyValuePair<Pos, TrackPieceData> entry in map)
        {
            AddTextToCanvas(entry.Key, entry.Value);
        }
    }

    public void AddTextToCanvas(Pos pos, TrackPieceData trackPiece)
    {
        Sprite s;

        switch(trackPiece.type) {
            case TrackType.Broken:
            case TrackType.Danger:
            case TrackType.Straight: 
                s = hexagonStraight;
                break;

            case TrackType.TwoWayJunction: 
                s = hexagonTwoWayJunction;
                break;

            case TrackType.ThreeWayJunction: 
                s = hexagonThreeWayJunction;
                break;

            case TrackType.Finish: 
                s = hexagonFinish;
                break;

            case TrackType.DeadEnd: 
                s = hexagonDeadEnd;
                break;

            case TrackType.Start: 
                s = hexagonStart;
                break;

            default:
                s = hexagon;
                break;
        }

        for(int z = 0; z < 2; ++z)
            AddImage(pos, trackPiece.o, s, z, trackPiece.type == TrackType.Start );
    }

    private void AddImage(Pos p, Orientation o, Sprite s, int z = 0, bool highlight = false)
    {
        GameObject go = new GameObject(GetPieceName(p), typeof(RectTransform));
        Color c = new Color(highlight ? 1f - z*0.5f : 0f, 0f, 0f, 1 - z*0.5f);

        var imageComp = go.AddComponent<Image>();
        imageComp.sprite = s;
        imageComp.color = c;

        go.transform.SetParent(canvas.transform);
        go.transform.localScale = SCALE;
        go.transform.localPosition = new Vector3 (
            offset.x + p.x - PER_TILE_OFFSET_X * p.x, 
            offset.y - (p.y - ((p.x + 100) % 2 == 0 ? -0.5f : 0) ), 
            z * 0.03f
        ) * 100;
        go.transform.localRotation = Quaternion.identity;
        go.transform.Rotate(Vector3.forward, ((int) o) * -60);
    }

    private void UpdatePiece(Pos pos, TrackPieceData trackPiece)
    {
        var img = GameObject.Find(GetPieceName(pos)).GetComponent<Image>();
        Color c = new Color(!trackPiece.brokenPath ? 1f : 0f, 0f, 0f, 1f);
        img.color = c;
    }

    private string GetPieceName(Pos p)
    {
        return "TrackPiece_" + p.x + "_" + p.y;
    }
}