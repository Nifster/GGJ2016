using UnityEngine;
using System;

public class GameVariables
{
    public float suspicionLevel = 0; // between 0 and 1.

    public bool findingToothbrushSearchedRoom = false;

    public bool brushedTeeth = false;
    public bool usedToilet = false;
    
    public bool madeBed = false;


    public bool isMilkOnDiningTable = false;
    public bool thinksMilkOnDiningTable = false;

    public bool milkIsMissing = false;

    public bool ateCereal = false;

    public bool isCoffeeBeingMade = false;
    public bool pastCoffeeDoneTime = false;
    public bool thinksCoffeeBeingMAde = false;
    public bool changedClothes = false;

    public bool hasReadNewsPapers = false;
    public bool hasDrankCoffee = false;

    public bool wornShoes = false;
    public bool leftHouse = false;

    public bool isLate = false;
    public bool isLateAndAwareOfIt = false;


    // Constants
    public const int toiletBowlX = 7;
    public const int toiletBowlY = 7;

    public const int toiletSinkX = 4;
    public const int toiletSinkY = 8;

    
    public const int bedSideX = 2;
    public const int bedSideY = 15;

    public const int exitX = 19;
    public const int exitY = 0;

    public const int searchRoomX = 3;
    public const int searchRoomY = 15;

    public const int diningTableX = 19;
    public const int diningTableY = 13;

    public const int diSningTableNextToX = 19;
    public const int diningTableNextToY = 14;




    // Functions
    public void AddSuspicion(float val)
    {
        suspicionLevel += val;
        suspicionLevel = Mathf.Clamp(suspicionLevel, 0, 1);
    }
}