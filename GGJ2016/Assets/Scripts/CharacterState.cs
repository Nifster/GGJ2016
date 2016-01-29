using UnityEngine;
using System;

public class CharacterState
{
    public State state;

    public JobType currentJobType;
    public float jobStartTime;
    public float jobFinishTime;

    public void UpdateCurrentJobState(JobType jobType, float taskTime)
    {
        if (currentJobType == jobType)
        {
            // Continue Job.
            return;
        }
        else
        {
            // Start Job.
            currentJobType = jobType;
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