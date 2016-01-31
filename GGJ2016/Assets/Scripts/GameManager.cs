using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class GameManager : MonoBehaviour
{
    private const bool DEBUG_MODE = true;
    public static string debugStatusText;
    public static string debugWeightsText;


    [SerializeField] private GameObject prefab_pickup;

    [SerializeField]
    GameObject playerObject;
    private PlayerMovement player;

    [SerializeField]
    GameObject characterObject;
    private CharacterMovement character;

    [SerializeField] private GameObject objectiveTextObject;
    private TextObject objectiveText;

    [SerializeField] private GameObject scene_unbed;





    private BlackFilter splashScreen;

    private Texture2D meterBack;
    private Rect rectBack;
    private Texture2D meterFront;
    private Rect rectFront;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        Global.Initialise();
    }


    private GridGraph houseGrid;
    private GameVariables gameVars;

    public GridGraph HouseGrid
    {
        get
        {
            Initialise();
            return houseGrid;
        }
    }

    public GameVariables GameVars
    {
        get
        {
            Initialise();
            return gameVars;
        }
    }

    public PlayerMovement Player
    {
        get
        {
            Initialise();
            return player;
        }
    }

    public CharacterMovement Character
    {
        get
        {
            Initialise();
            return character;
        }
    }

    [SerializeField]
	float realMinX, realMinY, realMaxX, realMaxY;
	// Use this for initialization

    public Dictionary<InteractableType, Interactable> interactables = new Dictionary<InteractableType, Interactable>();
    public Dictionary<PickUpType, PickUp> pickups = new Dictionary<PickUpType, PickUp>();


    private void Start()
    {

        Initialise();
    }

    private void Initialise()
    {
        if (houseGrid != null) return;
        houseGrid = new GridGraph();
        gameVars = new GameVariables();
        character = characterObject.GetComponent<CharacterMovement>();
        player = playerObject.GetComponent<PlayerMovement>();
        splashScreen = this.GetComponent<BlackFilter>();
        objectiveText = objectiveTextObject.GetComponent<TextObject>();

        meterBack = new Texture2D(1, 1);
        meterBack.SetPixel(0, 0, Color.gray);
        meterFront = new Texture2D(1, 1);
        meterFront.SetPixel(0, 0, Color.yellow);

        //set interactable in positions


		var testBool = new int[23,20] {
			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1,0,0,1,1,1,1,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,1,0,0,1,1,1,1,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,1,0,0,1,1,1,1,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,1,0,0,1,1,1,0,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,1,0,0,1,1,1,0,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,1,0,0,1,1,1,0,0,0,0,0,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,1,0,0,0,1,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,1,0,0,0,1,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,1,0,0,0,1,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,1,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,1,1},
			{0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,1,1},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
			{1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
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




        //initialise interactables
        InitialiseInteractables();

        //initialise pickupables
        InitialisePickupables();

        InitialiseObjective();
    }

    private void InitialiseInteractables()
    {
        interactables = new Interactable[]
        {
            new Interactable(0, 8, InteractableType.BED, InteractMakeBed),
        }.ToDictionary(i => i.type);
    }

    private void InitialisePickupables()
    {
        pickups = new PickUp[]
        {
            new PickUp(PickUpType.Toothbrush, 4, 9, prefab_pickup),
            new PickUp(PickUpType.Milk, 10, 6, prefab_pickup),
            new PickUp(PickUpType.Cereal, 10, 3, prefab_pickup),
            new PickUp(PickUpType.Bowl, 15, 6, prefab_pickup),
            new PickUp(PickUpType.Coffee, 11, 6, prefab_pickup, active:false),
            new PickUp(PickUpType.Clothes, 6, 17, prefab_pickup),
            new PickUp(PickUpType.Newspaper, 19, 1, prefab_pickup),
            new PickUp(PickUpType.KeysWallet, 15, 3, prefab_pickup),
            new PickUp(PickUpType.Briefcase, 20, 12, prefab_pickup),
            new PickUp(PickUpType.Shoes, 21, 3, prefab_pickup),
        }.ToDictionary(p => p.type);
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
        if (DEBUG_MODE)
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width-100, 0, 100, 100), debugStatusText);
            GUI.color = Color.yellow;
            GUI.Label(new Rect(0, 0, 200, Screen.height), debugWeightsText);
        }

        Initialise();
        if (splashScreen.animating) return;
        
        GUI.color = Color.white;

        float meterWidth = Screen.width*0.4f;
        float meterHeight = Screen.height*0.05f;
        float position = Screen.width*0.5f;
        float yPad = Screen.height*0.018f;

        rectBack.xMin = position - meterWidth / 2;
        rectBack.width = meterWidth;
        rectBack.yMin = yPad;
        rectBack.height = meterHeight;

        rectFront.xMin = position - meterWidth / 2;
        rectFront.width = meterWidth * gameVars.suspicionLevel;
        rectFront.yMin = yPad;
        rectFront.height = meterHeight;

        GUI.color = Color.gray;
        GUI.DrawTexture(rectBack, meterBack);
        GUI.color = Color.yellow;
        GUI.DrawTexture(rectFront, meterFront);
    }

    // Update is called once per frame
	void Update () {
	    if (DEBUG_MODE && !splashScreen.animating)
	    {
	        if (Input.GetKey(KeyCode.LeftShift))
	        {
	            Time.timeScale = 40.0f;
	        }
	        else
	        {
                Time.timeScale = 1.0f;
	        }
	    }

	    if (gameVars.suspicionLevel > 0.9999f)
        {
            if (!splashScreen.animating)
            {
                splashScreen.FadeOut(true);
            }
	    }

	    if (gameVars.leftHouse)
	    {
	        if (!splashScreen.animating)
	        {
	            splashScreen.FadeOut(false);
	        }
	    }
	}

    private void InitialiseObjective()
    {
        SetObjective("Get Pierce to make the bed!");
    }

    public void SetObjective(string objective)
    {
        objectiveText.UpdateString("Goal\n" + objective);
    }


    public void SetBedMadeSprite()
    {
        scene_unbed.GetComponent<SpriteRenderer>().enabled = false;
    }

    #region Interact Actions

    private void InteractMakeBed()
    {
        if (gameVars.madeBed) return;
        gameVars.madeBed = true;
        SetBedMadeSprite();
    }


    #endregion
}
