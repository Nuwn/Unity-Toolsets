using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayStateTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // Fetch pooling singleton and its request buffer
        var poolEntity = SystemAPI.GetSingletonEntity<PoolingSingleton>();
        var spawnBuffer = state.EntityManager.GetBuffer<SpawnEntityRequest>(poolEntity);
        uint tick = SystemAPI.GetSingleton<TimeTick>().Tick;

        // Ensure only one spawn per update
        bool spawnedThisUpdate = false;

        foreach (var (enemyPools, enemyType_Pool) in SystemAPI.Query<DynamicBuffer<EnemyPoolBuffer>, RefRO<EnemyTypeComponent>>())
        {
            if (spawnedThisUpdate)
                break; // already spawned one this frame

            // Count active enemies of this type
            int activeCount = 0;
            foreach (var typeComp in SystemAPI.Query<RefRO<EnemyTypeComponent>>().WithAll<EnemyTag, ActiveTag>())
            {
                if (typeComp.ValueRO.EnemyType == enemyType_Pool.ValueRO.EnemyType)
                    activeCount++;
            }

            const int maxEnemies = 5;
            if (activeCount >= maxEnemies)
                continue;

            // Spawn exactly one enemy of this type
            var rng = Random.CreateFromIndex(tick);
            float2 randomPosition = new(rng.NextFloat(-4f, 4f), rng.NextFloat(-4f, 4f));
            var prefab = SelectWeightedRandom(enemyPools, rng);
            spawnBuffer.Add(new SpawnEntityRequest { Prefab = prefab, Position = randomPosition });

            spawnedThisUpdate = true;
        }
    }

    private static Entity SelectWeightedRandom(DynamicBuffer<EnemyPoolBuffer> enemyPools, Random random)
    {
        float totalWeight = 0f;
        foreach (var pool in enemyPools)
            totalWeight += pool.Weight;

        float r = random.NextFloat(0f, totalWeight);
        float cumulative = 0f;
        foreach (var pool in enemyPools)
        {
            cumulative += pool.Weight;
            if (r <= cumulative)
                return pool.Prefab;
        }

        return enemyPools[0].Prefab;
    }
}
