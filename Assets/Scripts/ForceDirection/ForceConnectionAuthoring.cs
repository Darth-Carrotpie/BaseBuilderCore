using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ForceConnectionAuthoring : MonoBehaviour
{
    public GameObject nodeAPrefab;
    public GameObject nodeBPrefab;
    public class Baker : Baker<ForceConnectionAuthoring>
    {
        public override void Bake(ForceConnectionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ForceConnection
            {
                nodeA = GetEntity(authoring.nodeAPrefab, TransformUsageFlags.Dynamic),
                nodeB = GetEntity(authoring.nodeBPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}
public struct ForceConnection : IComponentData
{
    public Entity nodeA;
    public Entity nodeB;
}
