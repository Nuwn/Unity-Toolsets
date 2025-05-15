using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyPoolAuthoring : MonoBehaviour
{
    public List<EnemyPrefabSpawnData> EnemyPrefabs;
    public EnemyTypeEnum EnemyType;

    [Serializable]
    public class EnemyPrefabSpawnData
    {
        public GameObject Prefab;
        public float Weight;
    }
}

public class EnemyPoolBaker : Baker<EnemyPoolAuthoring>
{
    public override void Bake(EnemyPoolAuthoring authoring)
    {
        if (authoring.EnemyPrefabs == null || authoring.EnemyPrefabs.Count == 0) return;

        var entity = GetEntity(TransformUsageFlags.Dynamic);
        var buffer = AddBuffer<EnemyPoolBuffer>(entity);

        AddComponent(entity, new EnemyTag());
        AddComponent(entity, new EnemyTypeComponent { EnemyType = (int)authoring.EnemyType });

        foreach (var prefab in authoring.EnemyPrefabs)
        {
            if(prefab.Prefab == null) continue;

            var prefabEntity = GetEntity(prefab.Prefab, TransformUsageFlags.Dynamic);
            buffer.Add(new EnemyPoolBuffer { Prefab = prefabEntity, Weight = prefab.Weight });
        }
    }
}