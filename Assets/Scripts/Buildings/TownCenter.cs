using UnityEngine;
using World;

public class TownCenter : Building
{
    public GameObject VillagerPrefab;

    public int InitialVillagersToSpawn = 3;

    public float VillagerSpawnTime = 30;
    private float _timeSinceLastSpawn = 0;

    public override void OnConstructed()
    {
        base.OnConstructed();

        for(int i = 0; i < InitialVillagersToSpawn; ++i)
        {
            var villager = GameObject.Instantiate(VillagerPrefab);

            villager.PositionOnBlock(gameObject.GetRandomBlockWithinBounds());
        }
    }

}
