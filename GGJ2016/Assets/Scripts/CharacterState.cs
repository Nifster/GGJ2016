using UnityEngine;
using System;

public class CharacterState
{
    public State state;

    public State currentJobState;
    public float jobStartTime;
    public float jobFinishTime;

    public void UpdateCurrentJobState(State s, float taskTime)
    {
        if (currentJobState == s)
        {
            // Continue Job.
            return;
        }
        else
        {
            // Start Job.
            currentJobState = s;
            jobStartTime = Time.time;
            jobFinishTime = jobStartTime + taskTime;
        }
    }

    public float ProgressPercent
    {
        get { return Math.Min(1f, (Time.time - jobStartTime)/(jobFinishTime - jobStartTime)); }
    }

    public bool JobDone {
        get { return Time.time > jobFinishTime; }
    }
}