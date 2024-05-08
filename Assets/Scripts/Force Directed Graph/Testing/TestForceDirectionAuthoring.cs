using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TestForceDirectionAuthoring : MonoBehaviour
{
    public int spawnAmount = 1;
    public int linkAmount = 2;
    public GameObject linkEntityPrefab;
    public GameObject nodeEntityPrefab;

    private class Baker : Baker<TestForceDirectionAuthoring>
    {
        public override void Bake(TestForceDirectionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TestForceDirection
            {
                spawnAmount = authoring.spawnAmount,
                linkAmount = authoring.linkAmount,
                linkEntityPrefab = GetEntity(authoring.linkEntityPrefab, TransformUsageFlags.Dynamic),
                nodeEntityPrefab = GetEntity(authoring.nodeEntityPrefab, TransformUsageFlags.Dynamic),
                generateNodes = false
            }) ;
        }
    }
}

public struct TestForceDirection : IComponentData
{
    public int spawnAmount;
    public Entity linkEntityPrefab;
    public Entity nodeEntityPrefab; 
    public bool generateNodes;
    public int linkAmount;
}
