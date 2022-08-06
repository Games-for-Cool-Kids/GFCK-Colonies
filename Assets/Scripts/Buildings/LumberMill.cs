using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberMill : Building
{
    // Create lumberjack
    // -> lumberjack creates chop-wood job
    //          chop job = move to tree + chop/take log + return log
    //               job has list of sub-jobs that are looped over.
    //               job has a loop bool.
    // -> assign villager to job
    // -> building has unused resources
    //      -> building creates pick up resources job

    public override void Start()
    {
        base.Start();

        AddJob(JobType.LUMBERJACK);
    }

    void Update()
    {
        
    }
}
