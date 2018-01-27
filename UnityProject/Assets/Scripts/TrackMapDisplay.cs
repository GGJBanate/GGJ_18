using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackMapDisplay : MonoBehaviour {

    public GameObject canvas;

    public Sprite hexagon;

    private Dictionary<Pos, TrackData> map;

    private int top, right, bottom, left;

    // Use this for initialization
    void Start () {
        map = TrackController.Instance.map;

        Pos p;
        foreach(KeyValuePair<Pos, TrackData> entry in map)
        {
            p = entry.Key;
            if(p.x < left) left = p.x;
            if(p.x > right) right = p.x;
            if(p.y < top) top = p.y;
            if(p.y > bottom) bottom = p.y;
        }

        foreach(KeyValuePair<Pos, TrackData> entry in map)
        {
            // AddTrackPieceToCanvas(entry.Key, entry.Value, canvas);

            AddTextToCanvas("(" +entry.Key.x+ ","+entry.Key.y+ ") " + entry.Value.o, canvas, entry.Key, entry.Value.o);

        }


        Debug.Log("bounderies: " + top + " " + right + " " + bottom + left + ";" );

    }
    
    // Update is called once per frame
    void Update () {
        
    }

    public Text AddTextToCanvas(string text, GameObject canvasGameObject, Pos pos, Orientation o)
    {
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        /*
        Text text = canvasGameObject.AddComponent<Text>();
        text.text = textString;

        text.font = ArialFont;
        text.material = ArialFont.material;

        return text;
        */
        GameObject newText = new GameObject(text.Replace(" ", "-"), typeof(RectTransform));
        /*
        var newTextComp = newText.AddComponent<Text>();
        newTextComp.text = text;        
        newTextComp.font = ArialFont;
        newTextComp.material = ArialFont.material;
        newTextComp.color = Color.red;
        newTextComp.alignment = TextAnchor.MiddleCenter;
        newTextComp.fontSize = 10;
        */
        var newImageComp = newText.AddComponent<Image>();
        newImageComp.sprite = hexagon;

        newText.transform.SetParent(canvasGameObject.transform);
        newText.transform.localScale = new Vector3(1, 0.866f, 1);
        newText.transform.localPosition = new Vector3(pos.x*100, pos.y*100 + ( (pos.x+1) % 2 == 0 ? 50 : 0), 0);
        newText.transform.Rotate(Vector3.forward, ((int)o) * 60);
        return null;
    }

    public Button AddTrackPieceToCanvas(Pos pos, TrackData track, GameObject canvasGameObject)
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
/* 
public class TrackMapPart {

    public int x;
    public int y;

    public Orientation o;

    public TrackType type;

    public TrackMapPart l;
    public TrackMapPart c;
    public TrackMapPart r;

    TrackMapPart(int x, int y, Orientation orientation, TrackData part, List<TrackMapPart> map) {
        this.x = x;
        this.y = y;
        this.o = orientation;

        this.type = part.type;

        switch(this.type) {
            case TrackType.Straight:
                CreateCenter(part.track[0], map);
                break;

            case TrackType.TwoWayJunction:
                CreateLeft (part.track[0], map);
                CreateRight(part.track[1], map);
                break;

            case TrackType.ThreeWayJunction:
                CreateLeft  (part.track[0], map);
                CreateCenter(part.track[1], map);
                CreateRight (part.track[2], map);
                break;

            default:
                break;

        }

        map.Add(this);
    }

    private void CreateCenter(TrackData child, List<TrackMapPart> map) {
        int x = this.x;
        int y = this.y;

        switch(this.o) {
            case Orientations.NN: 
                y-= 1;
                break;
            case Orientations.NE:
                x+= 1;
                y-= 1;
                break;
            case Orientations.SE:
                x+= 1;
                break;
            case Orientations.SS:
                y+= 1;
                break;
            case Orientations.SW:
                x-= 1;
                break;
            case Orientations.NW:
                x-= 1;
                y-= 1;
                break;
        }

        c = new TrackMapPart(x, y, this.o, child, map);
    }


    private void CreateLeft(TrackData child, List<TrackMapPart> map) {
        int x = this.x;
        int y = this.y;

        switch((Orientations)this.o + 1 % 6) {
            case Orientations.NN: 
                y-= 1;
                break;
            case Orientations.NE:
                x+= 1;
                y-= 1;
                break;
            case Orientations.SE:
                x+= 1;
                break;
            case Orientations.SS:
                y+= 1;
                break;
            case Orientations.SW:
                x-= 1;
                break;
            case Orientations.NW:
                x-= 1;
                y-= 1;
                break;
        }

        l = new TrackMapPart(x, y, this.o, child, map);
    }


    private void CreateRight(TrackData child, List<TrackMapPart> map) {
        switch((Orientations)this.o + 5 % 6) {
            case Orientation.NN: 
                c = new TrackMapPart(this.x    , this.y - 1, child, map);
                break;
            case Orientations.NE:
                c = new TrackMapPart(this.x + 1, this.y - 1, child, map);
                break;
            case Orientations.SE:
                c = new TrackMapPart(this.x + 1, this.y    , child, map);
                break;
            case Orientations.SS:
                c = new TrackMapPart(this.x    , this.y + 1, child, map);
                break;
            case Orientations.SW:
                c = new TrackMapPart(this.x - 1, this.y    , child, map);
                break;
            case Orientations.NW:
                c = new TrackMapPart(this.x - 1, this.y - 1, child, map);
                break;
        }
    }

}

*/