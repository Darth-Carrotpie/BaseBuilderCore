using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace BaseBuilderCore {
    public class TestForceDirectionAuthoring : MonoBehaviour {
        public int spawnAmount = 1;
        public int linkAmount = 2;

        private class Baker : Baker<TestForceDirectionAuthoring> {
            public override void Bake(TestForceDirectionAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TestForceDirection {
                    spawnAmount = authoring.spawnAmount,
                    linkAmount = authoring.linkAmount,
                    generateNodes = false
                });
            }
        }
    }
}
