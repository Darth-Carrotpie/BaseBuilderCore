using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace BaseBuilderCore {
    public class ForceLinkAuthoring : MonoBehaviour {
        public GameObject nodeAPrefab;
        public GameObject nodeBPrefab;
        public class Baker : Baker<ForceLinkAuthoring> {
            public override void Bake(ForceLinkAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ForceLink {
                    nodeA = GetEntity(authoring.nodeAPrefab, TransformUsageFlags.Dynamic),
                    nodeB = GetEntity(authoring.nodeBPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}