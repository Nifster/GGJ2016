using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static string debugStatusText;

    private GridGraph houseGrid;

    public GridGraph HouseGrid
    {
        get
        {
            Initialise();
            return houseGrid;
        }
    }

    [SerializeField]
	float realMinX, realMinY, realMaxX, realMaxY;
	// Use this for initialization

	public Dictionary<Point,Interactable> interactableHash = new Dictionary<Point,Interactable>();
	public Dictionary<Point,PickUpable> pickUpsHash = new Dictionary<Point,PickUpable>();


    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        if (houseGrid != null) return;
        houseGrid = new GridGraph();

        //initialise interactables
        interactableHash.Add(new Point {x = 0, y = 8}, new Interactable("bed", 0, 8));

		//initialise pickupables
		pickUpsHash.Add(new Point{x=2, y=2}, new PickUpable(PickUps.Bowl));

        //initialize empty room
        bool[,] isBlocked = new bool[10, 10];
        for (int i = 0; i < isBlocked.GetLength(0); i++)
        {
            for (int j = 0; j < isBlocked.GetLength(1); j++)
            {
                isBlocked[i, j] = false;
            }
        }

        //set interactable in positions

        isBlocked[0, 8] = true;
        isBlocked[0, 4] = true;
        isBlocked[1, 4] = true;
        isBlocked[2, 4] = true;
        isBlocked[3, 4] = true;
        isBlocked[4, 4] = true;
        isBlocked[0, 5] = true;
        isBlocked[1, 5] = true;
        isBlocked[2, 5] = true;
        isBlocked[3, 5] = true;
        isBlocked[4, 5] = true;
        isBlocked[0, 6] = true;
        isBlocked[1, 6] = true;
        isBlocked[2, 6] = true;
        isBlocked[3, 6] = true;
        isBlocked[4, 6] = true;
        isBlocked[4, 8] = true;
        isBlocked[4, 9] = true;

        houseGrid.Initialise(isBlocked, realMinX, realMinY, realMaxX - realMinX, realMaxY - realMinY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(realMinX, realMinY, 0),
            new Vector3(realMinX, realMaxY, 0));
        Gizmos.DrawLine(new Vector3(realMinX, realMinY, 0),
            new Vector3(realMaxX, realMinY, 0));
        Gizmos.DrawLine(new Vector3(realMaxX, realMaxY, 0),
            new Vector3(realMinX, realMaxY, 0));
        Gizmos.DrawLine(new Vector3(realMaxX, realMaxY, 0),
            new Vector3(realMaxX, realMinY, 0));
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), debugStatusText);
    }

    // Update is called once per frame
	void Update () {
	
	}
}
