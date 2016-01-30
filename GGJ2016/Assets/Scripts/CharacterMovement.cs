using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMovement : MonoBehaviour
{
    public delegate void OnReachDestinationFunction(int cx, int cy);

    [SerializeField]
    GameObject gameManager;
    GridGraph houseGrid;

    private CharacterAI ai;

    private Queue<Point> pathQueue;
    private int targetX; // set together with pathQueue
    private int targetY; // set together with pathQueue

    private Action onStep;
    private OnReachDestinationFunction onReachDestination;

    private float moveTime = 0.5f;
    private float moveStartTime;
    private float moveEndTime;

    private bool stopped;
    private bool moving;
    private int cx;
    private int cy;
    private int nextX;
    private int nextY;

    private void OnDrawGizmos()
    {
        if (houseGrid == null) return;
        if (moving)
        {
            Vector2 targetPos;
            houseGrid.ToRealCoordinates(targetX, targetY, out targetPos.x, out targetPos.y);
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }

    // Use this for initialization
    void Start()
    {
        houseGrid = gameManager.GetComponent<GameManager>().HouseGrid;
        ai = new CharacterAI(this);

        // Snap to nearest grid position.
        houseGrid.ToGridCoordinates(transform.position.x, transform.position.y, out cx, out cy);
        Vector2 newPosition = Vector2.zero;
        houseGrid.ToRealCoordinates(cx, cy, out newPosition.x, out newPosition.y);
        transform.position = newPosition;
    }
	
	// Update is called once per frame
	void Update () {
	    if (moving)
	    {
	        var nextPosition = Vector2.zero;
	        houseGrid.ToRealCoordinates(nextX, nextY, out nextPosition.x, out nextPosition.y);

	        if (Time.time > moveEndTime)
	        {
	            transform.position = nextPosition;
	            OnReach();
	        }
	        else
            {
	            var currPosition = Vector2.zero;
	            houseGrid.ToRealCoordinates(cx, cy, out currPosition.x, out currPosition.y);
	            transform.position = currPosition + (Time.time - moveStartTime)/(moveEndTime - moveStartTime)*(nextPosition - currPosition);
	        }
	    }

	    ai.Update();
	}

    public void SetOnStepAction(Action action)
    {
        onStep = action;
    }

    public bool PathFindTowards(int tx, int ty, OnReachDestinationFunction onReach)
    {
        var path = houseGrid.PathFind(cx, cy, tx, ty, ignoreObstructions:false);
        if (path == null)
        {
            return false;
        }
        this.onReachDestination = onReach;
        this.pathQueue = path;
        this.targetX = tx;
        this.targetY = ty;
        GoToNextInQueue();
        return true;
    }


    private void OnReach()
    {
        moving = false;
        cx = nextX;
        cy = nextY;
        
        if (pathQueue.Count <= 0)
        {
            onReachDestination(cx, cy);
            return;
        }

        if (onStep != null)
        {
            var run = onStep;
            onStep = null;
            run();
        }
        
        GoToNextInQueue();
    }

    private void GoToNextInQueue() {
        var next = pathQueue.Dequeue();
        if (houseGrid.IsBlockedEdge(cx, cy, next.x, next.y))
        {
            Debug.Log("Obstructed!");
            ai.OnObstructed(cx, cy, next.x, next.y);
            return;
        }

        moving = true;
        nextX = next.x;
        nextY = next.y;
        moveStartTime = Time.time;

        var nextPos = Vector2.zero;
        houseGrid.ToRealCoordinates(nextX, nextY, out nextPos.x, out nextPos.y);
        nextPos -= OhVec.toVector2(transform.position);

        moveEndTime = moveStartTime + moveTime*Math.Max(Math.Abs(nextPos.x/houseGrid.tileWidth), Math.Abs(nextPos.y/houseGrid.tileHeight));
    }

    public bool TargetMatches(int tx, int ty)
    {
        return targetX == tx && targetY == ty;
    }
}
