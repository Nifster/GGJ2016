using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CharacterAI {
    delegate float WeightFunction();
    delegate void UpdateFunction();

    private readonly List<PickUp> heldItems;

    private readonly GameManager gameManager;
    private readonly GameVariables gameVars;
    private readonly GridGraph houseGrid;
    private readonly CharacterState charState;
    private readonly Dictionary<DecisionType, WeightFunction> weightFunctions;
    private readonly Dictionary<DecisionType, UpdateFunction> updateFunctions;
    private readonly CharacterMovement character;
    private readonly PlayerMovement player;


    public CharacterAI(GameManager gameManager)
    {
        this.gameManager = gameManager;
        this.character = gameManager.Character;
        this.gameVars = gameManager.GameVars;
        this.houseGrid = gameManager.HouseGrid;
        this.player = gameManager.Player;
        charState = new CharacterState();
        heldItems = new List<PickUp>();

        weightFunctions = new Dictionary<DecisionType, WeightFunction>
        {
            {DecisionType.MakeBed, WeightMakeBed},
            {DecisionType.GoToilet, WeightGoToilet},        
            {DecisionType.BrushTeeth, WeightBrushTeeth},    
            {DecisionType.GetToothBrush, WeightFindToothbrush},    
            {DecisionType.LeaveHouse, WeightLeaveHouse},
        };

        updateFunctions = new Dictionary<DecisionType, UpdateFunction>
        {
            {DecisionType.MakeBed, UpdateMakeBed},
            {DecisionType.GoToilet, UpdateGoToilet},
            {DecisionType.BrushTeeth, UpdateBrushTeeth},
            {DecisionType.GetToothBrush, UpdateFindToothbrush},    
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

        UpdateSuspicion();
    }

    public void OnObstructed(int cx, int cy, int nextX, int nextY)
    {
        charState.SetState(State.STANDING);
    }

    private void PathFindTowards(int tx, int ty, Orientation targetOrientation, CharacterMovement.OnReachDestinationFunction onReach, bool tryHarder = false)
    {
        bool canFindPath = character.PathFindTowards(tx, ty, targetOrientation, onReach, tryHarder);
        if (canFindPath)
        {
            charState.SetState(State.WALKING);
        }
        else
        {
            charState.SetState(State.STANDING);
        }
    }

    private void UpdateSuspicion()
    {
        gameVars.AddSuspicion(-0.001f);
        if (CanSee(player.cx, player.cy))
        {
            if (player.IsHoldingItem())
            {
                gameVars.AddSuspicion(0.04f);
            }
        }
    }

    // "GameVariable" functions.

    private bool IsHolding(PickUpType item)
    {
        return heldItems.Exists((p) => p.type == item);
    }

    private bool HasBeliefInLocation(PickUpType item)
    {
        return gameManager.pickups[item].hasBeliefInLocation;
    }

    private void LoseBeliefInLocation(PickUpType item)
    {
        gameManager.pickups[item].LoseBeliefInLocation();
    }


    private bool CanSee(PickUpType item)
    {
        var pickup = gameManager.pickups[item];
        bool seen = CanSee(pickup.cx, pickup.cy) ||
                   CanSee(pickup.cx-1, pickup.cy) ||
                   CanSee(pickup.cx+1, pickup.cy) ||
                   CanSee(pickup.cx, pickup.cy-1) ||
                   CanSee(pickup.cx, pickup.cy+1);

        if (seen) pickup.SetBelievedLocation(pickup.cx, pickup.cy);
        return seen;
    }

    private bool CanSee(int x, int y)
    {
        var ch = character;
        return houseGrid.DirectionalLineOfSight(ch.cx, ch.cy, x, y, ch.currentOrientation);
    }

    private bool TryTakeItem(PickUpType item)
    {
        var p = gameManager.pickups[item];
        var c = character;
        if (p.CanTake())
        {
            if (p.cx == c.cx && p.cy == c.cy)
            {
                heldItems.Add(p.CharacterTake(c.transform));
                return true;
            }
            if (p.cx == c.cx && p.cy == c.cy + 1)
            {
                c.FaceDirection(Orientation.DOWN);
                heldItems.Add(p.CharacterTake(c.transform));
                return true;
            }
            if (p.cx == c.cx && p.cy == c.cy - 1)
            {
                c.FaceDirection(Orientation.UP);
                heldItems.Add(p.CharacterTake(c.transform));
                return true;
            }
            if (p.cx == c.cx - 1 && p.cy == c.cy)
            {
                c.FaceDirection(Orientation.LEFT);
                heldItems.Add(p.CharacterTake(c.transform));
                return true;
            }
            if (p.cx == c.cx + 1 && p.cy == c.cy)
            {
                c.FaceDirection(Orientation.RIGHT);
                heldItems.Add(p.CharacterTake(c.transform));
                return true;
            }
        }
        return false;
    }

    private void WalkTowards(int targetX, int targetY, bool relaxed = false)
    {
        if (relaxed)
        {
            if (Math.Abs(character.cx - targetX) + Math.Abs(character.cy - targetY) > 1)
            {
                character.SetOnStepAction(() => charState.SetState(State.STANDING));
            }
        }
        else
        {
            if (!character.TargetMatches(targetX, targetY))
            {
                character.SetOnStepAction(() => charState.SetState(State.STANDING));
            }
        }
    }

    #region Weight Functions

    private float WeightGoToilet()
    {
        if (gameVars.usedToilet) return -1f;
        return 1f;
    }

    private float WeightFindToothbrush()  // NEXT: IMPLEMENT UpdateFindToothBrush -> toothbrush finding logic  ||| and OnReachFindToothBrush() <-- pickup if within +.
    {
        if (IsHolding(PickUpType.Toothbrush)) return -100f;
        if (gameVars.brushedTeeth) return -1f;
        if (!HasBeliefInLocation(PickUpType.Toothbrush) && !CanSee(PickUpType.Toothbrush))
        {
            if (gameVars.findingToothbrushSearchedRoom) return -0.4f;
            return 0.88f;
        }

        return 0.9f;
    }


    private float WeightBrushTeeth()
    {
        if (gameVars.brushedTeeth) return -1f;
        if (!IsHolding(PickUpType.Toothbrush)) return -100f;
        return 0.85f;;
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

    private void DefaultUpdateFunction(int targetX, int targetY, Orientation targetOrientation, CharacterMovement.OnReachDestinationFunction onReach, JobType jobType, Action<GameVariables> gameVariableChanges)
    {
        switch (charState.state)
        {
            case State.WALKING:
                WalkTowards(targetX, targetY);
                break;
            case State.STANDING:
                PathFindTowards(targetX, targetY, targetOrientation, onReach);
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

    private void GrabItemUpdateFunction(int targetX, int targetY, CharacterMovement.OnReachDestinationFunction onReach)
    {
        switch (charState.state)
        {
            case State.WALKING:
                WalkTowards(targetX, targetY, true);
                break;
            case State.STANDING:
                PathFindTowards(targetX, targetY, Orientation.UP, onReach, true);
                break;
            case State.DOING_JOB:
                charState.CancelJob();
                break;
        }
    }

    private void UpdateGoToilet()
    {
        DefaultUpdateFunction(
            GameVariables.toiletBowlX, 
            GameVariables.toiletBowlY,
            Orientation.UP, 
            OnReachGoToilet,
            JobType.GO_TOILET,
            (gameVars) =>
            {
                gameVars.usedToilet = true;
            }
        );
    }

    private void UpdateFindToothbrush()
    {
        var toothbrush = gameManager.pickups[PickUpType.Toothbrush];
        if (toothbrush.hasBeliefInLocation)
        {
            GrabItemUpdateFunction(toothbrush.believedX, toothbrush.believedY, OnReachFindToothbrush);
        }
        else
        {
            GrabItemUpdateFunction(GameVariables.searchRoomX, GameVariables.searchRoomY, OnReachCannotFindToothbrush);
        }
    }

    private void UpdateBrushTeeth()
    {
        DefaultUpdateFunction(
            GameVariables.toiletSinkX,
            GameVariables.toiletSinkY,
            Orientation.UP, 
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
            Orientation.LEFT, 
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
            Orientation.DOWN, 
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

    private void OnReachFindToothbrush(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.Toothbrush);
        if (!result) LoseBeliefInLocation(PickUpType.Toothbrush);
    }

    private void OnReachCannotFindToothbrush(int cx, int cy)
    {
        gameVars.findingToothbrushSearchedRoom = true;
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