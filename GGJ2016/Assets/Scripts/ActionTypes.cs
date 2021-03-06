using UnityEngine;
using System;

public enum DecisionType {
    // The regular sequence of events.
    None,
    GoToilet,
    GetToothBrush,
    BrushTeeth,
    GetMilk,
    PutMilkOnDiningTable,
    GetCereal,
    GetBowl,
    EatCereal,
    StartMakingCoffee,
    GetClothes,
    ChangeClothes,
    GetNewspaper,
    GetCoffee,
    SitOnCouch,
    GetKeysAndWallet,
    GetBriefcase,
    GetShoes,
    LeaveHouse,

    MakeBed,

    SEARCH_B,
    SEARCH_A,
    SEARCH_F,
    SEARCH_C,
    SEARCH_D,
    SEARCH_E,
}


public enum State {
    WALKING,
    STANDING,
    DOING_JOB,
}

public enum JobType
{
    NONE,
    GO_TOILET,
    BRUSH_TEETH,
    GET_MILK,
    PUT_MILK_ON_DINING_TABLE,
    GET_CEREAL,
    GET_BOWL,
    EAT_CEREAL,
    START_MAKING_COFFEE,
    GET_CLOTHES,
    CHANGE_CLOTHES,
    GET_NEWSPAPER,
    GET_COFFEE,
    SIT_ON_COUCH,
    GET_KEYS_AND_WALLET,
    GET_BRIEFCASE,
    GET_SHOES,
    LEAVE_HOUSE,

    MAKE_BED,
}