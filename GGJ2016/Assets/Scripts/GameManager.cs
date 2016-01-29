using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {

	public GridGraph houseGrid = new GridGraph();

	[SerializeField]
	float realMinX, realMinY, realMaxX, realMaxY;
	// Use this for initialization

	[SerializeField]
	//public List<Interactable> interactablesList = new List<Interactable>();
	public Dictionary<Point,Interactable> interactableHash = new Dictionary<Point,Interactable>();
	void Start () {
		
		//interactablesList.Add(new Interactable("bed",0,8));
		interactableHash.Add(new Point{x=0,y=8},new Interactable("bed",0,8));

		//initialize empty room
		bool[,] isBlocked = new bool[10,10];
		for(int i=0; i<isBlocked.GetLength(0);i++){
			for(int j=0; j< isBlocked.GetLength(1);j++){
				isBlocked[i,j] = false;
			}
		}

		//set interactable in positions

		isBlocked[0,8] = true;
		isBlocked[0,4] = true;
		isBlocked[1,4] = true;
		isBlocked[2,4] = true;
		isBlocked[3,4] = true;
		isBlocked[4,4] = true;
		isBlocked[0,5] = true;
		isBlocked[1,5] = true;
		isBlocked[2,5] = true;
		isBlocked[3,5] = true;
		isBlocked[4,5] = true;
		isBlocked[0,6] = true;
		isBlocked[1,6] = true;
		isBlocked[2,6] = true;
		isBlocked[3,6] = true;
		isBlocked[4,6] = true;
		isBlocked[4,8] = true;
		isBlocked[4,9] = true;
	
		houseGrid.Initialise(isBlocked,realMinX,realMinY,realMaxX-realMinX,realMaxY-realMinY);
	}

	void OnDrawGizmos() {
		Gizmos.DrawLine (new Vector3(realMinX, realMinY, 0),
			new Vector3(realMinX, realMaxY, 0));
		Gizmos.DrawLine (new Vector3(realMinX, realMinY, 0),
			new Vector3(realMaxX, realMinY, 0));
		Gizmos.DrawLine (new Vector3(realMaxX, realMaxY, 0),
			new Vector3(realMinX, realMaxY, 0));
		Gizmos.DrawLine (new Vector3(realMaxX, realMaxY, 0),
			new Vector3(realMaxX, realMinY, 0));
	}

	// Update is called once per frame
	void Update () {
	
	}
}
