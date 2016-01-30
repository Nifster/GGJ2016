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

	[SerializeField]
	TextAsset floorPlan;

	[SerializeField]
	GameObject floorPrefab;
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

        //set interactable in positions

		var testBool = new int[23,20] {
			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,0,0,0,1,1,1,1,0,0,0,1,1,1,1},
			{1,1,1,1,1,1,0,0,0,1,1,1,1,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,0,0,0,1,1,1,1,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,0,0,0,1,1,1,0,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,0,0,0,1,1,1,0,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,0,0,0,1,1,1,0,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,1,1,1},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,1,1,1},
			{1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},

		};

		var initBool = new bool[23,20];
		for(int i = 0; i<23;i++){
			for(int j=0; j<20;j++){
				if(testBool[i,j] == 1){
					initBool[i,j] = true;
				}else{
					initBool[i,j] = false;
				}
			}
		}
			
		houseGrid.Initialise(initBool, realMinX, realMinY, realMaxX - realMinX, realMaxY - realMinY);

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


        /*if (houseGrid != null)
        {
            int index = ((int)(Time.time * 1.25f) + 15) % (houseGrid.sizeX * houseGrid.sizeY);
            var ori = (Orientation)(((int) (Time.time*5f) + 60)%4);
            int px = index%houseGrid.sizeY;
            int py = index/houseGrid.sizeX;

            GizmosDrawTile(px, py, Color.red);
            for (int y = 0; y < houseGrid.sizeY; ++y)
            {
                for (int x = 0; x < houseGrid.sizeX; ++x)
                {
                    if (x == px && y == py) continue;
                    if (houseGrid.DirectionalLineOfSight(px, py, x, y, ori))
                    {
                        GizmosDrawTile(x, y, Color.green);
                    }
                }
            }
        }*/

    }

	private void GizmosDrawTile(int px, int py, Color col, GridGraph ggraph)
	{
		var vector = Vector2.zero;
		ggraph.ToRealCoordinates(px, py, out vector.x, out vector.y);
		Gizmos.color = col;
		Gizmos.DrawSphere(vector, 0.5f);
	}

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), debugStatusText);
    }

    // Update is called once per frame
	void Update () {
	
	}
}
