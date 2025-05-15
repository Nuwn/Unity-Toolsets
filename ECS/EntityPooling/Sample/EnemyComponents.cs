using Unity.Entities;


public enum EnemyTypeEnum { Neutral, Light, Dark, }

public struct EnemyTag : IComponentData { }

public struct EnemyTypeComponent : IComponentData
{
    public int EnemyType; // Map to EnemyTypeEnum
}

public struct EnemyPoolBuffer : IBufferElementData
{
    public Entity Prefab;
    public float Weight;
}