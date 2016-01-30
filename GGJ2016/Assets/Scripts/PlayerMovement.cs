using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMovement : MonoBehaviour {

	[SerializeField]
	KeyCode moveLeft, moveRight, moveUp, moveDown;
	[SerializeField]
	GameObject gameManager;
	GridGraph houseGrid;
	List<Interactable> interactables;
	List<PickUp> pickups;
    private PickUp currentlyHeldPickUp;

    public Orientation orientation { get; private set; }
    public int cx { get; private set; }
    public int cy { get; private set; }

    [SerializeField]
	float movementCooldown;
	float cooldownTimer;

	public List<Sprite> sprites = new List<Sprite>();
	// Use this for initialization
	void Start () {
		houseGrid = gameManager.GetComponent<GameManager>().HouseGrid;
        interactables = gameManager.GetComponent<GameManager>().interactables;
        pickups = gameManager.GetComponent<GameManager>().pickups;


        // Snap to nearest grid position.
        int x, y;
        houseGrid.ToGridCoordinates(transform.position.x, transform.position.y, out x, out y);
        cx = x;
        cy = y;
        Vector2 newPosition = Vector2.zero;
        houseGrid.ToRealCoordinates(cx, cy, out newPosition.x, out newPosition.y);
        transform.position = newPosition;
	}

	// Update is called once per frame
    private void FixedUpdate()
    {
        MovementCheck();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryInteract();
        }	
	}

    private void TryInteract()
    {
        if (currentlyHeldPickUp != null)
        {
            // Drop currently held pickup
            int tx, ty;
            houseGrid.ToGridCoordinates(transform.position.x, transform.position.y, out tx, out ty);
            tx += ToMovementX(orientation);
            ty += ToMovementY(orientation);
            if (!houseGrid.IsBlocked(tx, ty))
            {
                currentlyHeldPickUp.Release(tx, ty);
                currentlyHeldPickUp = null;
            }
            else
            {
                Debug.Log("CANNOT DROP");
            }
        }
        else
        {
            // Pickup Pickup.
            var pickup = FindPickUpInFront();
            if (pickup != null && pickup.CanTake())
            {
                currentlyHeldPickUp = pickup.PlayerTake(transform);
                return;
            }
            var interactable = FindInteractableInFront();
            if (interactable != null)
            {
                interactable.Interact();
                return;
            }
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
			if(!houseGrid.IsBlocked(cx+dx,cy+dy))
			{
			    cx += dx;
			    cy += dy;
			    var newPosition = Vector2.zero;
			    houseGrid.ToRealCoordinates(cx, cy, out newPosition.x, out newPosition.y);
			    this.transform.position = newPosition;

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

	    return interactables.FirstOrDefault(interactable => interactable.gx == gx + dx && interactable.gy == gy + dy);
	}

	PickUp FindPickUpInFront()
    {
        int gx, gy;
        houseGrid.ToGridCoordinates(this.transform.position.x, this.transform.position.y, out gx, out gy);
        int dx = ToMovementX(orientation);
        int dy = ToMovementY(orientation);

        return pickups.FirstOrDefault(pickup => pickup.cx == gx + dx && pickup.cy == gy + dy);
	}

    public bool IsHoldingItem()
    {
        return currentlyHeldPickUp != null;
    }

}

