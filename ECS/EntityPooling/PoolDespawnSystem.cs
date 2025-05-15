using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(PoolingSystemGroup))]
[UpdateAfter(typeof(PoolInitializeSystem))]
public partial struct PoolDespawnSystem : ISystem
{
    public readonly void OnCreate(ref SystemState state) => 
        state.RequireForUpdate<PoolingSingleton>();

    public void OnUpdate(ref SystemState state)
    {
        var singleton = SystemAPI.GetSingletonEntity<PoolingSingleton>();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var elapsedTime = SystemAPI.Time.ElapsedTime;

        foreach (var (_, entity) in SystemAPI.Query<RefRO<DeactivateEntityTag>>().WithEntityAccess())
        {
            // Deactivate and return to pool
            ecb.RemoveComponent<ActiveTag>(entity);
            ecb.RemoveComponent<DeactivateEntityTag>(entity);
            state.EntityManager.GetBuffer<AvailableEntity>(singleton).Add(new AvailableEntity { Entity = entity, registrationTime = elapsedTime });
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}