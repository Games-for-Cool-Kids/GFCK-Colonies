using UnityEngine;
using System.Diagnostics;

public partial class Resource : MonoBehaviour
{
    public ResourceType type;

    public float maxLifeTime = 20.0f;
    private Stopwatch _lifeTimeStopWatch;

    private void Start()
    {
        _lifeTimeStopWatch = new();
        _lifeTimeStopWatch.Start();
    }

    private void Update()
    {
        if (_lifeTimeStopWatch.ElapsedMilliseconds >= maxLifeTime * 1000)
            RemoveFromWorld();
    }

    private void RemoveFromWorld()
    {
        ResourceManager.Instance.RemoveResourceFromWorld(this);
    }

    public void Touch()
    {
        _lifeTimeStopWatch.Restart();
    }
}
