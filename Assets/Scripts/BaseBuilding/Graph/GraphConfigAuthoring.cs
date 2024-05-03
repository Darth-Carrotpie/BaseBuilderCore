using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GraphConfigAuthoring : MonoBehaviour
{
    public GameObject linkPrefab;
    public float linkWidth;

    private class Baker : Baker<GraphConfigAuthoring>
    {
        public override void Bake(GraphConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GraphConfig
            {
                linkPrefabEntity = GetEntity(authoring.linkPrefab, TransformUsageFlags.Dynamic),
                linkWidth = authoring.linkWidth,
                configEntity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic),
            });
            AddComponent(entity, new LinkOrder { startLinking = false });
        }
    }
}


public struct GraphConfig : IComponentData
{
    public Entity linkPrefabEntity;
    public float linkWidth;
    public Entity configEntity;
}