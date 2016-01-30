using UnityEngine;
using System;
using System.Collections.Generic;

public class CharacterAI {
    delegate float WeightFunction();
    delegate void UpdateFunction();

    private readonly GameVariables gameVars;
    private readonly CharacterState charState;
    private readonly Dictionary<DecisionType, WeightFunction> weightFunctions;
    private readonly Dictionary<DecisionType, UpdateFunction> updateFunctions;
    private readonly CharacterMovement characterMovement;


    public CharacterAI(CharacterMovement characterMovement)
    {
        this.characterMovement = characterMovement;
        gameVars = new GameVariables();
        charState = new CharacterState();

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


        // Debug check
        foreach (DecisionType decType in Enum.GetValues(typeof (DecisionType)))
        {
            if (!weightFunctions.ContainsKey(decType))
            {
                Debug.Log("Decision Type " + decType + " not found in weight dictionary");
            }
            if (!updateFunctions.ContainsKey(decType))
            {
                Debug.Log("Decision Type " + decType + " not found in weight dictionary");
            }
        }
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
            if (!weightFunctions.ContainsKey(decType)) continue;
            float weight = weightFunctions[decType]();
            if (weight > maxWeight)
            {
                maxWeight = weight;
                decision = decType;
            }
        }
        GameManager.debugStatusText = decision.ToString() + " / " + charState.state + " / " + charState.currentJobType + 
            ((charState.currentJobType == JobType.NONE) ? "" : " (" + (charState.ProgressPercent*100) + "%)");
        return decision;
    }

    public void Update()
    {
        var decision = MakeDecision();

        if (updateFunctions.ContainsKey(decision))
        {
            updateFunctions[decision]();
        }
    }

    public void OnObstructed(int cx, int cy, int nextX, int nextY)
    {
        charState.SetState(State.STANDING);
    }

    private void PathFindTowards(int tx, int ty, CharacterMovement.OnReachDestinationFunction onReach)
    {
        bool canFindPath = characterMovement.PathFindTowards(tx, ty, onReach);
        if (canFindPath)
        {
            charState.SetState(State.WALKING);
        }
        else
        {
            charState.SetState(State.STANDING);
        }
    }

    #region Weight Functions

    private float WeightGoToilet()
    {
        if (gameVars.usedToilet) return -1f;
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
        switch (charState.state)
        {
            case State.WALKING:
                if (!characterMovement.TargetMatches(GameVariables.toiletX, GameVariables.toiletY))
                {
                    characterMovement.SetOnStepAction(() => PathFindTowards(GameVariables.toiletX, GameVariables.toiletY, OnReachGoToilet));
                }
                break;
            case State.STANDING:
                PathFindTowards(GameVariables.toiletX, GameVariables.toiletY, OnReachGoToilet);
                break;
            case State.DOING_JOB:
                if (charState.currentJobType == JobType.GO_TOILET)
                {
                    if (charState.JobDone)
                    {
                        gameVars.usedToilet = true;
                        charState.SetState(State.STANDING);
                    }
                }
                else
                {
                    charState.CancelJob();
                }

                break;
        }
    }

    private void UpdateBrushTeeth()
    {

    }

    private void UpdateMakeBed()
    {

    }


#endregion


#region On Reach Functions

    private void OnReachGoToilet(int cx, int cy)
    {
        charState.StartJob(JobType.GO_TOILET, 5f);
    }

    private void OnReachBrushTeeth(int cx, int cy)
    {

    }

    private void OnReachMakeBed(int cx, int cy)
    {

    }


    #endregion
}