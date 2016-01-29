using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	[SerializeField]
	KeyCode moveLeft, moveRight, moveUp, moveDown;
	[SerializeField]
	GameObject gameManager;
	GridGraph houseGrid;
	[SerializeField]
	float movementCooldown;
	float cooldownTimer;
	// Use this for initialization
	void Start () {
		houseGrid = gameManager.GetComponent<GameManager>().houseGrid;
	}

	// Update is called once per frame
	void FixedUpdate () {

		MovementCheck();
	}

	void MovementCheck(){
		cooldownTimer -= Time.deltaTime;
		if(Input.GetKey(moveRight) && cooldownTimer < 0){
			if(!houseGrid.IsBlockedActual(this.transform.position.x,this.transform.position.y, dx:1)){
				this.transform.position = new Vector2(this.transform.position.x+1,this.transform.position.y);
				cooldownTimer = movementCooldown;
			}
		}

		if(Input.GetKey(moveLeft) && cooldownTimer < 0){
			if(!houseGrid.IsBlockedActual(this.transform.position.x,this.transform.position.y, dx:-1)){
				this.transform.position = new Vector2(this.transform.position.x-1,this.transform.position.y);
				cooldownTimer = movementCooldown;
			}
		}


		if(Input.GetKey(moveDown) && cooldownTimer < 0){
			if(!houseGrid.IsBlockedActual(this.transform.position.x,this.transform.position.y, dy:-1)){
				this.transform.position = new Vector2(this.transform.position.x,this.transform.position.y-1);
				cooldownTimer = movementCooldown;
			}
		}

		if(Input.GetKey(moveUp) && cooldownTimer < 0){
			if(!houseGrid.IsBlockedActual(this.transform.position.x,this.transform.position.y, dy:1)){
				this.transform.position = new Vector2(this.transform.position.x,this.transform.position.y+1);
				cooldownTimer = movementCooldown;
			}
		}
	}
}
