using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace BaseBuilderCore {
    public class ForceDirGraphConfigAuthoring : MonoBehaviour {
        public float repulsiveForce = 2f;
        public float springConstant = 0.5f;
        public float coolingFactor = 0.9f;
        public float initialTemperature = 1f;
        public float temperature = 1f;
        public GameObject linkEntityPrefab;
        public GameObject nodeEntityPrefab;

        private class Baker : Baker<ForceDirGraphConfigAuthoring> {
            public override void Bake(ForceDirGraphConfigAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ForceDirGraphConfig {
                    repulsiveForce = authoring.repulsiveForce,
                    springConstant = authoring.springConstant,
                    coolingFactor = authoring.coolingFactor,
                    initialTemperature = authoring.initialTemperature,
                    temperature = authoring.temperature,
                    linkEntityPrefab = GetEntity(authoring.linkEntityPrefab, TransformUsageFlags.Dynamic),
                    nodeEntityPrefab = GetEntity(authoring.nodeEntityPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}