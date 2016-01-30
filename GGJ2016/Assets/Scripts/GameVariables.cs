using UnityEngine;
using System;

public class GameVariables
{
    public float suspicionLevel = 0; // between 0 and 1.


    public bool brushedTeeth = false;
    public bool usedToilet = false;
    
    public bool madeBed = false;

    public bool isHoldingMilk = false;
    public bool isMilkOnDiningTable = false;
    public bool thinksMilkOnDiningTable = false;

    public bool isHoldingCereal = false;
    public bool isHoldingBowl = false;
    public bool madeCerealOnTable = false;
    public bool ateCereal = false;

    public bool isCoffeeBeingMade = false;
    public bool thinksCoffeeBeingMAde = false;
    public bool isHoldingClothes = false;
    public bool changedClothes = false;

    public bool isHoldingNewspaper = false;
    public bool isHoldingCoffee = false;

    public bool hasReadNewsPapers = false;
    public bool hasDrankCoffee = false;

    public bool isHoldingKeysAndWallet = false;
    public bool isHoldingBriefcase = false;

    public bool wornShoes = false;
    public bool leftHouse = false;



    // Constants
    public const int toiletBowlX = 5;
    public const int toiletBowlY = 2;

    public const int toiletSinkX = 8;
    public const int toiletSinkY = 7;

    public const int bedSideX = 1;
    public const int bedSideY = 8;

    public const int exitX = 9;
    public const int exitY = 0;


}