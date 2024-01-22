using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BuildControllerAuthoring : MonoBehaviour
{
    public class Baker : Baker<BuildControllerAuthoring>
    {
        public override void Bake(BuildControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BuildOrder());
            AddComponent(entity, new BlockClickThrough());
        }
    }
}
