﻿
using UnityEngine;

public enum PickUpType{
	None,
	Toothbrush,
	Milk,
	Cereal,
	Bowl,
	Coffee,
	Clothes,
	Newspaper,
	Keys,
	Wallet,
	Briefcase,
	Shoes,
}

public enum PickUpStatus
{
    Unheld,
    CharacterHeld,
    PlayerHeld,
}

public class PickUp {
    private Transform transform;

    public PickUpType type { get; private set; }
    public int cx { get; private set; }
    public int cy { get; private set; }

    public int believedX { get; private set; }
    public int believedY { get; private set; }
    public bool hasBeliefInLocation { get; private set; }

    public PickUpStatus status { get; private set; }
    private readonly GridGraph houseGrid = GameManager.Instance.HouseGrid;

    public PickUp(PickUpType type, int cx, int cy, GameObject prefab_pickup)
    {
        this.cx = cx;
        this.cy = cy;
        believedX = cx;
        believedY = cy;
        hasBeliefInLocation = true;

        this.type = type;
        var go = MonoBehaviour.Instantiate(prefab_pickup) as GameObject;
        transform = go.transform;
        RefreshPosition();
    }

    private void RefreshPosition()
    {
        var pos = Vector2.zero;
        houseGrid.ToRealCoordinates(cx, cy, out pos.x, out pos.y);
        this.transform.position = pos;
    }

    public PickUpType getName()
    {
        return type;
    }

    public void Release(int cx, int cy)
    {
        this.cx = cx;
        this.cy = cy;
        this.status = PickUpStatus.Unheld;
        transform.parent = null;
        RefreshPosition();
    }

    public PickUp CharacterTake(Transform t)
    {
        this.status = PickUpStatus.CharacterHeld;
        transform.position = t.position;
        transform.parent = t;
        return this;
    }

    public PickUp PlayerTake(Transform t)
    {
        this.status = PickUpStatus.PlayerHeld;
        transform.position = t.position;
        transform.parent = t;
        return this;
    }

    public bool CanTake()
    {
        return this.status == PickUpStatus.Unheld;
    }

    public void SetBelievedLocation(int x, int y)
    {
        believedX = x;
        believedY = y;
        hasBeliefInLocation = true;
    }

    public void LoseBeliefInLocation()
    {
        hasBeliefInLocation = false;
    }
}
