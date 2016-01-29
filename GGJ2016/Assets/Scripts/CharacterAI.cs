using UnityEngine;
using System;
using System.Collections.Generic;

public class CharacterAI {
    delegate float WeightFunction();
    delegate void UpdateFunction();

    private GameVariables gameVars;
    private CharacterState charState;

    private Dictionary<DecisionType, WeightFunction> weightFunctions;
    private Dictionary<DecisionType, UpdateFunction> updateFunctions;

    private void Initialise()
    {
        weightFunctions = new Dictionary<DecisionType, WeightFunction>
        {
            {DecisionType.MakeBed, WeightMakeBed},
            {DecisionType.GoToilet, WeightGoToilet},
            {DecisionType.BrushTeeth, WeightBrushTeeth},
        };

        updateFunctions = new Dictionary<DecisionType, UpdateFunction>
        {
            {DecisionType.MakeBed, UpdateMakeBed},
            {DecisionType.GoToilet, UpdateGoToilet},
            {DecisionType.BrushTeeth, UpdateBrushTeeth},
        };
    }


    private void ChangeState(CharacterState state)
    {

    }

    public DecisionType MakeDecision()
    {
        float maxWeight = float.NegativeInfinity;
        DecisionType decision = DecisionType.None;
        foreach (DecisionType decType in Enum.GetValues(typeof(DecisionType)))
        {
            if (!weightFunctions.ContainsKey(decType))
            {
                Debug.Log("Decision Type " + decType + " not found in weight dictionary");
                continue;
            }
            float weight = weightFunctions[decType]();
            if (weight > maxWeight)
            {
                maxWeight = weight;
                decision = decType;
            }
        }
        return decision;
    }

    public void Update()
    {
        var decision = MakeDecision();

        if (updateFunctions.ContainsKey(decision))
        {
            updateFunctions[decision]();
        }
        else
        {
            Debug.Log("Decision Type " + decision + " not found in update dictionary");
        }
    }


#region Weight Functions

    private float WeightGoToilet()
    {

        return 1f;
    }

    private float WeightBrushTeeth()
    {

        return 0.5f;;
    }

    private float WeightMakeBed()
    {

        return 0.5f;
    }

    #endregion


#region Update Functions

    private void UpdateGoToilet()
    {

    }

    private void UpdateBrushTeeth()
    {

    }

    private void UpdateMakeBed()
    {

    }


#endregion
}