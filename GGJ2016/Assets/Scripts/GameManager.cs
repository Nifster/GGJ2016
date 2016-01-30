using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab_pickup;

    [SerializeField]
    GameObject playerObject;
    private PlayerMovement player;

    [SerializeField]
    GameObject characterObject;
    private CharacterMovement character;



    private Texture2D meterBack;
    private Rect rectBack;
    private Texture2D meterFront;
    private Rect rectFront;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public static string debugStatusText;

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

        meterBack = new Texture2D(1, 1);
        meterBack.SetPixel(0, 0, Color.gray);
        meterFront = new Texture2D(1, 1);
        meterFront.SetPixel(0, 0, Color.yellow);

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


        //initialise interactables
        InitialiseInteractables();

        //initialise pickupables
        InitialisePickupables();
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
            new PickUp(PickUpType.Toothbrush, 0, 0, prefab_pickup),
            new PickUp(PickUpType.Milk, 1, 0, prefab_pickup),
            new PickUp(PickUpType.Cereal, 2, 0, prefab_pickup),
            new PickUp(PickUpType.Bowl, 3, 0, prefab_pickup),
            new PickUp(PickUpType.Coffee, 4, 0, prefab_pickup),
            new PickUp(PickUpType.Clothes, 5, 0, prefab_pickup),
            new PickUp(PickUpType.Newspaper, 6, 0, prefab_pickup),
            new PickUp(PickUpType.Keys, 7, 0, prefab_pickup),
            new PickUp(PickUpType.Wallet, 8, 0, prefab_pickup),
            new PickUp(PickUpType.Briefcase, 9, 0, prefab_pickup),
            new PickUp(PickUpType.Shoes, 10, 0, prefab_pickup),
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

    private void GizmosDrawTile(int px, int py, Color col)
    {
        if (houseGrid == null) return;
        var vector = Vector2.zero;
        houseGrid.ToRealCoordinates(px, py, out vector.x, out vector.y);
        Gizmos.color = col;
        Gizmos.DrawSphere(vector, 0.5f);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), debugStatusText);

        float meterWidth = 140;
        float meterHeight = 30;

        rectBack.xMin = Screen.width / 2 - meterWidth / 2;
        rectBack.width = meterWidth;
        rectBack.yMin = 10;
        rectBack.height = meterHeight;

        rectFront.xMin = Screen.width / 2 - meterWidth / 2;
        rectFront.width = meterWidth * gameVars.suspicionLevel;
        rectFront.yMin = 10;
        rectFront.height = meterHeight;

        GUI.color = Color.gray;
        GUI.DrawTexture(rectBack, meterBack);
        GUI.color = Color.yellow;
        GUI.DrawTexture(rectFront, meterFront);
    }

    // Update is called once per frame
	void Update () {

    }


    #region Interact Actions

    private void InteractMakeBed()
    {
        if (gameVars.madeBed) return;
        gameVars.madeBed = true;
    }


    #endregion
}
