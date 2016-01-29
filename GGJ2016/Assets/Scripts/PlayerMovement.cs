using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	[SerializeField]
	KeyCode moveLeft, moveRight, moveUp, moveDown;
	[SerializeField]
	GameObject gameManager;
	GridGraph houseGrid;
	// Use this for initialization
	void Start () {
		houseGrid = gameManager.GetComponent<GameManager>().houseGrid;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(moveRight)){
			if(!houseGrid.IsBlockedActual(this.transform.position.x+1,this.transform.position.y+1)){
				
			}
		}
	}
}
