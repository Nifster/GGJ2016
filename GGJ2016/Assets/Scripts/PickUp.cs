
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
	KeysWallet,
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
    public bool active { get; private set; }

    public int believedX { get; private set; }
    public int believedY { get; private set; }
    public bool hasBeliefInLocation { get; private set; }

    public PickUpStatus status { get; private set; }
    private readonly GridGraph houseGrid = GameManager.Instance.HouseGrid;

    public PickUp(PickUpType type, int cx, int cy, GameObject prefab_pickup, bool active=true)
    {
        this.cx = cx;
        this.cy = cy;
        believedX = cx;
        believedY = cy;
        hasBeliefInLocation = true;


        this.type = type;
        var go = MonoBehaviour.Instantiate(prefab_pickup) as GameObject;
        transform = go.transform;
        go.GetComponent<PickupSprite>().SetSprite(type);
        RefreshPosition();

        this.active = active;
        transform.GetComponent<PickupSprite>().SetVisible(active);
    }

    public void Activate()
    {
        active = true;
        transform.GetComponent<PickupSprite>().SetVisible(true);
    }

    public void Deactivate()
    {
        active = false;
        transform.GetComponent<PickupSprite>().SetVisible(false);
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
        transform.GetComponent<PickupSprite>().SetCarried(false);
        Debug.Log(cx + ", " + cy + ", " + status);
        transform.parent = null;
        RefreshPosition();
    }

    public PickUp CharacterTake(Transform t)
    {
        this.status = PickUpStatus.CharacterHeld;
        transform.GetComponent<PickupSprite>().SetCarried(true);
        transform.position = t.position;
        transform.parent = t;
        return this;
    }

    public PickUp PlayerTake(Transform t)
    {
        this.status = PickUpStatus.PlayerHeld;
        transform.GetComponent<PickupSprite>().SetCarried(true);
        transform.position = t.position;
        transform.parent = t;
        return this;
    }

    public bool CanTake()
    {
        return active && (this.status == PickUpStatus.Unheld);
    }

    public void SetBelievedLocation(int x, int y)
    {
        believedX = x;
        believedY = y;
        hasBeliefInLocation = true;
    }

    public void UpdateBelievedLocation()
    {
        believedX = cx;
        believedY = cy;
        hasBeliefInLocation = true;
    }

    public void LoseBeliefInLocation()
    {
        hasBeliefInLocation = false;
    }
}
