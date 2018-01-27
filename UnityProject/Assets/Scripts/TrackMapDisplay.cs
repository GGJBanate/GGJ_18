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

    private Dictionary<Pos, TrackPieceData> map;

    private int offsetx;
    private int offsety;

    // Use this for initialization
    void Start () {

    }

    public void init(Dictionary<Pos, TrackPieceData> map) {
        this.map = map;

        int top = 0, right = 0, bottom = 0, left = 0;
        // map = new Dictionary<Pos, TrackData> (); //TrackController.Instance.map;

        Pos p;
        foreach(KeyValuePair<Pos, TrackPieceData> entry in map)
        {
            p = entry.Key;
            if(p.x < left) left = p.x;
            if(p.x > right) right = p.x;
            if(p.y < top) top = p.y;
            if(p.y > bottom) bottom = p.y;
        }

        offsetx = 0;//- (right + left);
        offsety = 0;//- (top + bottom);

        foreach(KeyValuePair<Pos, TrackPieceData> entry in map)
        {
            AddTextToCanvas("TrackPiece_" +entry.Key.x+ "_"+entry.Key.y, new Pos(entry.Key.x+offsetx, entry.Key.y+offsety), entry.Value);
        }


        Debug.Log("bounderies: " + top + " " + right + " " + bottom + " " + left + "||" + offsetx + " " + offsety );
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    public Text AddTextToCanvas(string text, Pos pos, TrackPieceData trackPiece)
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

        // newImageComp.sprite = hexagon;

        newText.transform.SetParent(canvas.transform);
        newText.transform.localScale = new Vector3(1, 0.866f, 1);
        newText.transform.localPosition = new Vector3(pos.x, -(pos.y - ( (pos.x+100) % 2 == 0 ? -0.5f : 0) ) , 0) * 100;
        newText.transform.Rotate(Vector3.forward, ((int)trackPiece.o) * -60);
        return null;
    }

    public Button AddTrackPieceToCanvas(Pos pos, TrackPieceData track, GameObject canvasGameObject)
    {
        /*
        GameObject text_Object = new GameObject("Text");
        text_Object.transform.SetParent(canvasGameObject.transform);

        float text_Object_rectSize_width = 10;
        float text_Object_rectSize_height = 10;
        float text_Object_trans_x = -75;
        float text_Object_trans_y = 75;

        RectTransform text_Object_trans = text_Object.AddComponent<RectTransform>();
        text_Object_trans.sizeDelta.Set(text_Object_rectSize_width, text_Object_rectSize_height);
        text_Object_trans.anchoredPosition3D = new Vector3(0, 0, 0);
        text_Object_trans.anchoredPosition = new Vector2(text_Object_trans_x, text_Object_trans_y);
        text_Object_trans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        text_Object_trans.localPosition.Set(0, 0, 0);

        Button piece = text_Object.AddComponent<Button>();



        return piece;
        */
        return null;
    }
}