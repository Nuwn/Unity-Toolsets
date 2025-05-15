using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(PoolingSystemGroup))]
[UpdateAfter(typeof(PoolDespawnSystem))]
public partial struct PoolSpawnSystem : ISystem
{
    public readonly void OnCreate(ref SystemState state) =>
        state.RequireForUpdate<PoolingSingleton>();

    public readonly void OnUpdate(ref SystemState state)
    {
        var singleton = SystemAPI.GetSingletonEntity<PoolingSingleton>();
        var spawnRequests = state.EntityManager.GetBuffer<SpawnEntityRequest>(singleton);
        var availableBuffer = state.EntityManager.GetBuffer<AvailableEntity>(singleton);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        for (int i = 0; i < spawnRequests.Length; i++)
        {
            var request = spawnRequests[i];
            Entity prefab = request.Prefab;
            Entity spawn = Entity.Null;

            // Search for an available entity matching the prefab
            for (int j = 0; j < availableBuffer.Length; j++)
            {
                var candidate = availableBuffer[j].Entity;
                if (state.EntityManager.HasComponent<PrefabType>(candidate) &&
                    state.EntityManager.GetComponentData<PrefabType>(candidate).Prefab == prefab)
                {
                    spawn = candidate;
                    availableBuffer.RemoveAt(j);
                    break;
                }
            }

            // If no available entity found, instantiate a new one
            if (spawn == Entity.Null)
            {
                spawn = ecb.Instantiate(prefab);
                ecb.AddComponent(spawn, new PrefabType { Prefab = prefab });
            }

            // Mark the entity as active
            ecb.AddComponent<ActiveTag>(spawn);
            ecb.SetComponent(spawn, LocalTransform.FromPosition(new (request.Position, 0)));
        }

        // Clear processed spawn requests
        spawnRequests.Clear();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}