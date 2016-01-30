using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

	[SerializeField]
	KeyCode moveLeft, moveRight, moveUp, moveDown;
	[SerializeField]
	GameObject gameManager;
	GridGraph houseGrid;
	Dictionary<Point, Interactable> interactableHash;

	[SerializeField]
	float movementCooldown;
	float cooldownTimer;

	public List<Sprite> sprites = new List<Sprite>();
	// Use this for initialization
	void Start () {
		houseGrid = gameManager.GetComponent<GameManager>().HouseGrid;
		interactableHash = gameManager.GetComponent<GameManager>().interactableHash;
	}

	// Update is called once per frame
	void FixedUpdate () {

		MovementCheck();
		//check if next to interactable
		int gx, gy;
		houseGrid.ToGridCoordinates(this.transform.position.x,this.transform.position.y,out gx,out gy);
		//Debug.Log("GX: "+gx);
		//Debug.Log("GY: "+gy);
		if(interactableHash.ContainsKey(new Point{x=gx+1,y=gy}) && this.GetComponent<SpriteRenderer>().sprite == sprites[0]){
			Debug.Log("Interactable found GX:" + gx +" GY: " +gy);
		}
		if(interactableHash.ContainsKey(new Point{x=gx,y=gy+1}) && this.GetComponent<SpriteRenderer>().sprite == sprites[1]){
			Debug.Log("Interactable found GX:" + gx +" GY: " +gy);
		}
		if(interactableHash.ContainsKey(new Point{x=gx-1,y=gy}) && this.GetComponent<SpriteRenderer>().sprite == sprites[2]){
			Debug.Log("Interactable found GX:" + gx +" GY: " +gy);
		}
		if(interactableHash.ContainsKey(new Point{x=gx,y=gy-1}) && this.GetComponent<SpriteRenderer>().sprite == sprites[3]){
			Debug.Log("Interactable found GX:" + gx +" GY: " +gy);
		}

			
	}


	int ToMovementX(Orientation dir) {
		switch(dir) {
		case Orientation.DOWN: return 0;
		case Orientation.UP: return 0;
		case Orientation.LEFT: return -1;
		case Orientation.RIGHT: return 1;
		default: return 0;
		}

	}

	int ToMovementY(Orientation dir) {
		switch(dir) {
		case Orientation.DOWN: return -1;
		case Orientation.UP: return 1;
		case Orientation.LEFT: return 0;
		case Orientation.RIGHT: return 0;
		default: return 0;
		}
	}

	void MovementCheck(){
		cooldownTimer -= Time.deltaTime;

		int dx =0, dy = 0;
		if (cooldownTimer < 0) {
			if(Input.GetKey(moveRight)) {
				var dir = Orientation.RIGHT;
				dx += ToMovementX(dir);
				dy += ToMovementY(dir);
				this.GetComponent<SpriteRenderer>().sprite = sprites[0];
			}
			if(Input.GetKey(moveUp)) {
				var dir = Orientation.UP;
				dx += ToMovementX(dir);
				dy += ToMovementY(dir);
				this.GetComponent<SpriteRenderer>().sprite = sprites[1];
			}
			if(Input.GetKey(moveLeft)) {
				var dir = Orientation.LEFT;
				dx += ToMovementX(dir);
				dy += ToMovementY(dir);
				this.GetComponent<SpriteRenderer>().sprite = sprites[2];
			}
			if(Input.GetKey(moveDown)) {
				var dir = Orientation.DOWN;
				dx += ToMovementX(dir);
				dy += ToMovementY(dir);
				this.GetComponent<SpriteRenderer>().sprite = sprites[3];
			}
		}

		if (dx != 0 || dy != 0) {
			if(!houseGrid.IsBlockedActual(this.transform.position.x,this.transform.position.y, dx:dx, dy:dy)) {
				
				this.transform.position = new Vector2(this.transform.position.x+dx,this.transform.position.y+dy);
				cooldownTimer = movementCooldown;
			}
		}
	}
}
