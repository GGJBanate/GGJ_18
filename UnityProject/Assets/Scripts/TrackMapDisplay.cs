using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMapDisplay : MonoBehaviour {

	public TrackData root;

	// public List<TrackMapPart> map;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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