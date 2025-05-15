using Unity.Burst;
using Unity.Entities;
public struct FixedTimeTick : IComponentData
{
    public uint Tick;
}

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
public partial struct FixedTimeSystem : ISystem
{
    [BurstCompile]
    public readonly void OnCreate(ref SystemState state)
    {
        if (SystemAPI.HasSingleton<FixedTimeTick>()) return;

        Entity tickEntity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(tickEntity, new FixedTimeTick());

#if UNITY_EDITOR
        state.EntityManager.SetName(tickEntity, "FixedTimeTickEntity");
#endif
    }

    [BurstCompile]
    public readonly void OnDestroy(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<FixedTimeTick>()) return;

        Entity tickEntity = SystemAPI.GetSingletonEntity<FixedTimeTick>();
        state.EntityManager.DestroyEntity(tickEntity);
    }

    [BurstCompile]
    public readonly void OnUpdate(ref SystemState state)
    {
        foreach (var timeTick in SystemAPI.Query<RefRW<FixedTimeTick>>())
        {
            timeTick.ValueRW.Tick++;
        }
    }
}
