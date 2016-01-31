using UnityEngine;
using System.Collections;

public class PickupSprite : MonoBehaviour {

    [SerializeField] private Sprite[] sprites;
    private Transform childTransform;
    private Transform thisTransform;

    private float loop = 2f;
    private float startTime;

    private float yDiff = 0.15f;
    private float xDiff = 0.05f;

    private Vector3 carriedVec = new Vector3(0.1f, 0.6f, 0);
    private Vector3 dispVec = Vector3.zero;


    private void Start()
    {
        thisTransform = transform;
        childTransform = transform.GetChild(0);
        startTime = Time.time;
        SetCarried(false);
    }

    private void Update()
    {
        float d = 2*((Time.time - startTime)%loop) / loop;
        if (d > 1) d = (2 - d);
        d -= 0.5f;

        float yValue = yDiff*d;
        float xValue = xDiff*d;

        childTransform.position = thisTransform.position + new Vector3(xValue, yValue, 0) + dispVec;
    }

    public void SetCarried(bool value)
    {
        if (value)
        {
            dispVec = carriedVec;
        }
        else
        {
            dispVec = Vector3.zero;
        }
    }

    public void SetVisible(bool value)
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = value;
    }

    void SetSpriteIndex(int index)
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprites[index];
    }

    public void SetSprite(PickUpType type)
    {
        SetSpriteIndex(SpriteIndex(type));
    }

    int SpriteIndex(PickUpType type)
    {
        switch (type)
        {
            case PickUpType.None: return 0;
            case PickUpType.Toothbrush: return 1;
            case PickUpType.Milk: return 2;
            case PickUpType.Cereal: return 3;
            case PickUpType.Bowl: return 4;
            case PickUpType.Coffee: return 5;
            case PickUpType.Clothes: return 6;
            case PickUpType.Newspaper: return 7;
            case PickUpType.KeysWallet: return 8;
            case PickUpType.Briefcase: return 9;
            case PickUpType.Shoes: return 10;
        }
        return -1;
    }
}
