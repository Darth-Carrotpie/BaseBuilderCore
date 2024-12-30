using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace BaseBuilderCore {
    public class HexLinkAuthoring : MonoBehaviour {
        public GameObject lineGraphics;

        private class Baker : Baker<HexLinkAuthoring> {
            public override void Bake(HexLinkAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new HexLink {
                    lineGraphics = GetEntity(authoring.lineGraphics, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}
