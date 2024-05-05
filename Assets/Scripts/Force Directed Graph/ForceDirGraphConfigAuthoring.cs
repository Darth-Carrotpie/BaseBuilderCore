using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ForceDirGraphConfigAuthoring : MonoBehaviour
{
    public float pullStrength = 1f;
    public float pushStrength = 1f;
    public float friction = 0.05f;
    public float offsetDistance = 2f;

    private class Baker : Baker<ForceDirGraphConfigAuthoring>
    {
        public override void Bake(ForceDirGraphConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ForceDirGraphConfig
            {
                pullStrength = authoring.pullStrength,
                pushStrength = authoring.pushStrength,
                friction = authoring.friction,
                offsetDistance = authoring.offsetDistance,
            });
            AddComponent(entity, new LinkOrder { startLinking = false });
        }
    }
}


public struct ForceDirGraphConfig : IComponentData
{
    public float pullStrength;
    public float pushStrength;
    public float friction;
    public float offsetDistance;
}
