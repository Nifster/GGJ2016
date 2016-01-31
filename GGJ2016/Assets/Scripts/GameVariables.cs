using UnityEngine;
using System;

public class GameVariables
{
    public float startTime;

    public float minutesPassed
    {
        get
        {
            float t = Time.time - startTime;
            return t/3;
        }
    }

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
    public float coffeeFinishTime = float.PositiveInfinity;
    public bool finishedMakingCoffee = false;
    
    public bool changedClothes = false;

    public bool hasReadNewsPapers = false;
    public bool hasDrankCoffee = false;

    public bool wornShoes = false;
    public bool leftHouse = false;

    public bool isLate = false;
    public bool isLateAndAwareOfIt = false;


    public bool searchedA = false;
    public bool searchedB = false;
    public bool searchedC = false;
    public bool searchedD = false;
    public bool searchedE = false;
    public bool searchedF = false;


    // Constants
    public const int toiletBowlX = 7;
    public const int toiletBowlY = 7;

    public const int toiletSinkX = 4;
    public const int toiletSinkY = 8;

    
    public const int bedSideX = 2;
    public const int bedSideY = 16;

    public const int exitX = 19;
    public const int exitY = 0;

    public const int searchRoomX = 3;
    public const int searchRoomY = 15;

    public const int diningTableX = 19;
    public const int diningTableY = 13;

    public const int diningTableNextToX = 19;
    public const int diningTableNextToY = 14;

    public const int coffeeMachineX = 11;
    public const int coffeeMachineY = 5;

    public const int wardrobeX = 6;
    public const int wardrobeY = 16;

    public const int changeLocationX = 7;
    public const int changeLocationY = 16;

    public const int couchX = 12;
    public const int couchY = 13;

    public const int searchAX = 9;
    public const int searchAY = 4;

    public const int searchBX = 2;
    public const int searchBY = 16;

    public const int searchCX = 18;
    public const int searchCY = 1;

    public const int searchDX = 11;
    public const int searchDY = 11;

    public const int searchEX = 9;
    public const int searchEY = 9;

    public const int searchFX = 1;
    public const int searchFY = 7;


    public const float coffeeMakeTime = 10f;

    // Functions
    public void AddSuspicion(float val)
    {
        suspicionLevel += val;
        suspicionLevel = Mathf.Clamp(suspicionLevel, 0, 1);
    }



    public GameVariables()
    {
        startTime = Time.time;
    }
}