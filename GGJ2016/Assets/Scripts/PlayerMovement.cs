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
	Dictionary<Point,PickUp> pickUpsHash;
    private Orientation orientation;

	[SerializeField]
	float movementCooldown;
	float cooldownTimer;

	public List<Sprite> sprites = new List<Sprite>();
	// Use this for initialization
	void Start () {
		houseGrid = gameManager.GetComponent<GameManager>().HouseGrid;
		interactableHash = gameManager.GetComponent<GameManager>().interactableHash;
		pickUpsHash = gameManager.GetComponent<GameManager>().pickUpsHash;
	}

	// Update is called once per frame
	void FixedUpdate () {

		MovementCheck();
		//check if next to interactable
		var inter = FindInteractableInFront();
	    if (inter != null)
	    {
	        Debug.Log("interactable");
	    }
        var pickup = FindPickUpInFront();
        if (pickup != null)
        {
            Debug.Log("pickup");
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
				orientation = Orientation.RIGHT;
                dx += ToMovementX(orientation);
                dy += ToMovementY(orientation);
				this.GetComponent<SpriteRenderer>().sprite = sprites[0];
			}
			if(Input.GetKey(moveUp)) {
                orientation = Orientation.UP;
                dx += ToMovementX(orientation);
                dy += ToMovementY(orientation);
				this.GetComponent<SpriteRenderer>().sprite = sprites[1];
			}
			if(Input.GetKey(moveLeft)) {
                orientation = Orientation.LEFT;
                dx += ToMovementX(orientation);
                dy += ToMovementY(orientation);
				this.GetComponent<SpriteRenderer>().sprite = sprites[2];
			}
			if(Input.GetKey(moveDown)) {
                orientation = Orientation.DOWN;
                dx += ToMovementX(orientation);
                dy += ToMovementY(orientation);
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

	Interactable FindInteractableInFront()
	{
		int gx, gy;
		houseGrid.ToGridCoordinates (this.transform.position.x, this.transform.position.y, out gx, out gy);
        int dx = ToMovementX(orientation);
        int dy = ToMovementY(orientation);
	    Interactable interactable = null;
        if (interactableHash.TryGetValue(new Point(gx+dx,gy+dy), out interactable))
        {
            return interactable;
        }
	    return null;
	}

	PickUp FindPickUpInFront()
    {
        int gx, gy;
        houseGrid.ToGridCoordinates(this.transform.position.x, this.transform.position.y, out gx, out gy);
        int dx = ToMovementX(orientation);
        int dy = ToMovementY(orientation);
        PickUp pickup = null;
        if (pickUpsHash.TryGetValue(new Point(gx + dx, gy + dy), out pickup))
        {
            return pickup;
        }
        return null;
	}


  
}

