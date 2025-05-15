using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour{
    public EnemyTypeEnum EnemyType;
}

public partial class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<EnemyTag>(entity);
        AddComponent(entity, new EnemyTypeComponent { EnemyType = (int)authoring.EnemyType });
    }
}