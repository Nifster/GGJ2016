using UnityEngine;
using System.Collections;


public class GameManager : MonoBehaviour {

	public GridGraph houseGrid = new GridGraph();

	[SerializeField]
	float realMinX, realMinY, realMaxX, realMaxY;
	// Use this for initialization
	void Start () {
		bool[,] isBlocked = new bool[10,10];
		for(int i=0; i<isBlocked.GetLength(0);i++){
			for(int j=0; j< isBlocked.GetLength(1);j++){
				isBlocked[i,j] = false;
			}
		}
	
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
