using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public static event Action<GameObject> OnUnitSpawned;
    public static event Action<GameObject> OnUnitDespawned;

    public float moveSpeed = 5;

    public int harvestDamage = 10;
    public float harvestSpeed = 1; // Amount of times per second.

    public Job job = null;

    private Transform _toolSlot = null;

    private void Start()
    {
        _toolSlot = transform.Find("Tool");
        Debug.Assert(_toolSlot != null);

        OnUnitSpawned?.Invoke(gameObject);
    }

    protected void Update()
    {
        if (job == null)
            ApplyForJob();
        else
            job.Tick();
    }

    private void OnDestroy()
    {
        OnUnitDespawned?.Invoke(gameObject);
    }

    public BlockData GetCurrentBlock()
    {
        Vector3 blockPos = GameObjectUtil.GetObjectBottomPosition(gameObject) - Vector3.up / 2; // Offset with half a block.
        return GameManager.Instance.World.GetSurfaceBlockUnder(blockPos);
    }

    public void ApplyForJob()
    {
        JobManager.Instance.AssignToAvailableJob(this);
    }

    public void ClearJob()
    {
        job = null;
    }

    public void FireFromJob()
    {
        JobManager.Instance.GiveJobBackToLaborMarket(job);
        job = null;
    }

    public void AssignJob(Job newJob)
    {
        job = newJob;

        var tool = JobManager.Instance.tools[job.type];
        if(tool != null)
            Instantiate(tool, _toolSlot);
    }
}
