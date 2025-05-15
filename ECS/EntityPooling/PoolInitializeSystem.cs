using Unity.Entities;

[UpdateInGroup(typeof(PoolingSystemGroup), OrderFirst = true)]
public partial struct PoolInitializeSystem : ISystem
{
    public readonly void OnCreate(ref SystemState state)
    {
        var entity = state.EntityManager.CreateSingleton<PoolingSingleton>();
        state.EntityManager.AddBuffer<SpawnEntityRequest>(entity);
        state.EntityManager.AddBuffer<AvailableEntity>(entity);
    }

    public readonly void OnDestroy(ref SystemState state) => 
        state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<PoolingSingleton>());

    public readonly void OnUpdate(ref SystemState state) => 
        state.Enabled = false;
}