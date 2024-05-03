using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ForceDirGraphConfigAuthoring : MonoBehaviour
{
    public float pullStrength;
    public float pushStrength;
    public float friction;

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
}
