using Unity.Entities;

namespace BaseBuilderCore {
    public struct ForceDirGraphConfig : IComponentData {
        public float repulsiveForce;
        public float springConstant;
        public float coolingFactor;
        public float initialTemperature;
        public float temperature;
        public Entity linkEntityPrefab;
        public Entity nodeEntityPrefab;
    }
}