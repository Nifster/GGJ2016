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

            {DecisionType.GetMilk, WeightGetMilk},
            {DecisionType.PutMilkOnDiningTable, WeightPutMilkOnDiningTable},
            {DecisionType.GetCereal, WeightGetCereal},
            {DecisionType.GetBowl, WeightGetBowl},
            {DecisionType.EatCereal, WeightEatCereal},
            
            {DecisionType.StartMakingCoffee, WeightStartMakingCoffee},
            {DecisionType.GetClothes, WeightGetClothes},
            {DecisionType.ChangeClothes, WeightChangeClothes},
            {DecisionType.GetNewspaper, WeightGetNewspaper},
            {DecisionType.GetCoffee, WeightGetCoffee},
            {DecisionType.SitOnCouch, WeightSitOnCouch},
            {DecisionType.GetKeysAndWallet, WeightGetKeysAndWallet},
            {DecisionType.GetBriefcase, WeightGetBriefcase},
            {DecisionType.GetShoes, WeightGetShoes},

            {DecisionType.LeaveHouse, WeightLeaveHouse},
        };

        updateFunctions = new Dictionary<DecisionType, UpdateFunction>
        {
            {DecisionType.MakeBed, UpdateMakeBed},
            {DecisionType.GoToilet, UpdateGoToilet},
            {DecisionType.BrushTeeth, UpdateBrushTeeth},
            {DecisionType.GetToothBrush, UpdateFindToothbrush},    

            {DecisionType.GetMilk, UpdateGetMilk},
            {DecisionType.PutMilkOnDiningTable, UpdatePutMilkOnDiningTable},
            {DecisionType.GetCereal, UpdateGetCereal},
            {DecisionType.GetBowl, UpdateGetBowl},
            {DecisionType.EatCereal, UpdateEatCereal},
            
            {DecisionType.StartMakingCoffee, UpdateStartMakingCoffee},
            {DecisionType.GetClothes, UpdateGetClothes},
            {DecisionType.ChangeClothes, UpdateChangeClothes},
            {DecisionType.GetNewspaper, UpdateGetNewspaper},
            {DecisionType.GetCoffee, UpdateGetCoffee},
            {DecisionType.SitOnCouch, UpdateSitOnCouch},
            {DecisionType.GetKeysAndWallet, UpdateGetKeysAndWallet},
            {DecisionType.GetBriefcase, UpdateGetBriefcase},
            {DecisionType.GetShoes, UpdateGetShoes},

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
        var debugStatusTextList = new List<string>();

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

            debugStatusTextList.Add(decType + " : " + weight);
        }

        GameManager.debugWeightsText = string.Join("\n", debugStatusTextList.ToArray());
        GameManager.debugStatusText = decision.ToString() + " / " + charState.state + " / " + charState.currentJobType + 
            ((charState.currentJobType == JobType.NONE) ? "" : " (" + (charState.ProgressPercent*100) + "%)");
        return decision;
    }

    public void Update()
    {
        if (gameVars.isCoffeeBeingMade && !gameVars.finishedMakingCoffee)
        {
            if (Time.time > gameVars.coffeeFinishTime)
            {
                gameVars.finishedMakingCoffee = true;
                gameManager.pickups[PickUpType.Coffee].Activate();
            }
        }



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
        gameVars.AddSuspicion(-0.00001f);
        if (CanSee(player.cx, player.cy))
        {
            if (player.IsHoldingItem())
            {
                gameVars.AddSuspicion(0.02f);
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
        return gameManager.pickups[item].hasBeliefInLocation || CanSee(item);
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

        if (seen) pickup.UpdateBelievedLocation();
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
        if (!p.CanTake()) return false;

        if (p.cx == c.cx && p.cy == c.cy)
        {
            heldItems.Add(p.CharacterTake(c));
            return true;
        }
        if (p.cx == c.cx && p.cy == c.cy + 1)
        {
            c.FaceDirection(Orientation.DOWN);
            heldItems.Add(p.CharacterTake(c));
            return true;
        }
        if (p.cx == c.cx && p.cy == c.cy - 1)
        {
            c.FaceDirection(Orientation.UP);
            heldItems.Add(p.CharacterTake(c));
            return true;
        }
        if (p.cx == c.cx - 1 && p.cy == c.cy)
        {
            c.FaceDirection(Orientation.LEFT);
            heldItems.Add(p.CharacterTake(c));
            return true;
        }
        if (p.cx == c.cx + 1 && p.cy == c.cy)
        {
            c.FaceDirection(Orientation.RIGHT);
            heldItems.Add(p.CharacterTake(c));
            return true;
        }
        return false;
    }

    private void DropItem(PickUpType item, int dx=0, int dy=0)
    {
        var pickup = gameManager.pickups[item];
        if (pickup.status == PickUpStatus.CharacterHeld)
        {
            heldItems.RemoveAll(p => p.type == item);
            pickup.Release(character.cx+dx, character.cy+dy);
            pickup.UpdateBelievedLocation();
        }
    }

    private void PutDownItem(PickUpType item)
    {

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

    private bool ThinksMilkIsOnDiningTable()
    {
        return gameVars.thinksMilkOnDiningTable;
    }

    #region Weight Functions

    private float WeightGoToilet()
    {
        if (gameVars.usedToilet) return -1f;
        return 1f;
    }

    private float WeightFindToothbrush()
    {
        if (IsHolding(PickUpType.Toothbrush)) return -100f;
        if (gameVars.brushedTeeth) return -1f;
        if (!HasBeliefInLocation(PickUpType.Toothbrush))
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
        return 0.85f; ;
    }


    private float WeightGetMilk()
    {
        if (gameVars.ateCereal) return -100f;
        if (IsHolding(PickUpType.Milk)) return -100f;
        if (ThinksMilkIsOnDiningTable()) return -10f;
        if (!HasBeliefInLocation(PickUpType.Milk))
        {
            return -0.4f;
        }
        return 0.82f;
    }

    private float WeightPutMilkOnDiningTable()
    {
        if (!IsHolding(PickUpType.Milk)) return -100f;
        return 0.9f;
    }

    private float WeightGetCereal()
    {
        if (gameVars.ateCereal) return -100f;
        if (IsHolding(PickUpType.Cereal)) return -100f;
        if (!ThinksMilkIsOnDiningTable()) return 0f;
        if (gameVars.isLate) return 0.1f;
        if (!HasBeliefInLocation(PickUpType.Cereal))
        {
            return -0.4f;
        }
        return 0.71f;
    }

    private float WeightGetBowl()
    {
        if (gameVars.ateCereal) return -100f;
        if (IsHolding(PickUpType.Bowl)) return -100f;
        if (!ThinksMilkIsOnDiningTable()) return 0f;
        if (gameVars.isLate) return 0.1f;
        if (!HasBeliefInLocation(PickUpType.Bowl))
        {
            return -0.4f;
        }
        return 0.7f;
    }

    private float WeightEatCereal()
    {
        if (!IsHolding(PickUpType.Bowl)) return -100f;
        if (!IsHolding(PickUpType.Cereal)) return -100f;
        if (!ThinksMilkIsOnDiningTable()) return 0.9f;
        return 1.1f;
    }

    private float WeightStartMakingCoffee()
    {
        if (gameVars.isCoffeeBeingMade) return -100f;

        float value = 0.8f;
        if (!gameVars.ateCereal) value -= 0.2f;
        if (gameVars.isLate) value -= 0.6f;
        return 0.7f;
    }

    private float WeightGetClothes()
    {
        if (gameVars.changedClothes) return -100f;
        if (IsHolding(PickUpType.Coffee)) return -10f;
        if (IsHolding(PickUpType.Clothes)) return -100f;

        float value = 0.7f;
        if (!gameVars.ateCereal) value -= 0.2f;
        if (!HasBeliefInLocation(PickUpType.Clothes)) return -0.1f;
        return value;
    }

    private float WeightChangeClothes()
    {
        if (gameVars.changedClothes) return -100f;
        if (!IsHolding(PickUpType.Clothes)) return -100f;
        if (IsHolding(PickUpType.Coffee)) return -100f;

        float value = 0.8f;
        if (!gameVars.ateCereal) value -= 0.2f;
        return value;
    }

    private float WeightGetNewspaper()
    {
        if (IsHolding(PickUpType.Newspaper)) return -100f;
        if (gameVars.hasReadNewsPapers) return -100f;

        float value = 0.8f;
        if (!gameVars.changedClothes) value -= 0.2f;
        if (gameVars.isLate) value -= 0.7f;

        if (!HasBeliefInLocation(PickUpType.Newspaper)) return -0.1f;
        return value;
    }

    private float WeightGetCoffee()
    {
        if (!gameVars.isCoffeeBeingMade) return -100f;
        if (gameVars.hasDrankCoffee) return -100f;
        if (IsHolding(PickUpType.Clothes)) return -10f;
        if (!gameVars.finishedMakingCoffee) return -10f;
        if (IsHolding(PickUpType.Coffee)) return -100f;

        float value = 0.9f;
        if (!gameVars.changedClothes) value -= 0.25f;
        if (gameVars.isLate) value -= 0.7f;

        if (!HasBeliefInLocation(PickUpType.Coffee)) return -0.1f;
        return value;
    }

    private float WeightSitOnCouch()
    {
        float value = 0f;
        if (IsHolding(PickUpType.Coffee)) value += 0.7f;
        if (IsHolding(PickUpType.Newspaper)) value += 0.7f;
        if (gameVars.isLate) value -= 1.5f;
        return value;
    }

    private float WeightGetKeysAndWallet()
    {
        if (IsHolding(PickUpType.KeysWallet)) return -100f;

        float value = 0.7f;
        if (gameVars.hasDrankCoffee) value += 0.3f;
        if (gameVars.hasReadNewsPapers) value += 0.3f;
        if (gameVars.isLate) value -= 0.1f;
        if (gameVars.isLateAndAwareOfIt) value -= 0.1f;

        if (!HasBeliefInLocation(PickUpType.KeysWallet)) value -= 0.2f;
        return value;
    }

    private float WeightGetBriefcase()
    {
        if (IsHolding(PickUpType.Briefcase)) return -100f;

        float value = 0.9f;
        if (!IsHolding(PickUpType.KeysWallet)) value -= 0.3f;
        if (gameVars.hasDrankCoffee) value += 0.3f;
        if (gameVars.hasReadNewsPapers) value += 0.3f;
        if (gameVars.isLate) value -= 0.1f;
        if (gameVars.isLateAndAwareOfIt) value -= 0.1f;

        if (!HasBeliefInLocation(PickUpType.Briefcase)) value -= 0.2f;
        return value;
    }

    private float WeightGetShoes()
    {
        if (IsHolding(PickUpType.Shoes)) return -100f;

        float value = 0.8f;
        if (!IsHolding(PickUpType.KeysWallet)) value -= 0.2f;
        if (!IsHolding(PickUpType.Briefcase)) value -= 0.2f;
        if (gameVars.isLate) value -= 0;
        if (gameVars.isLateAndAwareOfIt) value -= 0.1f;

        if (!HasBeliefInLocation(PickUpType.Shoes)) value -= 0.2f;
        return value;
    }


    private float WeightLeaveHouse()
    {
        float value = 0.2f;
        if (!IsHolding(PickUpType.Briefcase)) value -= 0.2f;
        if (!IsHolding(PickUpType.KeysWallet)) value -= 0.1f;
        if (!IsHolding(PickUpType.Shoes)) value -= 0.2f;
        if (!gameVars.changedClothes) value -= 0.2f;
        if (!IsHolding(PickUpType.Briefcase)) value -= 0.2f;
        if (!IsHolding(PickUpType.KeysWallet)) value -= 0.2f;
        if (gameVars.isLate) value += 0.5f;
        if (gameVars.isLateAndAwareOfIt) value += 0.3f;
        return value;
    }

    private float WeightMakeBed()
    {
        float value = 0.1f;
        if (gameVars.madeBed) return -100f;
        if (gameVars.findingToothbrushSearchedRoom) value += 0.75f;
        if (gameVars.isLate) value -= 0.5f;
        return value;
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
            Orientation.LEFT, 
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
                DropItem(PickUpType.Toothbrush);
            }
        );
    }

    private void UpdateGetMilk()
    {
        var pickup = gameManager.pickups[PickUpType.Milk];
        GrabItemUpdateFunction(pickup.believedX, pickup.believedY, OnReachGetMilk);
    }


    private void UpdatePutMilkOnDiningTable()
    {
        DefaultUpdateFunction(
            GameVariables.diningTableNextToX,
            GameVariables.diningTableNextToY,
            Orientation.DOWN,
            OnReachPutMilkOnDiningTable,
            JobType.PUT_MILK_ON_DINING_TABLE,
            (gameVars) =>
            {
                gameVars.isMilkOnDiningTable = true;
                gameVars.thinksMilkOnDiningTable = true;
                DropItem(PickUpType.Milk, dx:0, dy:-1);
            }
        );
    }


    private void UpdateGetCereal()
    {
        var pickup = gameManager.pickups[PickUpType.Cereal];
        GrabItemUpdateFunction(pickup.believedX, pickup.believedY, OnReachGetCereal);
    }


    private void UpdateGetBowl()
    {
        var pickup = gameManager.pickups[PickUpType.Bowl];
        GrabItemUpdateFunction(pickup.believedX, pickup.believedY, OnReachGetBowl);
    }

    private void UpdateEatCereal()
    {
        DefaultUpdateFunction(
            GameVariables.diningTableNextToX,
            GameVariables.diningTableNextToY,
            Orientation.DOWN,
            OnReachEatCereal,
            JobType.EAT_CEREAL,
            (gameVars) =>
            {
                DropItem(PickUpType.Cereal, dx: 0, dy: 1);
                DropItem(PickUpType.Bowl, dx: 0, dy: 1);
                gameManager.pickups[PickUpType.Cereal].Deactivate();
                gameManager.pickups[PickUpType.Bowl].Deactivate();
                gameManager.pickups[PickUpType.Milk].Deactivate();
                gameVars.ateCereal = true;
            }
        );
    }


    private void UpdateStartMakingCoffee()
    {
        DefaultUpdateFunction(
            GameVariables.coffeeMachineX,
            GameVariables.coffeeMachineY,
            Orientation.UP,
            OnReachStartMakingCoffee,
            JobType.START_MAKING_COFFEE,
            (gameVars) =>
            {
                gameVars.isCoffeeBeingMade = true;
                gameVars.coffeeFinishTime = Time.time + GameVariables.coffeeMakeTime;
            }
        );
    }

    private void UpdateGetClothes()
    {
        var pickup = gameManager.pickups[PickUpType.Clothes];
        GrabItemUpdateFunction(pickup.believedX, pickup.believedY, OnReachGetClothes);
    }

    private void UpdateChangeClothes()
    {
        DefaultUpdateFunction(
            GameVariables.changeLocationX,
            GameVariables.changeLocationY,
            Orientation.RIGHT,
            OnReachChangeClothes,
            JobType.CHANGE_CLOTHES,
            (gameVars) =>
            {
                gameVars.changedClothes = true;
                DropItem(PickUpType.Clothes);
                gameManager.pickups[PickUpType.Clothes].Deactivate();
            }
        );
    }


    private void UpdateGetNewspaper()
    {
        var pickup = gameManager.pickups[PickUpType.Newspaper];
        GrabItemUpdateFunction(pickup.believedX, pickup.believedY, OnReachGetNewspaper);
    }


    private void UpdateGetCoffee()
    {
        var pickup = gameManager.pickups[PickUpType.Coffee];
        GrabItemUpdateFunction(pickup.believedX, pickup.believedY, OnReachGetCoffee);
    }


    private void UpdateSitOnCouch()
    {
        DefaultUpdateFunction(
            GameVariables.couchX,
            GameVariables.couchY,
            Orientation.UP,
            OnReachSitOnCouch,
            JobType.SIT_ON_COUCH,
            (gameVars) =>
            {
                if (IsHolding(PickUpType.Coffee))
                {
                    gameVars.hasDrankCoffee = true;
                    DropItem(PickUpType.Coffee);
                    gameManager.pickups[PickUpType.Coffee].Deactivate();
                }
                if (IsHolding(PickUpType.Newspaper))
                {
                    gameVars.hasReadNewsPapers = true;
                    DropItem(PickUpType.Newspaper);
                }
            }
        );
    }


    private void UpdateGetKeysAndWallet()
    {
        var pickup = gameManager.pickups[PickUpType.KeysWallet];
        GrabItemUpdateFunction(pickup.believedX, pickup.believedY, OnReachGetKeysAndWallet);
    }

    private void UpdateGetBriefcase()
    {
        var pickup = gameManager.pickups[PickUpType.Briefcase];
        GrabItemUpdateFunction(pickup.believedX, pickup.believedY, OnReachGetBriefcase);
    }

    private void UpdateGetShoes()
    {
        var pickup = gameManager.pickups[PickUpType.Shoes];
        GrabItemUpdateFunction(pickup.believedX, pickup.believedY, OnReachGetShoes);
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
                gameManager.SetBedMadeSprite();
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
        charState.StartJob(JobType.GO_TOILET, 1.8f);
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
        charState.StartJob(JobType.BRUSH_TEETH, 2.4f);
    }

    private void OnReachGetMilk(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.Milk);
        if (!result) LoseBeliefInLocation(PickUpType.Milk);
    }

    private void OnReachPutMilkOnDiningTable(int cx, int cy)
    {
        charState.StartJob(JobType.PUT_MILK_ON_DINING_TABLE, 0.5f);
    }

    private void OnReachGetCereal(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.Cereal);
        if (!result) LoseBeliefInLocation(PickUpType.Cereal);
    }

    private void OnReachGetBowl(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.Bowl);
        if (!result) LoseBeliefInLocation(PickUpType.Bowl);
    }

    private void OnReachEatCereal(int cx, int cy)
    {
        // TODO: Add Clause: no milk on table. -> put down cereal/bowl, find milk.
        charState.StartJob(JobType.EAT_CEREAL, 2f);
    }

    private void OnReachGetClothes(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.Clothes);
        if (!result) LoseBeliefInLocation(PickUpType.Clothes);
    }

    private void OnReachChangeClothes(int cx, int cy)
    {
        charState.StartJob(JobType.CHANGE_CLOTHES, 3f);
    }

    private void OnReachGetCoffee(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.Coffee);
        if (!result) LoseBeliefInLocation(PickUpType.Coffee);
    }


    private void OnReachGetNewspaper(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.Newspaper);
        if (!result) LoseBeliefInLocation(PickUpType.Newspaper);
    }

    private void OnReachStartMakingCoffee(int cx, int cy)
    {
        charState.StartJob(JobType.START_MAKING_COFFEE, 1.2f);
    }

    private void OnReachSitOnCouch(int cx, int cy)
    {
        charState.StartJob(JobType.SIT_ON_COUCH, 5f);
    }

    private void OnReachGetKeysAndWallet(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.KeysWallet);
        if (!result) LoseBeliefInLocation(PickUpType.KeysWallet);
    }

    private void OnReachGetBriefcase(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.Briefcase);
        if (!result) LoseBeliefInLocation(PickUpType.Briefcase);
    }

    private void OnReachGetShoes(int cx, int cy)
    {
        bool result = TryTakeItem(PickUpType.Shoes);
        if (!result) LoseBeliefInLocation(PickUpType.Shoes);
    }

    private void OnReachMakeBed(int cx, int cy)
    {
        charState.StartJob(JobType.MAKE_BED, 3f);
    }

    private void OnReachLeaveHouse(int cx, int cy)
    {
        charState.StartJob(JobType.LEAVE_HOUSE, 0.5f);
    }


    #endregion
}