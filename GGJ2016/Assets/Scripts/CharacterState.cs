using UnityEngine;
using System;

public class CharacterState
{
    public State state { get; private set; }

    public JobType currentJobType { get; private set; }
    public float jobStartTime { get; private set; }
    public float jobFinishTime { get; private set; }

    public CharacterState()
    {
        state = State.STANDING;
        currentJobType = JobType.NONE;
    }

    public void StartJob(JobType jobType, float taskTime)
    {
        state = State.DOING_JOB;
        
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
        get { return currentJobType != JobType.NONE && Time.time > jobFinishTime; }
    }

    public void SetState(State s)
    {
        Debug.Log("Set State -> " + s);
        if (s == State.DOING_JOB)
        {
            throw new ArgumentException("this function should not be used for DOING_JOB. use UpdateCurrentJobStatus instead");
        }

        state = s;
        currentJobType = JobType.NONE;
    }

    public void CancelJob()
    {
        SetState(State.STANDING);
    }
}