using Unity.Entities;

namespace BaseBuilderCore {
    public struct TestForceDirection : IComponentData {
        public int spawnAmount;
        public bool generateNodes;
        public int linkAmount;
    }
}