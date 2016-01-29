using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMovement : MonoBehaviour {

    [SerializeField]
    GameObject gameManager;
    GridGraph houseGrid;

    private CharacterAI ai;

    private Queue<Point> pathQueue;

    private float moveTime = 0.5f;
    private float moveStartTime;
    private float moveEndTime;

    private bool stopped;
    private bool moving;
    private int cx;
    private int cy;
    private int nextX;
    private int nextY;

	// Use this for initialization
    void Start()
    {
        houseGrid = gameManager.GetComponent<GameManager>().houseGrid;
        ai = new CharacterAI(this);
    }
	
	// Update is called once per frame
	void Update () {
	    if (moving)
	    {
	        var target = Vector2.zero;
	        houseGrid.ToRealCoordinates(nextX, nextY, out target.x, out target.y);

	        if (Time.time > moveStartTime)
	        {
	            transform.position = target;
	            OnReach();
	        }
	        else
	        {
	            var start = Vector2.zero;
	            houseGrid.ToRealCoordinates(cx, cy, out start.x, out start.y);
	            transform.position = start + (Time.time - moveStartTime)/(moveEndTime - moveStartTime)*(target - start);
	        }
	    }

	}


    private void OnObstructed()
    {

    }


    private void OnReach()
    {
        moving = false;
        cx = nextX;
        cy = nextY;
        if (pathQueue.Count <= 0)
        {
            ai.OnReachDestination(cx, cy);
            return;
        }

        GoToNextInQueue();
    }

    private void GoToNextInQueue() {
        var next = pathQueue.Dequeue();
        if (houseGrid.IsBlockedEdge(cx, cy, next.x, next.y))
        {
            ai.OnObstructed(cx, cy, next.x, next.y);
            return;
        }

        moving = true;
        nextX = next.x;
        nextY = next.y;
        moveStartTime = Time.time;
        moveEndTime = moveStartTime + moveTime;
    }
}
