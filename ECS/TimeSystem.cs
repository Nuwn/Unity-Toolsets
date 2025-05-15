using Unity.Burst;
using Unity.Entities;

public struct TimeTick : IComponentData
{
    public uint Tick;
}

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct TimeSystem : ISystem
{
    [BurstCompile]
    public readonly void OnCreate(ref SystemState state)
    {
        var entity = state.EntityManager.CreateSingleton(new TimeTick { Tick = 0 });

#if UNITY_EDITOR
        state.EntityManager.SetName(entity, "TimeTickEntity");
#endif
    }

    [BurstCompile]
    public readonly void OnDestroy(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<TimeTick>()) return;

        Entity tickEntity = SystemAPI.GetSingletonEntity<TimeTick>();
        state.EntityManager.DestroyEntity(tickEntity);
    }

    [BurstCompile]
    public readonly void OnUpdate(ref SystemState state)
    {
        foreach (var timeTick in SystemAPI.Query<RefRW<TimeTick>>())
        {
            timeTick.ValueRW.Tick++;
        }
    }
}