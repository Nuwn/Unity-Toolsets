using Unity.Entities;
using Unity.Mathematics;


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(PlayerSystemsGroup))]
public partial class PoolingSystemGroup : ComponentSystemGroup{}

public struct PoolingSingleton : IComponentData {}

// Marks an entity as active (in use)
public struct ActiveTag : IComponentData { }

// Marks an entity to be deactivated and returned to the pool
public struct DeactivateEntityTag : IComponentData { }

// A buffer to hold available entities in the pool, we need enemytype for the vfx
public struct AvailableEntity : IBufferElementData
{
    public Entity Entity;
    public double registrationTime;
}

// used for identification in AvailableEntity buffer
public struct PrefabType : IComponentData
{
    public Entity Prefab;
}

// Request to spawn an entity from a prefab
public struct SpawnEntityRequest : IBufferElementData
{
    public Entity Prefab;
    public float2 Position;
}