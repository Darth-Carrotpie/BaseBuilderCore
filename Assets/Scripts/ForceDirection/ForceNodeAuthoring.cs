using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Extensions;

public class ForceNodeAuthoring : MonoBehaviour
{
    //public GameObject unbuiltPrefab;
    public class Baker : Baker<ForceNodeAuthoring>
    {
        public override void Bake(ForceNodeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ForceNode
            {
                //cellUI = GetEntity(authoring.unbuiltPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}
public struct ForceNode : IComponentData
{

}
