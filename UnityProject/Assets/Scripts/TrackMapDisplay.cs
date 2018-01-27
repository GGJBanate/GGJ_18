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

    public static readonly Vector3 SCALE = new Vector3(1, 0.866f, 1);

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        
    }

    public void init(Dictionary<Pos, TrackPieceData> map) {

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
            AddTextToCanvas("TrackPiece_" +entry.Key.x+ "_"+entry.Key.y, entry.Key, entry.Value);
        }


        Debug.Log("bounderies: " + top + " " + right + " " + bottom + " " + left + "||" + offset.x + " " + offset.y );
    }

    public void AddTextToCanvas(string text, Pos pos, TrackPieceData trackPiece)
    {
        GameObject newText = new GameObject(text.Replace(" ", "-"), typeof(RectTransform));

        var newImageComp = newText.AddComponent<Image>();

        switch(trackPiece.type) {
            case TrackType.Straight: 
                newImageComp.sprite = hexagonStraight;
                break;

            case TrackType.TwoWayJunction: 
                newImageComp.sprite = hexagonTwoWayJunction;
                break;

            case TrackType.ThreeWayJunction: 
                newImageComp.sprite = hexagonThreeWayJunction;
                break;

            case TrackType.Finish: 
                newImageComp.sprite = hexagonFinish;
                break;

            case TrackType.DeadEnd: 
                newImageComp.sprite = hexagonDeadEnd;
                break;

            case TrackType.Start: 
                newImageComp.sprite = hexagonStart;
                break;

            default:
                newImageComp.sprite = hexagon;
                break;
        }

        newText.transform.SetParent(canvas.transform);
        newText.transform.localScale = SCALE;
        newText.transform.localPosition = new Vector3(
            offset.x + pos.x - 0.14f*pos.x, 
            offset.y - (pos.y - ( (pos.x+100) % 2 == 0 ? -0.5f : 0)), 
            0
        ) * 100;
        newText.transform.Rotate(Vector3.forward, ((int)trackPiece.o) * -60);
    }
}