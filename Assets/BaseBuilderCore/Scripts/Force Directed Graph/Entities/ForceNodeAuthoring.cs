using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics.Extensions;
using Unity.Physics;
using Unity.Mathematics;

namespace BaseBuilderCore {
    public class ForceNodeAuthoring : MonoBehaviour {
        //public GameObject unbuiltPrefab;
        public class Baker : Baker<ForceNodeAuthoring> {
            public override void Bake(ForceNodeAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ForceNode {
                    //buildingRepr - this contains a reference toi the data layer
                });
                AddBuffer<GridCellArea>(entity);
            }
        }
    }
}