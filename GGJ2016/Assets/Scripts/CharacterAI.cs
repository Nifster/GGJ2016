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
            {DecisionType.LeaveHouse, WeightLeaveHouse},
        };

        updateFunctions = new Dictionary<DecisionType, UpdateFunction>
        {
            {DecisionType.MakeBed, UpdateMakeBed},
            {DecisionType.GoToilet, UpdateGoToilet},
            {DecisionType.BrushTeeth, UpdateBrushTeeth},
            {DecisionType.LeaveHouse, UpdateLeaveHouse},
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
        if (gameVars.brushedTeeth) return -1f;
        return 0.5f;;
    }

    private float WeightMakeBed()
    {
        if (gameVars.madeBed) return -1f;
        return 0.5f;
    }

    private float WeightLeaveHouse()
    {
        return 0;
    }

    #endregion


#region Update Functions

    private void DefaultUpdateFunction(int targetX, int targetY, CharacterMovement.OnReachDestinationFunction onReach, JobType jobType, Action<GameVariables> gameVariableChanges)
    {
        switch (charState.state)
        {
            case State.WALKING:
                if (!characterMovement.TargetMatches(targetX, targetY))
                {
                    characterMovement.SetOnStepAction(() => PathFindTowards(targetX, targetY, onReach));
                }
                break;
            case State.STANDING:
                PathFindTowards(targetX, targetY, onReach);
                break;
            case State.DOING_JOB:
                if (charState.currentJobType == jobType)
                {
                    if (charState.JobDone)
                    {
                        gameVariableChanges(gameVars);
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

    private void UpdateGoToilet()
    {
        DefaultUpdateFunction(
            GameVariables.toiletBowlX, 
            GameVariables.toiletBowlY,
            OnReachGoToilet,
            JobType.GO_TOILET,
            (gameVars) =>
            {
                gameVars.usedToilet = true;
            }
        );
    }


    private void UpdateBrushTeeth()
    {
        DefaultUpdateFunction(
            GameVariables.toiletSinkX,
            GameVariables.toiletSinkY,
            OnReachBrushTeeth,
            JobType.BRUSH_TEETH,
            (gameVars) =>
            {
                gameVars.brushedTeeth = true;
            }
        );
    }

    private void UpdateMakeBed()
    {
        DefaultUpdateFunction(
            GameVariables.bedSideX,
            GameVariables.bedSideY,
            OnReachMakeBed,
            JobType.MAKE_BED,
            (gameVars) =>
            {
                gameVars.madeBed = true;
            }
        );
    }


    private void UpdateLeaveHouse()
    {
        DefaultUpdateFunction(
            GameVariables.exitX,
            GameVariables.exitY,
            OnReachLeaveHouse,
            JobType.LEAVE_HOUSE,
            (gameVars) =>
            {
                gameVars.leftHouse = true;
            }
        );
    }


#endregion


#region On Reach Functions

    private void OnReachGoToilet(int cx, int cy)
    {
        charState.StartJob(JobType.GO_TOILET, 3f);
    }

    private void OnReachBrushTeeth(int cx, int cy)
    {
        charState.StartJob(JobType.BRUSH_TEETH, 4f);
    }

    private void OnReachMakeBed(int cx, int cy)
    {
        charState.StartJob(JobType.MAKE_BED, 5f);
    }

    private void OnReachLeaveHouse(int cx, int cy)
    {
        charState.StartJob(JobType.LEAVE_HOUSE, 1f);
    }


    #endregion
}