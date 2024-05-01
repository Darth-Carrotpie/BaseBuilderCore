using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ForceDirectionConfigAuthoring : MonoBehaviour
{
    public float connectionForce = 1f;
    public float pushForce = 1f;

    private class Baker : Baker<ForceDirectionConfigAuthoring>
    {
        public override void Bake(ForceDirectionConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ForceDirectionConfig
            {
                //cellPrefabEntity = GetEntity(authoring.cellPrefab, TransformUsageFlags.Dynamic),
                connectionForce = authoring.connectionForce,
                pushForce = authoring.pushForce,
            });
        }
    }
}

public struct ForceDirectionConfig : IComponentData
{
    public float connectionForce;
    public float pushForce;
}